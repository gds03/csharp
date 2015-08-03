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

        public IRichStringbuilder Design(IRichStringbuilder richSb)
        {
            // Visibility PropertyName: ReturnType {get;set}
            Visibility.Design(richSb);
            richSb.WriteRegular(" " + Name + ": ");
            richSb.WriteBold(ReturnType);
            bool OpenCloseBracket = Getter || Setter;
            if (OpenCloseBracket)
            {
                richSb.WriteRegular("{");
                if (Getter) richSb.WriteRegular("get;");
                if (Setter) richSb.WriteRegular("set;");
                richSb.WriteRegular("}");
            }
            
            AppendSuffix(richSb);
            AppendSuffixOverride(richSb);
            return richSb;
        }
    }
}
