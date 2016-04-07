using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomComponents.Mvc.UserControls.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ImagesUrl(this UrlHelper url, string name)
        {
            return url.Content("~/Content/Images/") + name;
        }

        public static string JavascriptUrl(this UrlHelper url, string name)
        {
            return url.Content("~/Content/Javascript/") + name;
        }

        public static string StylesheetsUrl(this UrlHelper url, string name)
        {
            return url.Content("~/Content/Stylesheets/") + name;
        }
    }
}