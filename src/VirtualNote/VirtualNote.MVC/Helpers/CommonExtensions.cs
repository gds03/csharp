using System;
using System.Web.Mvc;

namespace VirtualNote.MVC.Helpers
{
    public static class CommonExtensions
    {
        // Usado por button Extensions
        public static String AnchorWithInnerHtml(string url, string innerHtml)
        {
            TagBuilder linkBuilder = new TagBuilder("a");

            linkBuilder.MergeAttribute("href", url);
            linkBuilder.InnerHtml = innerHtml;

            return linkBuilder.ToString();
        }
    }
}