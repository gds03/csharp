using CodeToUMLNotation.Model.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeToUMLNotation.Model.Const
{
    public class ConstField : Model.Abstract.Const
    {
        public ConstField(Visibility visibility, string name, string returnType, string initialValue) : base(visibility, name, returnType, initialValue)
        {

        }

        public override IRichStringbuilder Design(IRichStringbuilder richerStringBuilder)
        {
            // CONSTANT_NAME : constant_type = value
            richerStringBuilder.WriteRegular(Name + " : ");
            richerStringBuilder.WriteBold(ReturnType);
            richerStringBuilder.WriteRegular(" = ");
            richerStringBuilder.WriteItalic(InitialValue);
            return richerStringBuilder;
        }
    }
}
