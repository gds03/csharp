using System;
using System.Web.Mvc;

namespace VirtualNote.MVC.Helpers
{
    public static class UrlHelper2
    {
        public static String GetUrlFrom(this HtmlHelper htmlHelper,
            string actionName, string controllerName, object routeValues) 
        {
            UrlHelper urlHelper = ((Controller)htmlHelper.ViewContext.Controller).Url;
            return urlHelper.Action(actionName, controllerName, routeValues);
        }

        public static String GetUrlFrom(this HtmlHelper htmlHelper,
            string actionName)
        {
            return GetUrlFrom(htmlHelper, actionName, null, null);
        }

        public static String GetUrlFrom(this HtmlHelper htmlHelper,
            string actionName, string controllerName)
        {
            return GetUrlFrom(htmlHelper, actionName, controllerName, null);
        }

        public static String GetUrlFrom(this HtmlHelper htmlHelper,
            string actionName, object routeValues)
        {
            return GetUrlFrom(htmlHelper, actionName, null, routeValues);
        }

        
    }
}