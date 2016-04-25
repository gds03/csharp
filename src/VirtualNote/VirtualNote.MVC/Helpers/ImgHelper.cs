using System.Web.Mvc;
using System.Web.Routing;

namespace VirtualNote.MVC.Helpers
{
    public static class ImgHelper
    {
        public static MvcHtmlString ImageFromImagesDirectory(this HtmlHelper helper,
                                   string imageNameWithExtension,
                                   string alt,
                                   object htmlAttributes) {
            TagBuilder builder = new TagBuilder("img");
            UrlHelper urlHelper = ((Controller)helper.ViewContext.Controller).Url;

            builder.MergeAttribute("src", urlHelper.Content(string.Format("~/Content/images/{0}", imageNameWithExtension)));
            builder.MergeAttribute("alt", alt);
            builder.MergeAttributes(new RouteValueDictionary(htmlAttributes), true);

            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString ImageFromImagesDirectory(this HtmlHelper helper,
                                   string imageNameWithExtension,
                                   string alt) 
        {
            return ImageFromImagesDirectory(helper, imageNameWithExtension, alt, null);

        }
    }
}