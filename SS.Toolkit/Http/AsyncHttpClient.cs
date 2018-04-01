using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Net;

namespace SS.Toolkit.Http
{

    public class AsyncHttpClient : IDisposable
    {
        private bool _disposed;
        private Uri _uri;
        private Dictionary<string, string> _headers;
        private string _encoding;

        private CookieContainer _cookieContainer;
        private CookieContainerBuilder _cookieContainerBuilder;
        private bool? _expectContinue;
        private bool? _allowAutoRedirect;
        private TimeSpan? _timeout;
        private DecompressionMethods? _automaticDecompression;


        /// <summary>
        /// 只读属性
        /// </summary>
        public CookieContainer CookieContainer
        {
            get
            {
                return _cookieContainer;
            }
        }

        public Dictionary<string, string> Headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new Dictionary<string, string>();
                }
                return _headers;
            }
        }

        public AsyncHttpClient ExpectContinue(bool expectContinue)
        {
            _expectContinue = expectContinue;
            return this;
        }

        public Uri Uri()
        {
            return _uri;
        }

        public AsyncHttpClient Timeout(long timeout)
        {
            _timeout = new TimeSpan(timeout);
            return this;
        }

        public AsyncHttpClient Url(string url)
        {
            return Uri(new Uri(url));
        }

        public AsyncHttpClient Uri(Uri uri)
        {
            _uri = uri;
            return this;
        }

        /// <summary>
        /// 是否允许自动跳转
        /// </summary>
        /// <param name="allowAutoRedirect"></param>
        /// <returns></returns>
        public AsyncHttpClient AllowAutoRedirect(bool allowAutoRedirect)
        {
            _allowAutoRedirect = allowAutoRedirect;
            return this;
        }

        public AsyncHttpClient Encoding(string encoding)
        {
            _encoding = encoding;
            Header("Encoding", encoding);
            return this;
        }

        public AsyncHttpClient AutomaticDecompression(DecompressionMethods automaticDecompression)
        {
            _automaticDecompression = automaticDecompression;
            return this;
        }

        /// <summary>
        /// 使用默认
        /// </summary>
        /// <returns></returns>
        public AsyncHttpClient DefaultUserAgent()
        {
            return UserAgent("Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
        }

        public AsyncHttpClient Header(string name, string value)
        {
            if (_headers == null)
            {
                _headers = new Dictionary<string, string>();
            }
            _headers[name] = value;

            return this;
        }


        public AsyncHttpClient Cookies(string cookies)
        {
            if (cookies != null)
            {
                _cookieContainerBuilder = CookieContainerBuilder.Create(cookies);
            }
            return this;
        }

        public AsyncHttpClient Cookies(AsyncCookieContainer cookies)
        {
            if (cookies != null)
            {
                _cookieContainerBuilder = CookieContainerBuilder.Create(cookies);
            }
            return this;
        }

        public AsyncHttpClient Cookies(CookieContainer cookies)
        {
            _cookieContainerBuilder = CookieContainerBuilder.Create(cookies);
            return this;
        }

        public AsyncHttpClient Referer(string referer)
        {
            if (referer != null)
            {
                Header("Referer", referer);
            }
            return this;
        }

        public AsyncHttpClient UserAgent(string userAgent)
        {
            if (userAgent != null)
            {
                Header("User-Agent", userAgent);
            }
            return this;
        }

        public string ContentType()
        {
            return _headers["Content-Type"];
        }

        public AsyncHttpClient ContentType(string contentType)
        {
            if (contentType != null)
            {
                Header("Content-Type", contentType);
            }
            return this;
        }


        public AsyncHttpClient Accept(string accept)
        {
            if (accept != null)
            {
                Header("Accept", accept);
            }
            return this;
        }


        public async Task<AsyncHttpResponse> Get()
        {
            return await GetInternal();
        }


        private async Task<AsyncHttpResponse> GetInternal()
        {
            using (var client = DoBuildHttpClient())
            {
                try
                {
                    using (var rsp = await client.GetAsync(_uri))
                    {
                        return new AsyncHttpResponse(rsp, _encoding);
                    }
                }
                catch (Exception ex)
                {
                    return new AsyncHttpResponse(ex, _encoding);
                }
            }
        }

        public async Task<AsyncHttpResponse> Post(byte[] args)
        {
            using (var client = DoBuildHttpClient())
            {
                var postData = new ByteArrayContent(args);
                return await PostInternal(client, postData);
            }
        }


        public async Task<AsyncHttpResponse> Post(string args)
        {
            using (var client = DoBuildHttpClient())
            {
                var postData = new StringContent(args, System.Text.Encoding.UTF8, "application/json");
                return await PostInternal(client, postData);
            }
        }

        public async Task<AsyncHttpResponse> Post(Dictionary<string, string> args)
        {
            using (var client = DoBuildHttpClient())
            {
                var postData = new FormUrlEncodedContent(args);
                return await PostInternal(client, postData);
            }
        }

        private async Task<AsyncHttpResponse> PostInternal(HttpClient client, ByteArrayContent postData)
        {
            try
            {
                using (var rsp = await client.PostAsync(_uri, postData))
                {
                    return new AsyncHttpResponse(rsp, _encoding);
                }
            }
            catch (Exception ex)
            {
                return new AsyncHttpResponse(ex, _encoding);
            }
        }



        private HttpClient DoBuildHttpClient()
        {
            var clientHandler = new HttpClientHandler();
            clientHandler.UseCookies = true;
            if (_cookieContainerBuilder != null)
            {
                //需要用到Uri来确定Domain,否则会出错~
                _cookieContainer = _cookieContainerBuilder.Builder(_uri);
                _cookieContainerBuilder = null;//使用一次就不用了
            }

            if (_cookieContainer == null)
            {
                _cookieContainer = new CookieContainer();
            }
            clientHandler.CookieContainer = _cookieContainer;

            if (_automaticDecompression.HasValue)
            {
                clientHandler.AutomaticDecompression = _automaticDecompression.Value;
            }
            if (_allowAutoRedirect.HasValue)
            {
                clientHandler.AllowAutoRedirect = _allowAutoRedirect.Value;
            }
            var client = new HttpClient(clientHandler);
            if (_timeout.HasValue)
            {
                client.Timeout = _timeout.Value;
            }
            if (_expectContinue.HasValue)
            {
                client.DefaultRequestHeaders.ExpectContinue = _expectContinue.Value;
            }
            if (_headers != null)
            {
                foreach (var kv in _headers)
                {
                    client.DefaultRequestHeaders.Add(kv.Key, kv.Value);
                }
            }
            return client;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AsyncHttpClient()
        {
            Dispose(false);
        }

        //这里的参数表示示是否需要释放那些实现IDisposable接口的托管对象
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return; //如果已经被回收，就中断执行
            }
            if (disposing)
            {
                //TODO:释放那些实现IDisposable接口的托管对象
            }
            //释放非托管资源，设置对象为null
            _headers = null;
            _cookieContainerBuilder = null;
            _automaticDecompression = null;
            _uri = null;
            _disposed = true;
        }
    }
    public class AsyncHttpQuery : SortedDictionary<string, string>
    {
        public AsyncHttpQuery(string url)
        {
            ParseUrlInternal(url);
        }

        public AsyncHttpQuery(Uri uri) : this(uri.OriginalString)
        {
        }

        private void ParseUrlInternal(string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            url = url.Trim();
            try
            {
                var split = url.Split(new[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 1)
                {
                    return;
                }
                //获取前面的URL地址
                split.Skip(1).ToList().ForEach((s) =>
                {
                    //没有用String.Split防止某些少见Query String中出现多个=，要把后面的无法处理的=全部显示出来
                    var idx = s.IndexOf('=');
                    try
                    {
                        this.Add(System.Uri.UnescapeDataString(s.Substring(0, idx)), System.Uri.UnescapeDataString(s.Substring(idx + 1)));
                    }
                    catch
                    {

                    }
                });
            }
            catch (Exception ex)
            {
                throw new FormatException("URL格式错误", ex);
            }
        }
    }
}
