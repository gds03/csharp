
using CustomComponents.Core.Types;
using CustomComponents.Mvc.Types.Validators.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.Mvc.Types.Validators.TypeDependentValidator
{
    /// <summary>
    ///     Validates if current annotated property of DateTime type is superior or equal than current date.
    /// </summary>
    public class DateSuperiorThanCurrent : TypeDependentValueValidator
    {
        internal static UserDateTime DateTimeNow { get { return new UserDateTime(DateTime.Now, true); } }

        public DateSuperiorThanCurrent(bool AllowEquality = false)
            : base(AllowEquality ? TypeCompareOptions.GreaterOrEqual : TypeCompareOptions.Greater,
                   typeof(DateSuperiorThanCurrent), "DateTimeNow")
        {

        }
    }
}
