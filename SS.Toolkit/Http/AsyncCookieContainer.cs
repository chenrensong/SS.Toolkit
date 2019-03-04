using System;
using System.Collections.Generic;
using System.Net;
using System.Collections;
using System.Diagnostics;

namespace SS.Toolkit.Http
{
    public class AsyncCookieContainer
    {
        private readonly List<Cookie> _CookieList = new List<Cookie>();

        public Cookie this[string name]
        {
            get
            {
                foreach (var c in _CookieList)
                {
                    if (string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return c;
                    }
                }
                return null;
            }
        }

        public Cookie this[int index]
        {
            get
            {
                if (index < 0 || index >= _CookieList.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _CookieList[index];
            }
        }

        public int Count
        {
            get
            {
                return _CookieList.Count;
            }
        }

        public static AsyncCookieContainer Create(string cookies)
        {
            AsyncCookieContainer acc = new AsyncCookieContainer();
            string[] tempCookies = cookies.Split(';');
            string tempCookie = null;
            int equalLength = 0;//  =的位置 
            string cookieKey = null;
            string cookieValue = null;
            for (int i = 0; i < tempCookies.Length; i++)
            {
                if (!string.IsNullOrEmpty(tempCookies[i]))
                {
                    tempCookie = tempCookies[i];
                    equalLength = tempCookie.IndexOf("=");
                    if (equalLength != -1)       //有可能cookie 无=，就直接一个cookiename；比如:a=3;ck;abc=; 
                    {
                        cookieKey = tempCookie.Substring(0, equalLength).Trim();
                        if (equalLength == tempCookie.Length - 1)    //这种是等号后面无值，如：abc=; 
                        {
                            cookieValue = string.Empty;
                        }
                        else
                        {
                            cookieValue = tempCookie.Substring(equalLength + 1, tempCookie.Length - equalLength - 1).Trim();
                        }
                    }
                    else
                    {
                        cookieKey = tempCookie.Trim();
                        cookieValue = string.Empty;
                    }
                    acc.Add(new Cookie() { Name = cookieKey, Value = cookieValue });
                }
            }
            return acc;
        }

        public static AsyncCookieContainer Create(CookieContainer cc)
        {
            AsyncCookieContainer acc = new AsyncCookieContainer();
            List<Cookie> listCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) listCookies.Add(c);
            }
            foreach (Cookie cookie in listCookies)
            {
                acc.Add(new Cookie() { Name = cookie.Name, Value = cookie.Value, Domain = cookie.Domain, Path = cookie.Path });
            }
            return acc;
        }


        public void Add(string cookieStr)
        {
            try
            {
                Cookie cookie = new Cookie();
                bool isSetValue = false;
                var keys = cookieStr.Split(';');
                foreach (var item in keys)
                {
                    var str = item.Trim();
                    if (str.IndexOf('=') != -1)
                    {
                        var k = str.Split('=')[0].Trim();
                        var v = str.Split('=')[1].Trim();
                        if (string.Equals(k, "path", StringComparison.OrdinalIgnoreCase))
                        {
                            cookie.Path = v;
                        }
                        else if (string.Equals(k, "domain", StringComparison.OrdinalIgnoreCase))
                        {
                            cookie.Domain = v;
                        }
                        else if (string.Equals(k, "version", StringComparison.OrdinalIgnoreCase))
                        {
                            if (int.TryParse(v, out int version))
                            {
                                cookie.Version = version;
                            }
                        }
                        else if (string.Equals(k, "comment", StringComparison.OrdinalIgnoreCase))
                        {
                            cookie.Comment = v;
                        }
                        else if (string.Equals(k, "Max-Age", StringComparison.OrdinalIgnoreCase))
                        {
                            //暂时不处理
                            //Max-Age=604800
                        }
                        else
                        {
                            if (isSetValue)
                            {
                                continue;
                            }
                            isSetValue = true;
                            cookie.Name = k;
                            cookie.Value = v;
                        }
                    }
                    else
                    {
                        if (string.Equals(str, "secure", StringComparison.OrdinalIgnoreCase))
                        {
                            cookie.Secure = true;
                        }
                        else if (string.Equals(str, "httponly", StringComparison.OrdinalIgnoreCase))
                        {
                            cookie.HttpOnly = true;
                        }
                    }
                }
                this.Add(cookie);
            }
            catch (Exception ex)
            {
                // to do 
                Debug.WriteLine(ex);
            }
        }

        public void Add(Cookie cookie)
        {
            this._CookieList.Add(cookie);
        }

        public int MaxCookieSize { get; set; } = 4096;// CookieContainer.DefaultCookieLengthLimit;

        public int PerDomainCapacity { get; set; } = 30;// CookieContainer.DefaultPerDomainCookieLimit;

        public int Capacity { get; set; } = 300;// CookieContainer.DefaultCookieLimit;


        public CookieContainer ToCookieContainer(Uri uri)
        {
            CookieContainer cc = new CookieContainer(Capacity, PerDomainCapacity, MaxCookieSize);
            var index = 0;
            foreach (var item in _CookieList)
            {
                index++;
                cc.Add(new Cookie(item.Name, item.Value, "/", uri.Host));
                Debug.WriteLine("index:" + index + " count:" + cc.Count);
            }
            return cc;
        }

        public override string ToString()
        {
            return string.Join(";", this._CookieList);
        }

    }
}
