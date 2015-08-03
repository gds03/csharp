using CodeToUMLNotation.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.Model.Abstract
{
    public abstract class Const : IUmlDesignation
    {
        public Visibility Visibility { get; private set; }

        public string Name { get; private set; }

        public string ReturnType { get; private set; }

        public string InitialValue { get; private set; }


        public Const(Visibility visibility, string name, string returnType, string initialValue)
        {
            ParameterValidator.ThrowIfArgumentNullOrEmpty("name", name);
            ParameterValidator.ThrowIfArgumentNullOrEmpty("returnType", returnType);
            ParameterValidator.ThrowIfArgumentNullOrEmpty("initialValue", initialValue);

            Visibility = visibility;
            Name = name;
            ReturnType = returnType;
            InitialValue = initialValue;
        }

        public abstract IRichStringbuilder Design(IRichStringbuilder richerStringBuilder);
    }
}
