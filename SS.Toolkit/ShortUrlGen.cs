using System;
using System.Linq;


namespace SS.Toolkit
{
    public class ShortUrlGen
    {
        /// <summary>
        /// 短码长度        
        /// </summary>
        private const int _codeLength = 8;
        private readonly string _seqKey;

        public ShortUrlGen(string seqKey = "s9LFkgy5RovixI1aOf8UhdY3r4DMplQZJXPqebE0WSjBn7wVzmN2Gc6THCAKut")
        {
            this._seqKey = seqKey;
        }

        /// <summary>
        /// 注意：超过设定的长度可能会有异常数据
        /// </summary>
        private int MaxLength
        {
            get
            {
                return DecodeInternal("".PadLeft(_codeLength, _seqKey.Last())).ToString().Length;
            }
        }

        /// <summary>
        /// 生成随机的0-9a-zA-Z字符串
        /// </summary>
        /// <returns></returns>
        private static string GenerateKeys()
        {
            string[] chars = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z".Split(',');
            int seekSeek = unchecked((int)DateTime.Now.Ticks);
            Random SeekRand = new Random(seekSeek);
            for (int i = 0; i < 100000; i++)
            {
                int r = SeekRand.Next(1, chars.Length);
                string f = chars[0];
                chars[0] = chars[r - 1];
                chars[r - 1] = f;
            }
            return string.Join("", chars);
        }

        /// <summary>
        /// 10进制转换为62进制【简单】
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string EncodeInternal(long id)
        {
            if (id < 62)
            {
                return _seqKey[(int)id].ToString();
            }
            int y = (int)(id % 62);
            long x = id / 62;

            return EncodeInternal(x) + _seqKey[y];
        }

        /// <summary>
        /// 将62进制转为10进制【简单】
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private long DecodeInternal(string num)
        {
            long v = 0;
            int Len = num.Length;
            for (int i = Len - 1; i >= 0; i--)
            {
                int t = _seqKey.IndexOf(num[i]);
                double s = (Len - i) - 1;
                long m = (long)(Math.Pow(62, s) * t);
                v += m;
            }
            return v;
        }

        /// <summary>
        /// 10进制转换为62进制【混淆】
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public string Encode(long num)
        {
            if (num.ToString().Length > MaxLength)
                throw new Exception($"转换值不能超过最大位数{MaxLength}");
            var n = num.ToString()
                   .PadLeft(MaxLength, '0')
                   .ToCharArray()
                   .Reverse();
            return EncodeInternal(long.Parse(string.Join("", n))).PadLeft(_codeLength, _seqKey.First());
        }

        /// <summary>
        /// 将62进制转为10进制【混淆】
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public long Decode(string num)
        {
            if (num.Length > _codeLength + 1)
                throw new Exception($"转换值不能超过最大位数{_codeLength + 1}");
            var n = DecodeInternal(num).ToString().PadLeft(MaxLength, '0')
                .ToCharArray()
                .Reverse();
            return long.Parse(string.Join("", n));
        }
    }
}
