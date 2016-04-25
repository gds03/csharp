using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VirtualNote.MVC.Helpers
{
    public static class DropDownHelper
    {
        public static MvcHtmlString MyDropDown(this HtmlHelper helper, 
            String name, IEnumerable<SelectListItem> items, object htmlAttributes)
        {
            // set ViewData first
            helper.ViewData[name] = items;

            // pass in "null" intentionally into the DropDownList extension method
            return System.Web.Mvc.Html.SelectExtensions.DropDownList(helper, name, (IEnumerable<SelectListItem>)null, htmlAttributes);
        }
    }
}