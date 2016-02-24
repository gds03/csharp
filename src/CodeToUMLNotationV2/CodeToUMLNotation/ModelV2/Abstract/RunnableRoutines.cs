using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2.Abstract
{
    public abstract class RunnableRoutines : Declaration            // usually methods and properties
    {
        public bool Static { get; private set; }

        public bool Virtual { get; private set; }

        public bool Abstract { get; private set; }

        public bool Overrided { get; private set; }

        public String ReturnType { get; private set; }

        public RunnableRoutines(Visibility visibility, String name, 
            bool @static, bool @virtual, bool @abstract, bool overrided, string returnType) : base(visibility, name)
        {
            ParameterValidator.ThrowIfArgumentNullOrEmpty(returnType, "returnType");
            Static = @static;
            Virtual = @virtual;
            Abstract = @abstract;
            Overrided = overrided;
            ReturnType = returnType;
        }

        protected override bool IsStatic
        {
            get { return Static; }
        }
    }
}
