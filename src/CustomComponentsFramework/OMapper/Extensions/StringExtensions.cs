using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMapper.Extensions
{



    internal static class StringExtensions
    {
        internal static string Frmt(this String str, params object[] objs)
        {
            return string.Format(str, objs);
        }
    }
}
