using System;
using System.Text;

namespace EnhancedLibrary.ExtensionMethods.Business
{
    public static class ExceptionExtensions
    {
        /// <summary>
        ///     Format a message with tabs with the current Exception message, stacktrace and recursively
        ///     inner exceptions.
        /// </summary>
        /// <returns></returns>
        public static String PrepareMessage(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            return _PrepareMessageR(ex, sb, 0).ToString();
        }



        #region Internal Methods
        

        static String FormatConstructedMessage(string exMessage, string taber)
        {
            return exMessage.Replace("\r\n", taber + "\r\n");
        }


        static StringBuilder _PrepareMessageR(Exception ex, StringBuilder sb, int tabs)
        {
            if ( ex.Message.IsNE() )
                return sb;

            string space = GetTabCharactersFor(tabs);

            sb.AppendLine(" {0} ------------   Message  ------------ ".Frmt(space));
            sb.AppendLine(" {0} ".Frmt(space) + FormatConstructedMessage(ex.Message, space));


            if ( !ex.StackTrace.IsNE() )
            {
                sb.AppendLine(" {0} \n------------  StackTrace ------------ ".Frmt(space));
                sb.AppendLine(" {0} ".Frmt(space) + FormatConstructedMessage(ex.StackTrace, space));
            }

            // Recursively
            if ( ex.InnerException != null )
            {
                sb.AppendLine(" {0} \n\n Inner Exception: \n\n".Frmt(space));
                return _PrepareMessageR(ex.InnerException, sb, ++tabs);
            }
                

            return sb;
        }

        static string GetTabCharactersFor(int tabs)
        {
            StringBuilder sb = new StringBuilder();

            while ( tabs-- > 0 )
                sb.Append("\t");

            return sb.ToString();
        }


        #endregion
    }
}
