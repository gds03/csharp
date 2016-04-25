using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;

namespace VirtualNote.MVC.Helpers
{
    public sealed class CheckboxListItem
    {
        public int ID { get; set; }
        public bool Enabled { get; set; }
        public string Text { get; set; }
    }

    public static class CheckboxListHelper
    {
        public static MvcHtmlString CheckboxList(this HtmlHelper helper,
            String name,
            IEnumerable<CheckboxListItem> collection) {
            
            int count = collection.Count();
            if (count > 0)
            {
                TagBuilder spanBuilder = new TagBuilder("span");
                StringBuilder strBuilderTotal = new StringBuilder();

                int idx = 0;
                foreach (CheckboxListItem item in collection)
                {
                    TagBuilder checkboxBuilder = new TagBuilder("input");
                    String strName = String.Format("{0}.{1}", name, idx);

                    checkboxBuilder.MergeAttribute("type", "checkbox");
                    checkboxBuilder.MergeAttribute("name", strName);
                    checkboxBuilder.MergeAttribute("value", item.ID.ToString());
                    if (item.Enabled){
                        checkboxBuilder.MergeAttribute("checked", "checked");
                    }

                    StringBuilder strLine = new StringBuilder(checkboxBuilder.ToString(TagRenderMode.SelfClosing));
                    strLine.Append(String.Format(" {0} <br /><br /> ", item.Text));
                    
                    strBuilderTotal.Append(strLine.ToString());
                    idx++;
                }

                spanBuilder.InnerHtml = strBuilderTotal.ToString();
                return MvcHtmlString.Create(spanBuilder.ToString());
            }
            else
            {
                return MvcHtmlString.Create("");
            }

            // <span>
            //      <input type="checkbox" name="{name}idx" value="{ITEM}.ID" /> {ITEM}.TEXT />
            // ---
            // </span>
        }
    }
}