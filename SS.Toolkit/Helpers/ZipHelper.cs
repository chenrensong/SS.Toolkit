using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace SS.Toolkit.Helpers
{
    public class ZipHelper
    {

        /// <summary>
        /// GZip 压缩
        /// </summary>
        public static string GZipCompress(string source, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] bytes = GZipCompress(encoding.GetBytes(source));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// GZip 压缩
        /// </summary>
        public static byte[] GZipCompress(byte[] source)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var compressStream = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    compressStream.Write(source, 0, source.Length);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// GZip 解压
        /// </summary>
        public static string GZipDecompress(string source, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] bytes = GZipDecompress(Convert.FromBase64String(source));
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// GZip 解压
        /// </summary>
        public static byte[] GZipDecompress(byte[] source)
        {
            using (MemoryStream inStream = new MemoryStream(source))
            {
                inStream.Position = 0;
                using (var decompressStream = new GZipStream(inStream, CompressionMode.Decompress, true))
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];
                        while (true)
                        {
                            int read = decompressStream.Read(buffer, 0, buffer.Length);
                            if (read > 0)
                            {
                                outStream.Write(buffer, 0, read);
                            }
                            else
                            {
                                break;
                            }
                        }
                        return outStream.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Deflate 压缩
        /// </summary>
        public static string DeflateCompress(string source, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] bytes = DeflateCompress(encoding.GetBytes(source));
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Deflate 压缩
        /// </summary>
        public static byte[] DeflateCompress(byte[] source)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (var compressStream = new DeflateStream(ms, CompressionMode.Compress, true))
                {
                    compressStream.Write(source, 0, source.Length);
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Deflate 解压
        /// </summary>
        public static string DeflateDecompress(string source, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] bytes = DeflateDecompress(Convert.FromBase64String(source));
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Deflate 解压
        /// </summary>
        public static byte[] DeflateDecompress(byte[] source)
        {
            using (var inStream = new System.IO.MemoryStream(source))
            {
                inStream.Position = 0;
                using (var deflateStream = new DeflateStream(inStream, CompressionMode.Decompress))
                {
                    deflateStream.Flush();
                    using (var outStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];
                        while (true)
                        {
                            int read = deflateStream.Read(buffer, 0, buffer.Length);
                            if (read > 0)
                            {
                                outStream.Write(buffer, 0, read);
                            }
                            else
                            {
                                break;
                            }
                        }
                        return outStream.ToArray();
                    }
                }
            }
        }
    }
}
