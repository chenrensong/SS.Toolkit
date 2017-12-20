using System;
using System.Security.Cryptography;
using System.Text;


namespace SS.Toolkit.Helpers
{
    public class SecurityHelper
    {
        public static string MD5(string toHash, Encoding encoding = default(Encoding))
        {
            if (encoding == default(Encoding))
            {
                encoding = Encoding.UTF8;
            }
            // Create a new instance of the MD5CryptoServiceProvider object.
            var md5Hasher = System.Security.Cryptography.MD5.Create();
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(encoding.GetBytes(toHash));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();  // Return the hexadecimal string.
        }

        /// <summary>
        /// AES加密 
        /// </summary>
        /// <param name="context">待加密的内容</param>
        /// <param name="keyBytes">加密密钥</param>
        /// <returns></returns>
        public static string AESEncrypt(string context, byte[] keyBytes, byte[] ivBytes, int blockSize = 128, int keySize = 128)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = keySize;
            rijndaelCipher.BlockSize = blockSize;


            // 加密密钥
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

            byte[] plainText = Encoding.UTF8.GetBytes(context);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="context"></param>
        /// <param name="keyBytes"></param>
        /// <returns></returns>
        public static string AESDecrypt(string context, byte[] keyBytes, byte[] ivBytes, int blockSize = 128, int keySize = 128)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();

            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = keySize;
            rijndaelCipher.BlockSize = blockSize;

            byte[] encryptedData = Convert.FromBase64String(context);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();

            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }

        public static byte[] HmacMD5Encrypt(String encryptText, byte[] encryptKey)
        {
            HMACMD5 hmac = new HMACMD5(encryptKey);
            byte[] bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(encryptText));
            return bytes;
        }


        /// <summary>
        /// 生成BASE64(H_MAC)
        /// </summary>
        /// <param name="encryptText">被签名的字符串</param>
        /// <param name="encryptKey">秘钥</param>
        /// <returns></returns>
        public static string HmacMD5EncryptToBase64(string encryptText, byte[] encryptKey)
        {
            return Convert.ToBase64String(HmacMD5Encrypt(encryptText, encryptKey));
        }

        /// <summary>
        /// 生成BASE64(H_MAC),压缩H_MAC值
        /// </summary>
        /// <param name="encryptText"></param>
        /// <param name="encryptKey"></param>
        /// <param name="compressLen"></param>
        /// <returns></returns>
        public static String HmacMD5EncryptToBase64(string encryptText, byte[] encryptKey, int compressLen)
        {
            return Convert.ToBase64String(Compress(HmacMD5Encrypt(encryptText, encryptKey), compressLen));
        }


        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <param name="encode">编码方式</param>
        /// <returns></returns>
        public static string EncodeBase64(string source, Encoding encode)
        {
            byte[] bytes = encode.GetBytes(source);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        /// 
        /// <seealso cref="SecurityUtil.EncodeBase64(string,Encoding)">  
        /// 参看SecurityUtil.EncodeBase64(string,Encoding)方法的说明 </seealso>  
        public static string EncodeBase64(string source)
        {
            return EncodeBase64(source, Encoding.UTF8);
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="input"></param>
        /// <param name="toLength"></param>
        /// <returns></returns>
        private static byte[] Compress(byte[] input, int toLength)
        {
            if (toLength < 0)
            {
                return null;
            }
            byte[] output = new byte[toLength];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = 0;
            }

            for (int i = 0; i < input.Length; i++)
            {
                int index_output = i % toLength;
                output[index_output] ^= input[i];
            }

            return output;
        }


    }
}
