using CodeToUMLNotation.Model;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N = ICSharpCode.NRefactory.CSharp;

namespace CodeToUMLNotation.NRefactoryHelper
{
    public class NRefactoryCom
    {
        private readonly IRichStringbuilder m_richSb;


        public NRefactoryCom(IRichStringbuilder richSb)
        {
            ParameterValidator.ThrowIfArgumentNull(richSb, "richSb");
            m_richSb = richSb;
        }


        public void ParseAndWrite(PARSE_TYPE type, string inputText)
        {
            // parse through framework
            SyntaxTree tree = new N.CSharpParser().Parse(inputText);

            // dispatch to visitor
            var defaultVisitor = new NRefactoryVisitor();
            tree.AcceptVisitor(defaultVisitor);

            IEnumerable<CLRType> CLRTypesDetected = defaultVisitor.CLRTypes;
            switch (type)
            {
                case PARSE_TYPE.FIELDS:
                    CLRTypesDetected.ToList().ForEach(clrType => clrType.WriteFieldsUML(m_richSb));
                    break;
                    
                case PARSE_TYPE.PROPERTIES:
                    CLRTypesDetected.ToList().ForEach(clrType => clrType.WritePropertiesUML(m_richSb));
                    break;

                case PARSE_TYPE.METHODS:
                    CLRTypesDetected.ToList().ForEach(clrType => clrType.WriteMethodsUML(m_richSb));
                    break;

                case PARSE_TYPE.ALL:
                    CLRTypesDetected.ToList().ForEach(clrType => clrType.Design(m_richSb));
                    break;

                default:
                    throw new NotSupportedException();

            }
        }


        



    }
}
