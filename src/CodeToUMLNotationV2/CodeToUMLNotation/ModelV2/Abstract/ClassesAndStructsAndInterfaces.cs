using CodeToUMLNotation.ModelV2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2.Abstract
{
    public abstract class ClassesAndStructsAndInterfaces : Declaration
    {
        public ICollection<String> BaseTypes { get; private set; }       

        public ICollection<Property> Properties { get; private set; }           

        public ICollection<Method> Methods { get; private set; }

        public ICollection<String> ReferencedTypes { get; private set; }



        public ClassesAndStructsAndInterfaces(Visibility visibility, string name, string[] baseTypes = null)
            : base(visibility, name)
        {
            BaseTypes = new LinkedList<String>();
            Properties = new LinkedList<Property>();
            Methods = new LinkedList<Method>();
            ReferencedTypes = new LinkedList<String>();

            if (baseTypes != null && baseTypes.Length > 0)
            {
                // add each type
                baseTypes.ToList().ForEach(bt => BaseTypes.Add(bt));
            }
        }


        #region Helpers

        public void DesignHeader(IRichStringbuilder richSb)
        {
            int LINE_CHARS_NUM = Name.Length + 5;
            DrawLine(richSb, LINE_CHARS_NUM, true, '_', true, true);
            richSb.WriteRegular(": ");
            Visibility.Design(richSb);
            richSb.WriteRegular(Name);

            int charsWritten = DesignHeaderConcrete(richSb);

            if (BaseTypes.Any())
            {
                richSb.WriteRegular(" : ");
                charsWritten += (int) Math.Round((double)WriteBaseTypesUML(richSb) * 1.7d);
            }

            richSb.WriteLine();
            DrawLine(richSb, LINE_CHARS_NUM + charsWritten, true, '¯', true, true);
        }

        protected abstract int DesignHeaderConcrete(IRichStringbuilder richSb);


        protected int WriteBaseTypesUML(IRichStringbuilder richSb)
        {
            var sbuilder = BaseTypes.Aggregate(new StringBuilder(), (sb, s) => sb.Append(s + ", "));
            string baseTypes = sbuilder.Remove(sbuilder.Length - 2, 2).ToString();
            richSb.WriteBold(baseTypes);
            return sbuilder.Length;
        }


        protected bool WriteReferencesUML(IRichStringbuilder richSb)
        {
            ReferencedTypes.ToList().ForEach(x => richSb.WriteRegular("references: " + x).WriteLine());
            return ReferencedTypes.Count > 0;
        }
                
        protected bool WritePropertiesUML(IRichStringbuilder richSb)
        {
            Properties.OrderBy(x => x.Static).ToList().ForEach(p => p.Design(richSb).WriteLine());
            return Properties.Count > 0;
        }
        protected bool WriteMethodsUML(IRichStringbuilder richSb)
        {
            Methods.OrderBy(x => x.Static).ToList().ForEach(m => m.Design(richSb).WriteLine());
            return Methods.Count > 0;
        }


        #endregion
    }
}
