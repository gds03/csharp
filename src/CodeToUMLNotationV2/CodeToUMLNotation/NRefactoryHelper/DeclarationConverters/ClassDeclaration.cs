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
    public class ClassDeclaration : NRefactoryVisitorV2DeclarationHelper
    {
        public override bool Handle(ICSharpCode.NRefactory.CSharp.ClassType type)
        {
            return type == ICSharpCode.NRefactory.CSharp.ClassType.Class;
        }

        public override ModelV2.Abstract.Declaration Create(ICSharpCode.NRefactory.CSharp.TypeDeclaration td)
        {
            Class c = new Class(
                new Visibility(VisibilityMapper.Map(td.Modifiers)),
                td.Name,
                CheckFlag(td.Modifiers, ICSharpCode.NRefactory.CSharp.Modifiers.Static),
                CheckFlag(td.Modifiers, ICSharpCode.NRefactory.CSharp.Modifiers.Abstract)
                );

            SetBaseTypesForTypeDeclaration(c, td);
            return c;
        }
    }
}
