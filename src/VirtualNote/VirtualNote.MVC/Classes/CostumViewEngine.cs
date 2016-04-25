using System.Linq;
using System.Web.Mvc;

namespace VirtualNote.MVC.Classes
{
    sealed class CostumViewEngine : RazorViewEngine
    {
        static readonly string[] NewPartialViewFormats = new[] { 
                "~/Views/{1}/Partials/{0}.cshtml",
                "~/Views/Shared/Partials/{0}.cshtml"
            };
        // {1} - is the controller, {0} is the action

        public CostumViewEngine()
        {
            PartialViewLocationFormats = PartialViewLocationFormats.Union(NewPartialViewFormats).ToArray();
        }
    }
}