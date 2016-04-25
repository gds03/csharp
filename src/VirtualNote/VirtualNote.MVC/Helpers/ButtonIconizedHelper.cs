using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace VirtualNote.MVC.Helpers
{
    public static class ButtonIconizedHelper
    {
        static MvcHtmlString _ButtonIconized(HtmlHelper htmlHelper,
            string buttonText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            string url = htmlHelper.GetUrlFrom(actionName, controllerName, routeValues);

            TagBuilder button = new TagBuilder("button");
            button.MergeAttributes(new RouteValueDictionary(htmlAttributes), true);

            button.InnerHtml = String.Format("<span></span> {0}", buttonText);

            return MvcHtmlString.Create(CommonExtensions.AnchorWithInnerHtml(url, button.ToString()));



            //<button class="btn-icon btn-grey btn-comment">
            //    <span></span>
            //    Comment
            //</button>
        }

        public static MvcHtmlString ButtonIconized(this HtmlHelper htmlHelper,
            string buttonText, string actionName)
        {
            return _ButtonIconized(htmlHelper, buttonText, actionName, null, null, null);
        }

        public static MvcHtmlString ButtonIconized(this HtmlHelper htmlHelper,
            string buttonText, string actionName, string controllerName)
        {
            return _ButtonIconized(htmlHelper, buttonText, actionName, controllerName, null, null);
        }

        public static MvcHtmlString ButtonIconized(this HtmlHelper htmlHelper,
            string buttonText, string actionName, object routeValues)
        {
            return _ButtonIconized(htmlHelper, buttonText, actionName, null, routeValues, null);
        }

        public static MvcHtmlString ButtonIconized(this HtmlHelper htmlHelper,
            string buttonText, string actionName, string controllerName, object routeValues)
        {
            return _ButtonIconized(htmlHelper, buttonText, actionName, controllerName, routeValues, null);
        }

        public static MvcHtmlString ButtonIconized(this HtmlHelper htmlHelper,
            string buttonText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            return _ButtonIconized(htmlHelper, buttonText, actionName, controllerName, routeValues, htmlAttributes);
        }
    }
}