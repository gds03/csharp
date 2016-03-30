using NHibernateMVCApp.Repository.Types.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NHibernateMVCApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public override void Init()
        {
            this.BeginRequest += (o, e) =>
            {
                ISessionFactoryWrapper.BeginRequest();
            };

            this.EndRequest += (o, e)=>
            {
                ISessionFactoryWrapper.EndRequest();
            };
            base.Init();
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ISessionFactoryWrapper.Init("connString");
            
        }
    }
}
