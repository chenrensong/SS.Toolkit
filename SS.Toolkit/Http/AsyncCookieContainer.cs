using System;
using System.Collections.Generic;
using System.Net;
using System.Collections;

namespace SS.Toolkit.Http
{
    public class AsyncCookieContainer : SortedDictionary<string, AsyncCookie>
    {
        public static AsyncCookieContainer Create(string cookies)
        {
            AsyncCookieContainer acc = new AsyncCookieContainer();
            string[] tempCookies = cookies.Split(';');
            string tempCookie = null;
            int Equallength = 0;//  =的位置 
            string cookieKey = null;
            string cookieValue = null;
            for (int i = 0; i < tempCookies.Length; i++)
            {
                if (!string.IsNullOrEmpty(tempCookies[i]))
                {
                    tempCookie = tempCookies[i];
                    Equallength = tempCookie.IndexOf("=");
                    if (Equallength != -1)       //有可能cookie 无=，就直接一个cookiename；比如:a=3;ck;abc=; 
                    {
                        cookieKey = tempCookie.Substring(0, Equallength).Trim();
                        if (Equallength == tempCookie.Length - 1)    //这种是等号后面无值，如：abc=; 
                        {
                            cookieValue = string.Empty;
                        }
                        else
                        {
                            cookieValue = tempCookie.Substring(Equallength + 1, tempCookie.Length - Equallength - 1).Trim();
                        }
                    }
                    else
                    {
                        cookieKey = tempCookie.Trim();
                        cookieValue = string.Empty;
                    }
                    acc.Add(new AsyncCookie() { Name = cookieKey, Value = cookieValue });
                }
            }
            return acc;
        }

        public static AsyncCookieContainer Create(CookieContainer cc)
        {
            AsyncCookieContainer acc = new AsyncCookieContainer();
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            foreach (Cookie cookie in lstCookies)
            {
                acc.Add(new AsyncCookie() { Name = cookie.Name, Value = cookie.Value, Domain = cookie.Domain, Path = cookie.Path });
            }
            return acc;
        }

        public void Add(AsyncCookie cookie)
        {
            try
            {
                this.Add(cookie.Name, cookie);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public override string ToString()
        {
            return string.Join(";", this.Values);
        }

    }
}
