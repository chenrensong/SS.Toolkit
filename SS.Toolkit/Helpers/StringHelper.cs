using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SS.Toolkit.Helpers
{
    public class StringHelper
    {

        private static readonly CreateShim _createShim;

        static StringHelper()
        {
            var stringType = typeof(string);

            if (stringType != null)
            {
                _createShim = BuildCreateShim(stringType);
            }
        }

        private static CreateShim BuildCreateShim(Type stringType)
        {
            var ctor = stringType.GetConstructor(BindingFlags.Instance | BindingFlags.Public,
                                                 Type.DefaultBinder,
                                                 new Type[] { typeof(ReadOnlySpan<char>) },
                                                 modifiers: null);
            if (ctor == null)
            {
                return null;
            }
            var valueParameter = Expression.Parameter(typeof(ReadOnlySpan<char>), "value");
            var call = Expression.New(ctor, valueParameter);
            var lambda = Expression.Lambda<CreateShim>(call, valueParameter);
            return lambda.Compile();
        }

        private delegate string CreateShim(ReadOnlySpan<char> value);

        public static string Create(ReadOnlySpan<char> value)
        {
            if (_createShim != null)
            {
                return _createShim(value);
            }
            var result = new string('\0', value.Length);
            var dest = MemoryMarshal.AsMemory(result.AsMemory()).Span;
            value.CopyTo(dest);
            return result;
        }


        /// 获取错误信息
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetErrorInfo(Exception ex)
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("*************************************** \n");
            msg.AppendFormat(" 异常发生时间： {0} \n", DateTime.Now);
            msg.AppendFormat(" 异常类型： {0} \n", ex.HResult);
            msg.AppendFormat(" 导致当前异常的 Exception 实例： {0} \n", ex.InnerException);
            msg.AppendFormat(" 导致异常的应用程序或对象的名称： {0} \n", ex.Source);
            msg.AppendFormat(" 引发异常的方法： {0} \n", ex.TargetSite);
            msg.AppendFormat(" 异常堆栈信息： {0} \n", ex.StackTrace);
            msg.AppendFormat(" 异常消息： {0} \n", ex.Message);
            msg.Append("***************************************");
            return msg.ToString();
        }

        public static string GetRandomStr(int len)
        {
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                s.Append(rd.Next(1, 10).ToString());
            }
            return s.ToString();
        }

        public static string ToUnderlineStyle(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsUpper(c))
                {
                    if (i > 0)
                    {
                        buf.Append("_");
                    }
                    buf.Append(char.ToLower(c));
                }
                else
                {
                    buf.Append(c);
                }
            }
            return buf.ToString();
        }
    }
}
