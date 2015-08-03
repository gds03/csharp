using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.Model.Abstract
{
    public abstract class CodeWithReturnValue : Declaration
    {
        public string ReturnType { get; private set; }
        public bool Overrided { get; private set; }

        public CodeWithReturnValue(bool overrided, bool @static, Visibility visibility, bool @virtual, string name, bool @abstract, string returnType)
            : base(@static, visibility, @virtual, name, @abstract)
        {
            ParameterValidator.ThrowIfArgumentNull(returnType, "tyreturnTypepe");
            ReturnType = returnType;
            Overrided = overrided;
        }


        protected void AppendSuffixOverride(IRichStringbuilder richSb)
        {
            if (Overrided)
                richSb.WriteBold(" !overrided");
        }
    }
}
