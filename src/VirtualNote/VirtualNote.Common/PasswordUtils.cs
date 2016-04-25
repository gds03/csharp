using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using VirtualNote.Common.ExtensionMethods;

namespace VirtualNote.Common
{
    public static class PasswordUtils
    {
        private const int MinSalt = 1000;
        private const int MaxSalt = 2000000000;

        private static readonly Random Random = new Random();
        private static readonly Encoding Encoder = Encoding.Unicode;




        public static int GenerateRandomSalt()
        {
            return Random.Next(MinSalt, MaxSalt);
        }

        static String FilterPassword(String password)
        {
            return password.Trim().ToLower();
        }

        /// <summary>
        ///     Generate a 32 bytes array with encripted password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte[] Encript(String password)
        {
            return SHA256.Create().ComputeHash(Encoder.GetBytes(FilterPassword(password)));
        }

        public static String Confuse()
        {
            return "**********";
        }
    }
}
