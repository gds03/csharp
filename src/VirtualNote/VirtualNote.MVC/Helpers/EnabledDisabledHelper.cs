using System.Web.Mvc;

namespace VirtualNote.MVC.Helpers
{
    public static class EnabledDisabledHelper
    {
        public static MvcHtmlString SetImageIfEnabled(this HtmlHelper html,
            bool enabled)
        {
            if (!enabled)
                return MvcHtmlString.Empty;

            return html.ImageFromImagesDirectory("accept.png", "Enabled");
        }
    }
}