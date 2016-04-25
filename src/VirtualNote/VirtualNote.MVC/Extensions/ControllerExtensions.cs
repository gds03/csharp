using System.IO;
using System.Web.Mvc;

namespace VirtualNote.MVC.Extensions
{
    public static class ControllerExtensions
    {

        public static string RenderPartialViewToString(this Controller controller) 
        {
            return RenderPartialViewToString(controller, null, null);
        }

        public static string RenderPartialViewToString(this Controller controller, 
            string viewName) 
        {
            return RenderPartialViewToString(controller, viewName, null);
        }

        public static string RenderPartialViewToString(this Controller controller, 
            object model)
        {
            return RenderPartialViewToString(controller, null, model);
        }

        public static string RenderPartialViewToString(this Controller controller, string viewName, object model) {
            if (string.IsNullOrEmpty(viewName))
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");

            controller.ViewData.Model = model;

            using (StringWriter sw = new StringWriter()) {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}