//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.WinForms.UIControls.Infragistics
//{
//    public class CheckedListBoxAdapter : UltraCheckEditor
//    {
//        CheckedListBox m_checkedList;






//        public CheckedListBoxAdapter()
//        {
//            CheckedChanged += (sender, args) =>
//            {
//                if (m_checkedList == null)
//                    throw new InvalidOperationException("You must set the CheckedListBox with the Property CheckedListBox");

//                UpdateItems();
//            };
//        }




//        public CheckedListBoxAdapter(CheckedListBox checkedList)
//        {
//            if (checkedList == null)
//                throw new ArgumentNullException("checkedList");

//            m_checkedList = checkedList;

//            CheckedChanged += (sender, args) => UpdateItems();
//        }



//        /// <summary>
//        ///     If true, enable all items. 
//        ///     Otherwise, disable all items
//        /// </summary>
//        [Browsable(true)]
//        public new bool Checked
//        {
//            set
//            {
//                base.Checked = value;
//                UpdateItems();
//            }
//            get
//            {
//                return base.Checked;
//            }
//        }


//        /// <summary>
//        ///     Return all indices from the checkedListbox that are checked
//        /// </summary>
//        [Browsable(true)]
//        public IEnumerable<int> CheckedIndices
//        {
//            get { return GetIndices(true); }
//        }



//        /// <summary>
//        ///     Return all indices from the checkedListbox that are unchecked
//        /// </summary>
//        [Browsable(true)]
//        public IEnumerable<int> UncheckedIndices
//        {
//            get { return GetIndices(false); }
//        }


//        /// <summary>
//        ///     Get or sets the CheckedListBox object associated
//        /// </summary>
//        [Browsable(true)]
//        public CheckedListBox CheckedListBox
//        {
//            get { return m_checkedList; }
//            set { m_checkedList = value; }
//        }


//        void UpdateItems()
//        {
//            if (InvokeRequired)
//                throw new InvalidOperationException();

//            //
//            // In the case where the control's handle has not yet been created, 
//            // you should not simply call properties, methods, or events on the control. 
//            // This might cause the control's handle to be created on the background thread, 
//            // isolating the control on a thread without a message pump 
//            // and making the application unstable.
//            //


//            if (!IsHandleCreated)
//                return;

//            m_checkedList.Invoke((Action)delegate
//            {
//                for (int i = 0; i < m_checkedList.Items.Count; i++)
//                {
//                    m_checkedList.SetItemChecked(i, Checked);
//                }
//            });
//        }

//        IEnumerable<int> GetIndices(bool Checked)
//        {
//            List<int> r = new List<int>(m_checkedList.Items.Count);

//            for (int i = 0; i < m_checkedList.Items.Count; i++)
//            {
//                if (m_checkedList.GetItemChecked(i) == Checked)
//                    r.Add(i);
//            }

//            return r;
//        }

//    }
//}
