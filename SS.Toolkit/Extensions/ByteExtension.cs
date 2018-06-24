using System;
using System.IO;

namespace SS.Toolkit.Extensions
{
    public static class ByteExtension
    {

        public static int ToInt32(this byte[] src, int offset)
        {
            int value;
            value = (int)((src[offset] & 0xFF)
                    | ((src[offset + 1] & 0xFF) << 8)
                    | ((src[offset + 2] & 0xFF) << 16)
                    | ((src[offset + 3] & 0xFF) << 24));
            return value;
        }

        public static long ToLong(this byte[] src, int offset)
        {
            long value;
            value = (long)((src[offset] & 0xFF)
                    | ((src[offset + 1] & 0xFF) << 8)
                    | ((src[offset + 2] & 0xFF) << 16)
                    | ((src[offset + 3] & 0xFF) << 24)
                    | ((src[offset + 4] & 0xFF) << 32)
                    | ((src[offset + 5] & 0xFF) << 40)
                    | ((src[offset + 6] & 0xFF) << 48)
                    | ((src[offset + 7] & 0xFF) << 56));
            return value;
        }

        public static byte[] CopyOfRange(this byte[] original, int from, int to)
        {
            int newLength = to - from;
            if (newLength < 0)
                throw new Exception(from + " > " + to);
            byte[] copy = new byte[newLength];
            Array.Copy(original, from, copy, 0, Math.Min(original.Length - from, newLength));
            return copy;
        }

        public static int ToBit(this bool value)
        {
            return value ? 1 : 0;
        }

        public static byte[] ToByteArray(this Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public static byte[] CopyReverse(this byte[] src)
        {
            if (src == null)
                return null;

            var len = src.Length;

            var reverse = new byte[len];
            for (var i = 0; i <= len / 2; ++i)
            {
                var t = src[i];
                var o = len - i - 1;
                reverse[i] = src[o];
                reverse[o] = t;
            }

            return reverse;
        }

    }
}
