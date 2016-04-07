using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomComponents.Mvc.UserControls.Types.ViewEngine
{
    /// <summary>
    ///     Engines that knows where to search for user controls.
    /// </summary>
    public class UserControlsViewEngine : RazorViewEngine
    {
        // ctor
        public UserControlsViewEngine()
        {
            string[] newLocationFormat = new string[]
            {
                "~/bin/Views/Shared/{0}.cshtml"
            };

            PartialViewLocationFormats = base.PartialViewLocationFormats.Union(newLocationFormat).ToArray();
            ViewLocationFormats = PartialViewLocationFormats;
        }
    }
}