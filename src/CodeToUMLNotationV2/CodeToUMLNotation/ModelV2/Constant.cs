using CodeToUMLNotation.ModelV2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2
{
    public class Constant : IUmlDesign
    {
        
        public Visibility Visibility { get; private set; }

        public string Name { get; private set; }

        public string ReturnType { get; private set; }

        public string InitialValue { get; private set; }


        public Constant(Visibility visibility, string name, string returnType, string initialValue)
        {
            ParameterValidator.ThrowIfArgumentNullOrEmpty("name", name);
            ParameterValidator.ThrowIfArgumentNullOrEmpty("returnType", returnType);
            ParameterValidator.ThrowIfArgumentNullOrEmpty("initialValue", initialValue);

            Visibility = visibility;
            Name = name;
            ReturnType = returnType;
            InitialValue = initialValue;
        }
        public IRichStringbuilder Design(IRichStringbuilder richerStringBuilder)
        {
            // CONSTANT_NAME : constant_type = value
            richerStringBuilder.WriteRegular(Name + " : ");
            richerStringBuilder.WriteBold(ReturnType);
            richerStringBuilder.WriteRegular(" = ");
            richerStringBuilder.WriteItalic(InitialValue);
            return richerStringBuilder;
        }
    }
}
