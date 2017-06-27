using System.IO;
using System.Security.Cryptography;

namespace GlobalPlatform.NET.SecureChannel.SCP02.Cryptography
{
    internal static class TripleDES
    {
        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            using (var des = System.Security.Cryptography.TripleDES.Create())
            {
                des.Mode = CipherMode.ECB;
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
}
