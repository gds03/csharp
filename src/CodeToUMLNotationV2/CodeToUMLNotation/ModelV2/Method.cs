using CodeToUMLNotation.ModelV2.Abstract;
using CodeToUMLNotation.ModelV2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2
{
    public class Method : RunnableRoutines, IUmlDesign
    {
        public bool Ctor { get; private set; }

        public ICollection<KeyValuePair<string, string>> Arguments { get; private set; }

        public Method(Visibility visibility, String name,
            bool @static, bool @virtual, bool @abstract, bool overrided, string returnType,
            bool ctor, ICollection<KeyValuePair<string, string>> arguments) : base(visibility, name, @static, @virtual, @abstract, overrided, returnType)
        {
            Ctor = ctor;
            if (arguments != null && arguments.Count > 0)
                Arguments = arguments;
        }


        public IRichStringbuilder Design(IRichStringbuilder richSb)
        {
            // Visibility MethodName(arg: Type, ...)
            // Visibility (ctor) MethodName(arg: Type, ...)

            // parameters            
            Action<IRichStringbuilder> WriteArgumentsIfExist = innerSb =>
            {
                innerSb.WriteRegular("(");
                if (Arguments != null && Arguments.Count > 0)
                {
                    for (int i = 0; i < Arguments.Count; i++)
                    {
                        var kvp = Arguments.ElementAt(i);
                        innerSb.WriteRegular(kvp.Key + ": ");
                        innerSb.WriteBold(kvp.Value);

                        if ((i + 1) < Arguments.Count)
                            innerSb.WriteRegular(", ");
                    }
                }
                innerSb.WriteRegular(")");
            };

            if( Ctor && Static )
                this.Visibility = new Visibility(Enums.VisibilityMode.@public);

            Visibility.Design(richSb);
            richSb.WriteRegular(" ");
            if (Ctor)
            {
                if (Static) { richSb.WriteRegular("(cctor)"); }
                else { richSb.WriteRegular("(ctor)"); }
            }
            WriteNameHelper(richSb);
            WriteArgumentsIfExist(richSb);
            richSb.WriteRegular(": ");
            richSb.WriteBold(Ctor ? Name : ReturnType);

            if (Abstract)
                richSb.WriteItalic(" ** abstract **");

            if (Virtual)
            {
                richSb.WriteRegular(" « ");
                richSb.WriteUnderline("virtual");
                richSb.WriteRegular(" »");
            }
            if (Overrided)
                richSb.WriteBold(" !overrided");

            return richSb;
        }
    }
}
