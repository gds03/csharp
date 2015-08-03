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

        public IRichStringbuilder Design(IRichStringbuilder richSb)
        {
            // Visibility MethodName(arg: Type, ...)
            // Visibility (ctor) MethodName(arg: Type, ...)

            // parameters            
            Action<IRichStringbuilder> WriteArgumentsIfExist = innerSb =>  {
                innerSb.WriteRegular("(");
                if (Arguments != null && Arguments.Count > 0)
                {                    
                    for (int i = 0; i < Arguments.Count; i++)
                    {
                        var kvp = Arguments[i];
                        innerSb.WriteRegular(kvp.Key + ": ");
                        innerSb.WriteBold(kvp.Value);

                        if ( (i + 1) < Arguments.Count)
                            innerSb.WriteRegular(", ");
                    }
                }
                innerSb.WriteRegular(")");
            };

            Visibility.Design(richSb);
            richSb.WriteRegular(" ");
            if (Ctor) richSb.WriteRegular("(ctor)");
            richSb.WriteRegular(Name);
            WriteArgumentsIfExist(richSb);
            richSb.WriteRegular(": ");
            richSb.WriteBold(Ctor ? Name : ReturnType);
            AppendSuffix(richSb);
            AppendSuffixOverride(richSb);

            return richSb;
        }
    }
}
