using NHibernateMVCApp.Repository.Interfaces;
using NHibernateMVCApp.Repository.Mappings;
using NHibernateMVCApp.Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHibernateMVCApp.Controllers
{
    public class InfrastructureSearchController : Controller
    {
        private readonly IRepository m_repository;


        public InfrastructureSearchController()
        {
            m_repository = new NHibernateMVCApp.Repository.Types.Repository();
        }


        // GET: InfrastructureSearch
        public ActionResult Index()
        {
            IList<Product> data = m_repository.Query<Product>().GetPriceHigherThan(1000);
            return View(data);
        }
    }
}