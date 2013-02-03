using System;
using Infragistics.Win.UltraWinEditors;

namespace EnhancedLibrary.ExtensionMethods.Graphics
{
    public static class UltraDateTimeEditorExtensions
    {
        public static DateTime? GetDateTime(this UltraDateTimeEditor editor)
        {
            if ( editor.Value == null )
                return null;

            return (DateTime) editor.Value;
        }       
    }
}
