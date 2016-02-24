using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.ModelV2.Abstract
{
    public abstract class ClassesAndStructs : ClassesAndStructsAndInterfaces
    {
        public ICollection<Constant> ConstantFields { get; private set; }

        public ICollection<Field> Fields { get; private set; }


        public ClassesAndStructs(Visibility visibility, string name, string[] baseTypes = null)
            : base(visibility, name, baseTypes)
        {
            ConstantFields = new LinkedList<Constant>();
            Fields = new LinkedList<Field>();
        }




        protected bool WriteConstantsUML(IRichStringbuilder richSb)
        {
            ConstantFields.ToList().ForEach(c => c.Design(richSb).WriteLine());
            return ConstantFields.Count > 0;
        }

        protected bool WriteFieldsUML(IRichStringbuilder richSb)
        {
            Fields.OrderBy(x => x.Static).ToList().ForEach(f => f.Design(richSb).WriteLine());
            return Fields.Count > 0;
        }
    }
}
