using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace SS.Toolkit.Http
{



    public class AsyncHttpClient
    {

        public static AsyncHttpClient Create()
        {
            return new AsyncHttpClient();
        }


        private Uri _uri;
        private Dictionary<string, string> _headers;
        private string _encoding;

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


        public Uri Uri()
        {
            return _uri;
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


        public AsyncHttpClient Encoding(string encoding)
        {
            _encoding = encoding;
            Header("Encoding", encoding);
            return this;
        }

        public AsyncHttpClient SerDefaultUA()
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
                Header("Cookie", cookies);
            }
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
            var client = DoBuildHttpClient();

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

        public async Task<AsyncHttpResponse> Post(byte[] args)
        {
            var client = DoBuildHttpClient();

            var postData = new ByteArrayContent(args);

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


        public async Task<AsyncHttpResponse> Post(string args)
        {
            var client = DoBuildHttpClient();

            var postData = new StringContent(args, System.Text.Encoding.UTF8, "application/json");

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

        public async Task<AsyncHttpResponse> Post(Dictionary<string, string> args)
        {
            var client = DoBuildHttpClient();

            var postData = new FormUrlEncodedContent(args);

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

            HttpClient client = new HttpClient();

            if (_headers != null)
            {
                foreach (var kv in _headers)
                {
                    client.DefaultRequestHeaders.Add(kv.Key, kv.Value);
                }
            }

            return client;
        }
    }
    public class AsyncHttpCookies
    {

        private SortedDictionary<string, AsyncHttpCookie> cookies = new SortedDictionary<string, AsyncHttpCookie>();

        public AsyncHttpCookie this[string key]
        {
            get
            {
                return cookies[key];
            }
        }


        public void Add(AsyncHttpCookie cookie)
        {
            try
            {
                cookies.Add(cookie.Name, cookie);
            }
            catch (Exception ex)
            {

            }
        }

        public override string ToString()
        {
            return string.Join(";", cookies.Values);
        }

    }
    public sealed class AsyncHttpCookie
    {
        public AsyncHttpCookie(string cookie)
        {
            var keys = cookie.Split(';');
            if (keys.Length > 0)
            {
                this.Name = keys[0].Split('=')[0];
                this.Value = keys[0].Split('=')[1];
                if (keys.Length > 1)
                {
                    this.Path = keys[1].Split('=')[1];
                }
                try
                {
                    if (keys.Length > 2)
                    {
                        this.Domain = keys[2].Split('=')[1];
                    }
                    if (keys.Length > 3)
                    {
                        this.MaxAge = keys[3].Split('=')[1];
                    }
                }
                catch
                {

                }
            }
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Path { get; set; }

        public string MaxAge { get; set; }

        public string Domain { get; set; }

        public override string ToString()
        {
            var builder = new List<string>();
            builder.Add($"{Name}={Value}");
            if (!string.IsNullOrEmpty(Path))
            {
                builder.Add($"Path={Path}");
            }
            if (!string.IsNullOrEmpty(Domain))
            {
                builder.Add($"Domain={Domain}");
            }
            if (!string.IsNullOrEmpty(MaxAge))
            {
                builder.Add($"Max-Age={MaxAge}");
            }
            return string.Join(";", builder);
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
                    catch (Exception ex)
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
    public class AsyncHttpResponse : IDisposable
    {
        protected byte[] Data { get; private set; }

        public Dictionary<string, string> Headers { get; private set; }
        public AsyncHttpCookies Cookies { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }
        public Encoding Encoding { get; private set; }
        public Exception Exception { get; private set; }


        internal AsyncHttpResponse(HttpResponseMessage rsp, string encoding)
        {
            this.StatusCode = rsp.StatusCode;
            this.Encoding = Encoding.GetEncoding(encoding ?? "UTF-8");
            this.Headers = new Dictionary<string, string>();
            this.Cookies = null;
            this.Exception = null;
            Init(rsp);
        }

        internal AsyncHttpResponse(Exception exp, string encoding)
        {
            this.StatusCode = 0;
            this.Encoding = Encoding.GetEncoding(encoding ?? "UTF-8");
            this.Headers = new Dictionary<string, string>();
            this.Cookies = null;
            this.Exception = exp;
        }

        protected async void Init(HttpResponseMessage rsp)
        {
            if (rsp.StatusCode == HttpStatusCode.OK)
            {
                this.Data = await rsp.Content.ReadAsByteArrayAsync();
            }
            if (rsp.Headers != null)
            {
                foreach (var kv in rsp.Headers)
                {
                    if ("Set-Cookie".Equals(kv.Key))
                    {
                        if (Cookies == null)
                        {
                            Cookies = new AsyncHttpCookies();
                        }
                        foreach (var item in kv.Value)
                        {
                            AsyncHttpCookie cookie = new AsyncHttpCookie(item);
                            Cookies.Add(cookie);
                        }
                        //var test = string.Join(";", kv.Value);
                    }

                    Headers[kv.Key] = string.Join(";", kv.Value);
                }
            }
            StatusCode = rsp.StatusCode;
            RequestMessage = rsp.RequestMessage;
        }

        public HttpRequestMessage RequestMessage { get; private set; }

        //public async Task<IRandomAccessStream> GetRandomStream()
        //{
        //    if (Data == null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        var buffer = this.GetBuffer();
        //        InMemoryRandomAccessStream inStream = new InMemoryRandomAccessStream();
        //        DataWriter datawriter = new DataWriter(inStream.GetOutputStreamAt(0));
        //        datawriter.WriteBuffer(buffer, 0, buffer.Length);
        //        await datawriter.StoreAsync();
        //        return inStream;
        //    }

        //}

        //public IBuffer GetBuffer()
        //{
        //    if (Data == null)
        //        return null;
        //    else
        //        return WindowsRuntimeBufferExtensions.AsBuffer(Data);
        //}

        public byte[] GetBytes()
        {
            if (Data == null)
                return null;
            else
                return Data;
        }

        public string GetString()
        {
            if (Data == null)
                return null;
            else
                return Encoding.GetString(Data);
        }

        public void Dispose()
        {
            Data = null;
            Headers = null;
            Cookies = null;
            Encoding = null;
            Exception = null;
        }
    }
}
