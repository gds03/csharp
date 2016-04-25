using System.Web.Mvc;

namespace VirtualNote.MVC.Attributes.ActionFilters.ModelState
{
    public abstract class ModelStateTempDataTransfer : ActionFilterAttribute
    {
        protected static readonly string Key = typeof(ModelStateTempDataTransfer).FullName;
    }
}