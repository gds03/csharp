using CodeToUMLNotation.Model.Abstract;
using CodeToUMLNotation.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.Model
{
    public class Field : CodeWithReturnValue, IUmlDesignation
    {
        public bool Readonly { get; private set; }

        public Field(bool @static, bool @readonly, Visibility visibility, bool @virtual, string name, bool @abstract, string returnType)
            : base(false, @static, visibility, @virtual, name, @abstract, returnType)
        {
            Readonly = @readonly;
        }



        public string Design()
        {
            string str = string.Format("{0} {1}: {2}", Visibility.Design(), Name, ReturnType);
            
            if (@Readonly)
                str += " readonly";

            return AppendSuffix(str);
        }
    }
}

