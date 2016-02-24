using CodeToUMLNotation.ModelV2.Abstract;
using CodeToUMLNotation.ModelV2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2.Code
{
    public class Enum : Declaration, IUmlDesign, ITypeDeclaration
    {
        const string ENUM_WORD = "« enum »";
        public ICollection<KeyValuePair<string, string>> Values { get; private set; }

        public Enum(Visibility visibility, string name) : base(visibility, name)
        {
            Values = new LinkedList<KeyValuePair<string, string>>();
        }


        protected override bool IsStatic
        {
            get { return false; }
        }

        private IRichStringbuilder DesignHeader(IRichStringbuilder richSb)
        {
            int LINE_CHARS_NUM = Name.Length + 5;
            DrawLine(richSb, LINE_CHARS_NUM, true, '_', true, true);
            richSb.WriteRegular(": ");
            Visibility.Design(richSb);
            richSb.WriteRegular(Name);

            richSb.WriteRegular(" ");
            richSb.WriteItalic(ENUM_WORD);
            LINE_CHARS_NUM += ENUM_WORD.Length;

            richSb.WriteLine();
            DrawLine(richSb, LINE_CHARS_NUM, true, '¯', true, true);
            return richSb;
        }

        public IRichStringbuilder Design(IRichStringbuilder richSb)
        {
            DesignHeader(richSb);
            if (Values.Any())
            {
                StringBuilder sb2 = new StringBuilder();

                foreach (var kvp in Values)
                {
                    sb2.Append(kvp.Key);
                    if (!String.IsNullOrEmpty(kvp.Value))
                    {
                        sb2.Append(" = ");
                        sb2.Append(kvp.Value);
                    }

                    sb2.AppendLine(", ");
                }

                if (sb2.Capacity >= 2)
                    sb2.Remove(sb2.Length - 2, 2);

                string str =  sb2.ToString();
                richSb.WriteRegular(str).WriteLine(); ;
            }

            return richSb;
        }

        public void DesignType(IRichStringbuilder richSb)
        {
            Design(richSb);
        }
    }
}
