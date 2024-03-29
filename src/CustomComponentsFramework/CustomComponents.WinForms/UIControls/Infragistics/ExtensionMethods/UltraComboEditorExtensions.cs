﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CustomComponents.WinForms.UIControls.Infragistics.ExtensionMethods
//{
//    public static class UltraComboEditorExtensions
//    {
//        /// <summary>
//        ///     Gets the Value of the Editor.
//        ///     If null, return null, otherwise return the ToString of the Value.
//        /// </summary>
//        public static string GetStringValue(this UltraComboEditor editor)
//        {
//            return (editor.Value == null) ? null : editor.Value.ToString();
//        }


//        /// <summary>
//        ///     Set default ValueMember to "ID" and DisplayMember to "Descricao"
//        /// </summary>
//        public static UltraComboEditor SetDefaultDisplayAndValueMember(this UltraComboEditor editor)
//        {
//            editor.ValueMember = "ID";
//            editor.DisplayMember = "Descricao";

//            return editor;
//        }

//        /// <summary>
//        ///     Set items on the DataSource.
//        /// </summary>
//        public static UltraComboEditor DataBind<TItems>(this UltraComboEditor editor, IList<TItems> items)
//        {
//            IList<TItems> list;

//            if (items == null || items.Count == 0)
//                list = new List<TItems>();

//            else
//            {
//                list = items;
//            }

//            editor.DataSource = list;
//            editor.DataBind();

//            editor.SelectedIndex = -1;
//            return editor;
//        }

//        //public static UltraComboEditor ResetSource<TItems>(this UltraComboEditor editor)
//        //{
//        //    editor.DataSource = new List<TItems>();
//        //    editor.DataBind();

//        //    return editor;
//        //}

//        public static UltraComboEditor ResetSource(this UltraComboEditor editor)
//        {
//            if (editor.DataSource != null)
//            {
//                object dontKnowType = Activator.CreateInstance(editor.DataSource.GetType());

//                editor.DataSource = dontKnowType;
//                editor.DataBind();
//            }

//            return editor;
//        }


//        /// <summary>
//        ///     Binds the items to the DataSource, and select the Description Value
//        /// </summary>
//        public static void SelectDescription<TItemsType>(this UltraComboEditor dropdown, IList<TItemsType> items, string DescriptionToSelect)
//        {
//            DataBind(dropdown, items);

//            if (dropdown.Items.Count > 0)
//            {

//                DescriptionToSelect = DescriptionToSelect.ToLower();

//                for (int i = 0; i < items.Count; i++)
//                {
//                    if (dropdown.Items[i].DisplayText.ToLower() == DescriptionToSelect)
//                    {
//                        dropdown.SelectedIndex = i;
//                        break;
//                    }
//                }
//            }
//        }

//        /// <summary>
//        ///     Unselects the current selected item
//        ///     Does the same as SelectedIndex = -1
//        /// </summary>
//        /// <param name="dropdown"></param>
//        public static void Unselect(this UltraComboEditor dropdown)
//        {
//            dropdown.SelectedIndex = -1;
//        }
//    }
//}
