using System.Web.Mvc;
using System.Web.Routing;
using VirtualNote.Kernel.Configurations;
using VirtualNote.Kernel.Configurations.StructureMap;
using VirtualNote.MVC.Classes;

namespace VirtualNote.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //
            // Comments Routes
            routes.MapRoute(
                "Comments2",
                "Issues/{IssueId}/Comments/IndexPaging/{page}",
                new { controller = "Comments", action = "IndexPaging" }
            );

            routes.MapRoute(
                "Comments",
                "Issues/{IssueId}/Comments/{action}/{id}",
                new { controller = "Comments", action = "Index", id = UrlParameter.Optional }
            );

            //
            // Configuration Routes
            routes.MapRoute(
                "Members",
                "Configurations/Members/{action}/{id}",
                new { controller = "Members", action = "Index", id = ""}
            );

            routes.MapRoute(
                "Clients",
                "Configurations/Clients/{action}/{id}",
                new { controller = "Clients", action = "Index", id = "" }
            );

            routes.MapRoute(
                "Projects",
                "Configurations/Projects/{action}/{id}",
                new { controller = "Projects", action = "Index", id = "" }
            );




            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );


            

        }

        protected void Application_Start()
        {
            ViewEngines.Engines.Add(new CostumViewEngine());
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);


            ObjectsManager.Initialize();
            ControllerBuilder.Current.SetControllerFactory(new VnControllerFactory());

            VnDatabaseConfiguration.Initialize();
        }
    }
}