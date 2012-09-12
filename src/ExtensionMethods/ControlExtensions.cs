using System.Windows.Forms;
using System.Collections.Generic;
using System;

namespace ExtensionMethods
{
    public enum SearchType
    {
        Shallow = 0,
        Deep = 1
    }


    /// <summary>
    ///     Extends the behavior of Control interface
    /// </summary>
    /// 
    public static class ControlExtensions
    {
        /// <summary>
        ///     Allows you to add a variable number of controls to the context control.
        /// </summary>
        public static void AddChilds(this Control c, params Control[] controls)
        {
            foreach (Control ctrl in controls)
                c.Controls.Add(ctrl);
        }

        /// <summary>
        ///     Get the tag from the control and receive a generic type argument to cast to.
        /// </summary>
        public static TValue GetTag<TValue>(this Control control)
        {
            if ( control.Tag == null )
                return default(TValue);

            return (TValue) control.Tag;
        }


        public static IEnumerable<Control> AllControlsWithin(this Control control, SearchType searchType)
        {
            switch(searchType)
            {
                case SearchType.Shallow:
                    return AllControlsWithinShallow(control);

                case SearchType.Deep:
                    return AllControlsWithinDeep(control);

                default:
                    throw new NotSupportedException();
            }
        }


        static IEnumerable<Control> AllControlsWithinShallow(Control control)
        {
            LinkedList<Control> result = new LinkedList<Control>();

            foreach ( object objControl in control.Controls )
            {
                Control c = (Control) objControl;
                result.AddLast(c);
            }

            return result;
        }





        #region Recursive 
        

        static IEnumerable<Control> AllControlsWithinDeep(Control control)
        {
            LinkedList<Control> result = new LinkedList<Control>();

            foreach (object objControl in control.Controls)
            {
                Control c = (Control)objControl;
                AllControlosWithinR(c, result);
            }

            return result;
        }



        static void AllControlosWithinR(Control control, LinkedList<Control> result)
        {
            // Stop test
            if (control == null)
                return;

            // Add current control
            result.AddLast(control);

            // Add all childs
            foreach (object objControl in control.Controls)
            {
                Control c = (Control) objControl;
                AllControlosWithinR(c, result);
            }
        }

        #endregion
    }
}
