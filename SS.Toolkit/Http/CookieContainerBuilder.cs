using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SS.Toolkit.Http
{
    internal class CookieContainerBuilder
    {
        AsyncCookieContainer _asyncCookieContainer;
        CookieContainer _cookieContainer;

        private CookieContainerBuilder(AsyncCookieContainer asyncCookieContainer)
        {
            _asyncCookieContainer = asyncCookieContainer;
        }

        private CookieContainerBuilder(CookieContainer cookieContainer)
        {
            _cookieContainer = cookieContainer;
        }

        public CookieContainer Builder(Uri uri)
        {
            if (_asyncCookieContainer != null)
            {
                CookieContainer cc = new CookieContainer();
                foreach (var item in _asyncCookieContainer)
                {
                    cc.Add(new Cookie(item.Value.Name, item.Value.Value, "/", uri.Host));
                }
                return cc;
            }
            else if (_cookieContainer != null)
            {
                return _cookieContainer;
            }
            else
            {
                return null;
            }
        }

        public static CookieContainerBuilder Create(string cookies)
        {
            return new CookieContainerBuilder(AsyncCookieContainer.Create(cookies));
        }

        public static CookieContainerBuilder Create(AsyncCookieContainer cookies)
        {
            return new CookieContainerBuilder(cookies);
        }

        public static CookieContainerBuilder Create(CookieContainer cookies)
        {
            return new CookieContainerBuilder(cookies);
        }
    }
}
