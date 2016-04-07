using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomComponents.Mvc.UserControls.Extensions
{

    public static class ViewContextExtensions
    {
        /// <summary>
        ///     Get the current URL with controllerName and actionName for the current HTTP Request.
        /// </summary>
        public static string GetCurrentUrl(this ViewContext viewContext,
            out string controllerName, out string actionName)
        {
            var rd = viewContext.RouteData;

            actionName = rd.Values["Action"].ToString();
            controllerName = rd.Values["Controller"].ToString();

            Controller controller = (Controller)viewContext.Controller;
            return controller.Url.Action(actionName, controllerName);
        }
    }
}