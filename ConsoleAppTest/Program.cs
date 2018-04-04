using SS.Toolkit.Http;
using System;

namespace ConsoleAppTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            AsyncHttpClient client = new AsyncHttpClient();
            var result = client.Url($"http://ftrace.baidu.com/webCms/ums/queryUms?from=cuid&&cuid=0000032F6875678A93A7106191ADF834|505733030667568").Get();//.Result;
            var resultStr = result.Result.GetString();
        }
    }
}
