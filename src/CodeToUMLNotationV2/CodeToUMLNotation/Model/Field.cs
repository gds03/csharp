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



        public IRichStringbuilder Design(IRichStringbuilder richSb)
        {
            // Visibility Name: ReturnType readonly
            Visibility.Design(richSb);
            richSb.WriteRegular(" " + Name + ": ");
            richSb.WriteBold(ReturnType);

            if (@Readonly)
                richSb.WriteItalic(" readonly");

            AppendSuffix(richSb);
            return richSb;
        }
    }
}

