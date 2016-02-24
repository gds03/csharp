using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomComponents.WinForms.UIControls
{
    public class CustomDatePicker : DateTimePicker
    {
        public CustomDatePicker()
        {
            this.Format = DateTimePickerFormat.Custom;
            this.CustomFormat = "MM/yyyy";

            ValueChanged += new EventHandler(CustomDatePicker_ValueChanged);
            Enter += new EventHandler(CustomDatePicker_Enter);
            Leave += new EventHandler(CustomDatePicker_Leave);
        }






        #region Overrided members



        void CustomDatePicker_Enter(object sender, EventArgs e) { Format = DateTimePickerFormat.Short; }
        void CustomDatePicker_Leave(object sender, EventArgs e) { Format = DateTimePickerFormat.Custom; }

        void CustomDatePicker_ValueChanged(object sender, EventArgs e)
        {
            Value = new DateTime(Value.Year, Value.Month, 1).AddMonths(1).AddDays(-1);
        }



        #endregion



    }
}
