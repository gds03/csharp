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
        private readonly ParserForm m_form;


        public NRefactoryCom(ParserForm form)
        {
            ParameterValidator.ThrowIfArgumentNull(form, "form");
            m_form = form;
        }


        public string Parse(PARSE_TYPE type, string inputText)
        {
            // parse through framework
            SyntaxTree tree = new N.CSharpParser().Parse(inputText);

            // dispatch to visitor
            var defaultVisitor = new NRefactoryVisitor();
            tree.AcceptVisitor(defaultVisitor);

            IEnumerable<CLRType> inputTextTypes = defaultVisitor.CLRTypes;
            switch (type)
            {
                case PARSE_TYPE.FIELDS:
                    return inputTextTypes.Aggregate(new StringBuilder(), (sb, clr) => sb.AppendLine(clr.GetFieldsUML())).ToString();
                    
                case PARSE_TYPE.PROPERTIES:
                    return inputTextTypes.Aggregate(new StringBuilder(), (sb, clr) => sb.AppendLine(clr.GetPropertiesUML())).ToString();

                case PARSE_TYPE.METHODS:
                    return inputTextTypes.Aggregate(new StringBuilder(), (sb, clr) => sb.AppendLine(clr.GetMethodsUML())).ToString();

                case PARSE_TYPE.ALL:
                    return inputTextTypes.Aggregate(new StringBuilder(), (sb, clr) => sb.AppendLine(clr.Design())).ToString();

                default:
                    throw new NotSupportedException();

            }
        }


        



    }
}
