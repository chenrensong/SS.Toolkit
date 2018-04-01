using System;
using System.Net;

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
                return _asyncCookieContainer.ToCookieContainer(uri);
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
