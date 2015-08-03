using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.Model.Abstract
{
    public abstract class Declaration
    {
        public Visibility Visibility { get; set; }
        public string Name { get; private set; }
        public bool Static { get; private set; }
        public bool Virtual { get; private set; }
        public bool Abstract { get; private set; }


        public Declaration(bool @static, Visibility visibility, bool @virtual, string name, bool @abstract)
        {
            if (Static && (Abstract || Virtual))
                throw new InvalidOperationException("cannot be abstract while static");

            Static = @static;
            Visibility = visibility;
            Virtual = @virtual;
            Abstract = @abstract;

            ParameterValidator.ThrowIfArgumentNullOrEmpty(name, "name");
            Name = name;
        }


        protected void AppendSuffix(IRichStringbuilder richSb)
        {
            if (@Static)
                richSb.WriteRegular(" [STATIC]");

            if (Abstract)
                richSb.WriteItalic(" ** abstract **");

            if (Virtual)
            {
                richSb.WriteRegular(" « ");
                richSb.WriteUnderline("virtual");
                richSb.WriteRegular(" »");
            }
        }
    }
}
