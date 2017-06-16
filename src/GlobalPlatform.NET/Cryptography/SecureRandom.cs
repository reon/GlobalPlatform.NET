using System;
using System.Security.Cryptography;

namespace GlobalPlatform.NET.Cryptography
{
    public static class SecureRandom
    {
        public static byte[] GetBytes(int length)
        {
            var data = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            return data;
        }
    }
}