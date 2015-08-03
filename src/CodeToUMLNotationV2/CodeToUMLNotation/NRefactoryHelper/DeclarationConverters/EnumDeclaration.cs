using CodeToUMLNotation.ModelV2;
using CodeToUMLNotation.NRefactoryHelper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.NRefactoryHelper.DeclarationConverters
{
    public class EnumDeclaration : NRefactoryVisitorV2DeclarationHelper
    {
        public override bool Handle(ICSharpCode.NRefactory.CSharp.ClassType type)
        {
            return type == ICSharpCode.NRefactory.CSharp.ClassType.Enum;
        }

        public override ModelV2.Abstract.Declaration Create(ICSharpCode.NRefactory.CSharp.TypeDeclaration td)
        {
            // CodeToUMLNotation.ModelV2.Code.Enum e = new ModelV2.Code.Enum(td.)
            ModelV2.Code.Enum e = new ModelV2.Code.Enum(
                new Visibility(VisibilityMapper.Map(td.Modifiers)),
                td.Name
            );

            return e;

        }
    }
}
