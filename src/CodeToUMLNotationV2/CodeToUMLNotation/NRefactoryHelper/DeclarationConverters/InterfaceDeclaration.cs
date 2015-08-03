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
    public class InterfaceDeclaration : NRefactoryVisitorV2DeclarationHelper
    {
        public override bool Handle(ICSharpCode.NRefactory.CSharp.ClassType type)
        {
            return type == ICSharpCode.NRefactory.CSharp.ClassType.Interface;
        }

        public override ModelV2.Abstract.Declaration Create(ICSharpCode.NRefactory.CSharp.TypeDeclaration td)
        {
            Interface i = new Interface(
                new Visibility(VisibilityMapper.Map(td.Modifiers)),
                td.Name
            );

            SetBaseTypesForTypeDeclaration(i, td);
            return i;
        }
    }
}
