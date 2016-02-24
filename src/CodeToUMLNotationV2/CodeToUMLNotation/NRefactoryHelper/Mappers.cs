using CodeToUMLNotation.ModelV2.Enums;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.NRefactoryHelper
{
    public class VisibilityMapper
    {
        public static VisibilityMode Map(Modifiers m)
        {
            if ((m & Modifiers.Private) == Modifiers.Private)
                return VisibilityMode.@private;

            if ((m & Modifiers.Protected) == Modifiers.Protected)
                return VisibilityMode.@protected;

            if ((m & Modifiers.Internal) == Modifiers.Internal)
                return VisibilityMode.@internal;

            if ((m & Modifiers.Public) == Modifiers.Public)
                return VisibilityMode.@public;

            return VisibilityMode.@private;
        }
    }
}
