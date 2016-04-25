using System.Web.Mvc;
using System.Web.Routing;

namespace VirtualNote.MVC.Helpers
{
    public static class ImageLinkHelper
    {
        public static MvcHtmlString ImageLinkFromImagesDirectory(this HtmlHelper htmlHelper,
            string imageNameWithExtension, string alt, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            UrlHelper urlHelper = ((Controller)htmlHelper.ViewContext.Controller).Url;
            string url = urlHelper.Action(actionName, controllerName, routeValues);

            TagBuilder imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", urlHelper.Content(string.Format("~/Content/images/{0}", imageNameWithExtension)));
            imgBuilder.MergeAttribute("alt", alt);
            imgBuilder.MergeAttributes(new RouteValueDictionary(htmlAttributes), true);

            return MvcHtmlString.Create(CommonExtensions.AnchorWithInnerHtml(url, imgBuilder.ToString(TagRenderMode.SelfClosing)));
        }



        public static MvcHtmlString ImageLinkFromImagesDirectory(this HtmlHelper htmlHelper,
           string imageNameWithExtension, string alt, string actionName) {
            return ImageLinkFromImagesDirectory(htmlHelper, imageNameWithExtension, alt, actionName, null, null, null);
        }

        public static MvcHtmlString ImageLinkFromImagesDirectory(this HtmlHelper htmlHelper,
           string imageNameWithExtension, string alt, string actionName, string controllerName) {
            return ImageLinkFromImagesDirectory(htmlHelper, imageNameWithExtension, alt, actionName, controllerName, null, null);
        }

        public static MvcHtmlString ImageLinkFromImagesDirectory(this HtmlHelper htmlHelper,
           string imageNameWithExtension, string alt, string actionName, object routeValues) {
            return ImageLinkFromImagesDirectory(htmlHelper, imageNameWithExtension, alt, actionName, null, routeValues, null);
        }

        public static MvcHtmlString ImageLinkFromImagesDirectory(this HtmlHelper htmlHelper,
           string imageNameWithExtension, string alt, string actionName, string controllerName, object routeValues) {
            return ImageLinkFromImagesDirectory(htmlHelper, imageNameWithExtension, alt, actionName, controllerName, routeValues, null);
        }

    }
}