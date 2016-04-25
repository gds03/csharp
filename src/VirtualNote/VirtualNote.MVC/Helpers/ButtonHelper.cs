using System.Web.Mvc;
using System.Web.Routing;

namespace VirtualNote.MVC.Helpers
{

    public static class ButtonHelper
    {
        public static MvcHtmlString Button(this HtmlHelper htmlHelper,
           string buttonText, string actionName, string controllerName, object routeValues, object htmlAttributes) 
        {
            string url = htmlHelper.GetUrlFrom(actionName, controllerName, routeValues);

            TagBuilder buttonBuilder = new TagBuilder("button");
            buttonBuilder.MergeAttribute("type", "button");
            buttonBuilder.MergeAttribute("value", buttonText);
            buttonBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes), true);

            return MvcHtmlString.Create(CommonExtensions.AnchorWithInnerHtml(url, buttonBuilder.ToString(TagRenderMode.SelfClosing)));
        }


        public static MvcHtmlString Button(this HtmlHelper htmlHelper,
            string buttonText, string actionName)
        {
            return Button(htmlHelper, buttonText, actionName, null, null, null);
        }

        public static MvcHtmlString Button(this HtmlHelper htmlHelper,
            string buttonText, string actionName, string controllerName)
        {
            return Button(htmlHelper, buttonText, actionName, controllerName, null, null);
        }

        public static MvcHtmlString Button(this HtmlHelper htmlHelper,
            string buttonText, string actionName, object routeValues)
        {
            return Button(htmlHelper, buttonText, actionName, null, routeValues, null);
        }

        public static MvcHtmlString Button(this HtmlHelper htmlHelper,
            string buttonText, string actionName, string controllerName, object routeValues)
        {
            return Button(htmlHelper, buttonText, actionName, controllerName, routeValues, null);
        }

       
    }
}