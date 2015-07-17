using CodeToUMLNotation.Model.Abstract;
using CodeToUMLNotation.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.Model
{
    public class Property : CodeWithReturnValue, IUmlDesignation
    {
        public bool Getter { get; private set; }
        public bool Setter { get; private set; }

        public Property(bool overrided, bool @static, Visibility visibility, bool @virtual, string name, bool @abstract, string returnType, bool getter, bool setter)
            : base(overrided, @static, visibility, @virtual, name, @abstract, returnType)
        {
            Getter = getter;
            Setter = setter;
        }

        public string Design()
        {
            string str = string.Format("{0} {1}: {2}", Visibility.Design(), Name, ReturnType);

            // parameters
            if (!Getter)
            {
                if (Setter)
                    str += "{set}";
            }
            else
            {
                // there is getter
                if (!Setter)
                    str += "{get}";
                else str += "{get; set;}";
            }

            str = AppendSuffix(str);
            return AppendSuffixOverride(str);
        }
    }
}
