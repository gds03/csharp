using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnhancedLibrary.Utilities.Business
{
    public static class CustomEnvironment
    {
        /// <summary>
        ///     Creates a that represent Changes of lines
        /// </summary>
        public static string NewLine(int NumberOfLines)
        {
            StringBuilder sb = new StringBuilder();
            while ( ( NumberOfLines-- ) > 0 )
                sb.Append(Environment.NewLine);
            
            return sb.ToString();
        }
    }
}
