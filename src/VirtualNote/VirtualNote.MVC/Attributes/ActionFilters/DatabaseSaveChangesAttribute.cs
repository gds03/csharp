using System.Web.Mvc;
using VirtualNote.Kernel.Managers;

namespace VirtualNote.MVC.Attributes.ActionFilters
{
    public class DatabaseSaveChangesAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            TransactionManager.Commit();
            TransactionManager.Dispose();
        }
    }
}