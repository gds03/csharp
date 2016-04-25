using System;

namespace VirtualNote.Common.ExtensionMethods
{
    public static class StringExtensions
    {
        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }

        public static bool ToBool(this string str)
        {
            // "true" | "false", "True" | "False" , 1 | 0, "tRue", "falSe"
            if (str.Length == 0)
                throw new InvalidOperationException();
            if (str.Length == 1)
            {
                int v;
                try
                {
                    v = int.Parse(str);
                }
                catch (Exception) { throw new InvalidOperationException(); }
                return v == 1;
            }
            str = str.ToLower();

            if (str == "true")
                return true;
            if (str == "false")
                return false;
            throw new InvalidOperationException();
        }

        public static String UpFirstLetter(this String str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            char firstChar = str[0];
            if (char.IsLower(firstChar))
            {
                char Upper = char.ToUpper(firstChar);
                str = Upper + "" + str.Substring(1);
            }
            return str;
        }

        public static String Truncate(this String str, int maxLength){
            if (string.IsNullOrEmpty(str))
                return String.Empty;

            if (str.Length <= maxLength)
                return str;

            return str.Substring(0, maxLength) + "...";
        }
    }

}