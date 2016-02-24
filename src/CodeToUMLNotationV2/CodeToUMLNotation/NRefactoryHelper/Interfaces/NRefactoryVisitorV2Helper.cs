using CodeToUMLNotation.ModelV2.Abstract;
using CSharpParser.ProjectModel;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dec = CodeToUMLNotation.NRefactoryHelper.DeclarationConverters;
using CodeToUMLNotation.Extensions;
using CodeToUMLNotation.ModelV2.Enums;

namespace CodeToUMLNotation.NRefactoryHelper.Interfaces
{
    public abstract class NRefactoryVisitorV2DeclarationHelper
    {
        public readonly static List<NRefactoryVisitorV2DeclarationHelper> Converters = new List<NRefactoryVisitorV2DeclarationHelper>();

        static NRefactoryVisitorV2DeclarationHelper()
        {
            Converters.Add(new dec.ClassDeclaration());
            Converters.Add(new dec.EnumDeclaration());
            Converters.Add(new dec.InterfaceDeclaration());
            Converters.Add(new dec.StructDeclaration());
        }

        public abstract bool Handle(ClassType type);
        public abstract Declaration Create(ICSharpCode.NRefactory.CSharp.TypeDeclaration td);

        public static bool CheckFlag(Modifiers modifiers, Modifiers toCompare)
        {
            return (modifiers & toCompare) == toCompare;
        }



        protected static ModelV2.Visibility AdjustVisibilityForClassesInterfacesAndStructs(EntityDeclaration ed)
        {
            VisibilityMode mode = VisibilityMapper.Map(ed.Modifiers);
            mode = mode == VisibilityMode.@private ? VisibilityMode.@internal : VisibilityMode.@public;

            return new ModelV2.Visibility(mode);
        }

        protected static void SetBaseTypesForTypeDeclaration(ClassesAndStructsAndInterfaces csi, ICSharpCode.NRefactory.CSharp.TypeDeclaration td)
        {
            ParameterValidator.ThrowIfArgumentNull(td, "td");

            foreach (var i in td.BaseTypes)
            {
                SimpleType st = i as SimpleType;
                if (st != null)
                {
                    string generics = null;
                    if (st.TypeArguments != null && st.TypeArguments.Count > 0)
                    {
                         generics = st.TypeArguments.SeparateBy(", ").ToString();
                    }
                    csi.BaseTypes.Add(st.IdentifierToken.Name + ((generics != null) ? ( "<" + generics + ">") : ""));
                }
            }
        }

        public static string GetNameForGenericTypeDeclaration(ICSharpCode.NRefactory.CSharp.TypeDeclaration td)
        {
            ParameterValidator.ThrowIfArgumentNull(td, "td");

            string generics = null;
            if (td.TypeParameters != null && td.TypeParameters.Count > 0)
            {
                generics = td.TypeParameters.Select(x => x.Name).SeparateBy(", ").ToString();
            }
            else
            {
                return td.Name;
            }

            return td.Name + ("<" + generics + ">");
        }
    }
}
