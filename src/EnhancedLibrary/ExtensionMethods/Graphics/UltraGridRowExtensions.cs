using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infragistics.Win.UltraWinGrid;

namespace EnhancedLibrary.ExtensionMethods.Graphics
{
    public static class UltraGridRowExtensions
    {
        /// <summary>
        ///     Gets the object inside the row.
        /// </summary>
        public static T GetListObject<T>(this UltraGridRow r)
        {
            return (T) r.ListObject;
        }
    }
}
