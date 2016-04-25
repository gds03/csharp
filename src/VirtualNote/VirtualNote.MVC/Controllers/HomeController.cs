using System.Web.Mvc;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Services;
using VirtualNote.MVC.Attributes.Authorization;

namespace VirtualNote.MVC.Controllers
{
    [Authorized(new[] { LoginService.Admin, LoginService.Member, LoginService.Client })]
    public class HomeController : Controller
    {
        readonly IQueryService _queryService;


        public HomeController(IQueryService queryService){
            _queryService = queryService;
        }


        //
        // GET: /Home/
        public ActionResult Index()
        {
            if(User.IsInRole(LoginService.Admin)){
                var data = _queryService.GetHomeFeedForAdmin();
                return View("IndexAdmins", data);
            }

            if (User.IsInRole(LoginService.Member)) {
                var data = _queryService.GetHomeFeedForMember();
                return View("IndexMembers", data);
            }

            if (User.IsInRole(LoginService.Client)) {
                var data = _queryService.GetHomeFeedForClient();
                return View("IndexClients", data);
            }
            return new HttpUnauthorizedResult();
        }

    }
}
