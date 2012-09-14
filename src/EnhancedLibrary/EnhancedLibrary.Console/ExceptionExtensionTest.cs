using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnhancedLibrary.ExtensionMethods.Business;

namespace EnhancedLibrary.Console
{
    class MyErrorTypeInitialization
    {
        static MyErrorTypeInitialization()
        {
            throw new InvalidOperationException("Just for a test");
        }

        public int X { get { return 0; } }
    }


    public class ExceptionExtensionTest
    {
        public static void Execute()
        {
            try
            {
                global::System.Console.WriteLine("+++ This will give you an error +++ \n \n ");
                MyErrorTypeInitialization t = new MyErrorTypeInitialization();

                int x = t.X;


                global::System.Console.WriteLine("+++ You should never get here +++ ");
            }


            catch ( Exception ex )
            {
                global::System.Console.WriteLine("+++ Testing the extension method +++\n \n");
                global::System.Console.WriteLine(ex.PrepareMessage());
            }


        }
    }
}
