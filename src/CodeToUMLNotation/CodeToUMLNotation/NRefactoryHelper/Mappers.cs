using CodeToUMLNotation.Model;
using CodeToUMLNotation.Model.Enum;
using CodeToUMLNotation.Model.Enums;
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



    public class CLRAvailableTypeModeMapper
    {
        public static CLRAvailableTypeMode Map(ClassType c)
        {
            switch (c)
            {
                case ClassType.Class:
                    return CLRAvailableTypeMode.@class;

                case ClassType.Enum:
                    return CLRAvailableTypeMode.@enum;

                case ClassType.Interface:
                    return CLRAvailableTypeMode.@interface;

                case ClassType.Struct:
                    return CLRAvailableTypeMode.@struct;

                default: throw new NotSupportedException();
            }
        }
    }
}
