using CodeToUMLNotation.ModelV2.Abstract;
using CodeToUMLNotation.ModelV2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2
{
    public class Field : Declaration, IUmlDesign
    {
        public bool Static { get; private set; }

        public bool Readonly { get; private set; }

        public String ReturnType { get; private set; }

        public Field(Visibility visibility, String name, 
            bool @static, bool @readonly, string returnType) : base(visibility, name)
        {
            Static = @static;
            Readonly = @readonly;
            ReturnType = returnType;
        }


        public IRichStringbuilder Design(IRichStringbuilder richSb)
        {
            // Visibility Name: ReturnType readonly
            Visibility.Design(richSb);
            richSb.WriteRegular(" " + Name + ": ");
            richSb.WriteBold(ReturnType);

            if (@Readonly)
                richSb.WriteItalic(" readonly");

            if (@Static)
                richSb.WriteRegular(" [STATIC]");

            return richSb;
        }
    }
}
