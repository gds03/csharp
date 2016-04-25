using System.Web.Mvc;

namespace VirtualNote.MVC.Extensions
{
    public static class MvcHtmlStringExtensions
    {
        public static MvcHtmlString CheckForEmpty(this MvcHtmlString str)
        {
            if (!string.IsNullOrEmpty(str.ToHtmlString()))
                return str;

            var spanBuilder = new TagBuilder("span");
            spanBuilder.SetInnerText("None");
            return MvcHtmlString.Create(spanBuilder.ToString());
        }
    }
}