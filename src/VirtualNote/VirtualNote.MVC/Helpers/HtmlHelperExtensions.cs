using System;
using System.Web.Mvc;
using VirtualNote.Common.ExtensionMethods;
using VirtualNote.Kernel.Configurations.StructureMap;
using VirtualNote.Kernel.Services;

namespace VirtualNote.MVC.Helpers
{
    public static class HtmlHelperExtensions
    {
        internal static String RouteDataValueFromKey(HtmlHelper helper, string key)
        {
            return helper.ViewContext.Controller.ValueProvider.GetValue(key).RawValue.ToString();
        }




        public static String GetControllerName(this HtmlHelper helper)
        {
            return RouteDataValueFromKey(helper, "controller").UpFirstLetter();
        }

        public static String GetActionName(this HtmlHelper helper)
        {
            return RouteDataValueFromKey(helper, "action").UpFirstLetter();
        }

        public static bool IsEdit(this HtmlHelper helper) {
            return GetActionName(helper).ToLower() == "edit";
        }

        public static bool IsCreate(this HtmlHelper helper) {
            return GetActionName(helper).ToLower() == "create";
        }


        public static String GetUsername(this HtmlHelper helper)
        {
            return helper.ViewContext.HttpContext.User.Identity.Name;
        }

        public static String GetUsertype(this HtmlHelper helper){
            return ObjectsManager.GetInstance<LoginService>().GetRolesForUser(GetUsername(helper))[0];
        }

        public static UrlHelper GetUrlHelper(this HtmlHelper html){
            return ( (Controller) html.ViewContext.Controller ).Url;
        }
    }
}