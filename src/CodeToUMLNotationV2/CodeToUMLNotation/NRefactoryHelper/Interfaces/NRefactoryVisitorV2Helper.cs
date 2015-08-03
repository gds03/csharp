using CodeToUMLNotation.ModelV2.Abstract;
using CSharpParser.ProjectModel;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dec = CodeToUMLNotation.NRefactoryHelper.DeclarationConverters;

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


        protected static void SetBaseTypesForTypeDeclaration(ClassesAndStructsAndInterfaces csi, ICSharpCode.NRefactory.CSharp.TypeDeclaration td)
        {
            ParameterValidator.ThrowIfArgumentNull(td, "td");

            foreach (var i in td.BaseTypes)
            {
                SimpleType st = i as SimpleType;
                if (st != null)
                {
                    csi.ReferencedTypes.Add(st.IdentifierToken.Name);
                }
            }
        }
    }
}
