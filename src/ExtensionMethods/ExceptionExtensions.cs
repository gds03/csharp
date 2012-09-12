using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtensionMethods
{
    public static class ExceptionExtensions
    {
        public static String PrepareMessage(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();


            return _PrepareMessageR(ex, sb, 0).ToString();
        }

        static String FormatConstructedMessage(string exMessage, string taber)
        {
            return exMessage.Replace("\r\n", taber + "\r\n");
        }


        static StringBuilder _PrepareMessageR(Exception ex, StringBuilder sb, int tabs)
        {
            if ( string.IsNullOrEmpty(ex.Message) )
                return sb;

            string space = GetTabCharactersFor(tabs);

            sb.AppendLine(" {0} ------------   Message  ------------ ".Frmt(space));
            sb.AppendLine(" {0} ".Frmt(space) + FormatConstructedMessage(ex.Message, space));


            if ( !string.IsNullOrEmpty(ex.StackTrace) )
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
    }
}
