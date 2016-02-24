using CodeToUMLNotation.ModelV2;
using CodeToUMLNotation.ModelV2.Code;
using CodeToUMLNotation.NRefactoryHelper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.NRefactoryHelper.DeclarationConverters
{
    public class StructDeclaration : NRefactoryVisitorV2DeclarationHelper
    {
        public override bool Handle(ICSharpCode.NRefactory.CSharp.ClassType type)
        {
            return type == ICSharpCode.NRefactory.CSharp.ClassType.Struct;
        }

        public override ModelV2.Abstract.Declaration Create(ICSharpCode.NRefactory.CSharp.TypeDeclaration td)
        {
            Struct s = new Struct(
                 AdjustVisibilityForClassesInterfacesAndStructs(td),
                GetNameForGenericTypeDeclaration(td)
            );

            SetBaseTypesForTypeDeclaration(s, td);
            return s;
        }
    }
}
