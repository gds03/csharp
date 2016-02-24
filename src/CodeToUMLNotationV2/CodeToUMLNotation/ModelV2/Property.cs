using CodeToUMLNotation.ModelV2.Abstract;
using CodeToUMLNotation.ModelV2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2
{
    public class Property : RunnableRoutines, IUmlDesign
    {
        public bool Getter { get; private set; }
        public bool Setter { get; private set; }

        public Property(Visibility visibility, String name, bool @static, bool @virtual, bool @abstract, bool overrided, string returnType,
            bool getter, bool setter) : base(visibility, name, @static, @virtual, @abstract, overrided, returnType)
        {
            Getter = getter;
            Setter = setter;
        }


        public IRichStringbuilder Design(IRichStringbuilder richSb)
        {
            // Visibility PropertyName: ReturnType {get;set}
            Visibility.Design(richSb);
            richSb.WriteRegular(" ");
            WriteNameHelper(richSb);
            richSb.WriteRegular(": ");
            richSb.WriteBold(ReturnType);
            bool OpenCloseBracket = Getter || Setter;
            if (OpenCloseBracket)
            {
                richSb.WriteRegular("{");
                if (Getter) richSb.WriteRegular("get;");
                if (Setter) richSb.WriteRegular("set;");
                richSb.WriteRegular("}");
            }

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


        protected override bool IsStatic
        {
            get { return Static; }
        }
    }
}
