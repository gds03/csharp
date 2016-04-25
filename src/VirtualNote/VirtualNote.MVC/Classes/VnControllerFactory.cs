using System;
using System.Web.Mvc;
using VirtualNote.Kernel.Configurations.StructureMap;

namespace VirtualNote.MVC.Classes
{
    public sealed class VnControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                return null;

            return (Controller)ObjectsManager.GetInstance(controllerType);
        }
    }
}