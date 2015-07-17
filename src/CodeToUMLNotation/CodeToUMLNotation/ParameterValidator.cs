using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation
{
    public static class ParameterValidator
    {
        public static void ThrowIfArgumentNull(object arg, string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }

        public static void ThrowIfArgumentNullOrEmpty(object arg, string argName)
        {
            ThrowIfArgumentNull(arg, argName);

            string argString = arg as String;
            if( argString != null && argString == string.Empty )
                throw new ArgumentNullException("string is empty");
        }
    }
}
