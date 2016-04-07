using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Mvc.Types.Validators.Helpers
{
    /// <summary>
    ///     Enum that serves for comparator mode
    /// </summary>
    public enum TypeCompareOptions
    {
        Less = 0,
        Greater = 1,
        Equal = 2,
        LessOrEqual = 3,
        GreaterOrEqual = 4,
        NotEqual = 5
    }
}
