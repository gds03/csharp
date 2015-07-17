using CodeToUMLNotation.Model.Abstract;
using CodeToUMLNotation.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.Model
{
    public class Method : CodeWithReturnValue, IUmlDesignation
    {
        public List<KeyValuePair<string, string>> Arguments { get; private set; }
        public bool Ctor { get; private set; }

        public Method(bool overrided, bool ctor, bool @static, Visibility visibility, bool @virtual, string name, bool @abstract, List<KeyValuePair<string, string>> args, string returnType)
            : base(overrided, @static, visibility, @virtual, name, @abstract,  returnType)
        {
            if (args != null && args.Count > 0)
                Arguments = new List<KeyValuePair<string, string>>(args);

            Ctor = ctor;
        }

        public string Design()
        {
            // parameters

            string str_args = "";

            if (Arguments != null)
            {
                StringBuilder args = Arguments.Aggregate(new StringBuilder(), (sb, kvp) => sb.Append(kvp.Key + ":" + kvp.Value + ", "));
                str_args = args.Remove(args.Length - 2, 2).ToString();
            }

            string str = (!Ctor) ? string.Format("{0} {1}({2}): {3}", Visibility.Design(), Name, str_args, ReturnType)
                                 : string.Format("{0} (ctor) {1}({2})", Visibility.Design(), Name, str_args);

            str = AppendSuffix(str);
            return AppendSuffixOverride(str);            
        }
    }
}
