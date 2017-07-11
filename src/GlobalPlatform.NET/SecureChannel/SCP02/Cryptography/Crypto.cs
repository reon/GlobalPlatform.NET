using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GlobalPlatform.NET.SecureChannel.SCP02.Cryptography
{
    internal static class Crypto
    {
        public static class TripleDes
        {
            /// <summary>
            /// Encrypts data using TripleDES. Uses CBC as the cipher mode. 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="key"></param>
            /// <returns></returns>
            public static byte[] Encrypt(byte[] data, byte[] key) => Encrypt(data, key, CipherMode.CBC);

            /// <summary>
            /// Encrypts data using TripleDES. 
            /// </summary>
            /// <param name="data"></param>
            /// <param name="key"></param>
            /// <param name="cipherMode"></param>
            /// <returns></returns>
            public static byte[] Encrypt(byte[] data, byte[] key, CipherMode cipherMode)
            {
                using (var des = TripleDES.Create())
                {
                    des.Mode = cipherMode;
                    des.Padding = PaddingMode.None;
                    des.IV = new byte[8];
                    des.Key = key;

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                        }

                        return ms.ToArray();
                    }
                }
            }
        }

        public static class Mac
        {
            public static byte[] FullTripleDes(byte[] data, byte[] key)
            {
                byte[] ciphertext = TripleDes.Encrypt(data, key);

                return ciphertext.Skip(ciphertext.Length - 8).Take(8).ToArray();
            }
        }
    }
}
