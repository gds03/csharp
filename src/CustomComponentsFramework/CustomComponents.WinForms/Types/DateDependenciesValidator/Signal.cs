using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.WinForms.Types.DateDependenciesValidator
{
    public enum Signal
    {
        [Description("=")]
        Equal = 0,

        [Description("<")]
        Lesser = 1,

        [Description(">")]
        Greater = 2,

        [Description("<=")]
        LesserOrEqual = 3,

        [Description(">=")]
        GreaterOrEqual = 4,

        [Description("!=")]
        NotEqual = 5
    }
}
