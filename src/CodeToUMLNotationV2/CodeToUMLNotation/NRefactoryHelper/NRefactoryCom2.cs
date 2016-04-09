using CodeToUMLNotation.ModelV2;
using CodeToUMLNotation.ModelV2.Abstract;
using CodeToUMLNotation.ModelV2.Interfaces;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N = ICSharpCode.NRefactory.CSharp;

namespace CodeToUMLNotation.NRefactoryHelper
{
    public class NRefactoryCom2
    {
        private readonly IRichStringbuilder m_richSb;


        public NRefactoryCom2(IRichStringbuilder richSb)
        {
            ParameterValidator.ThrowIfArgumentNull(richSb, "richSb");
            m_richSb = richSb;
        }


        public void ParseAndWrite(PARSE_TYPE type, string inputText)
        {
            // parse through framework
            SyntaxTree tree = new N.CSharpParser().Parse(inputText);

            // dispatch to visitor
            var defaultVisitor = new NRefactoryVisitorV2();
            tree.AcceptVisitor(defaultVisitor);

            IEnumerable<Declaration> CLRDeclarations = defaultVisitor.Declarations;
            switch (type)
            {
                case PARSE_TYPE.FIELDS:
                    CLRDeclarations.OfType<ClassesAndStructs>().ToList().ForEach(cs => cs.Fields.ToList().ForEach(f => f.Design(m_richSb).WriteLine()));
                    break;
                    
                case PARSE_TYPE.PROPERTIES:
                    CLRDeclarations.OfType<ClassesAndStructsAndInterfaces>().ToList().ForEach(csi => csi.Properties.OrderByDescending(x => !x.Static).ToList().ForEach(p => p.Design(m_richSb).WriteLine()));
                    break;

                case PARSE_TYPE.METHODS:                    
                    CLRDeclarations.OfType<ClassesAndStructsAndInterfaces>().ToList().ForEach(csi => csi.Methods.OrderByDescending(x => !x.Static).ToList().Aggregate(new List<Method>(), (l, m) => {
                        l.Insert((m.Ctor && m.Static) ? 0 : l.Count, m);
                        return l;
                    })
                    .ForEach(m => m.Design(m_richSb).WriteLine()));
                    break;

                case PARSE_TYPE.ALL:
                    CLRDeclarations.OfType<ITypeDeclaration>().ToList().ForEach(clrType => clrType.DesignType(m_richSb));
                    break;

                default:
                    throw new NotSupportedException();

            }
        }


        



    }
}
