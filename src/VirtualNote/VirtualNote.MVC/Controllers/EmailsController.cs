using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VirtualNote.Common.ExtensionMethods;
using VirtualNote.Kernel.Configurations;
using VirtualNote.Kernel.Configurations.StructureMap;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Emails;
using VirtualNote.Kernel.Services;
using VirtualNote.MVC.Attributes.Authorization;
using VirtualNote.MVC.Enums;

namespace VirtualNote.MVC.Controllers
{
    [Authorized]
    public class EmailsController : Controller
    {
        readonly IQueryService _queryService;


        public EmailsController(IQueryService queryService)
        {
            _queryService = queryService;
        }


        //
        // GET: /Emails/

        public ActionResult Index()
        {
            var data = _queryService.GetEmailConfigsFor();

            if (User.IsInRole(LoginService.Client)) {
                return View("IndexClients", data);
            }

            return View("IndexMembers", data);
        }

        static IEnumerable<EmailConfig> GetConfigsFromFormCollection(FormCollection collection){
            return collection.AllKeys.Select(k => (EmailConfig)Enum.Parse(typeof(EmailConfig), k)).ToList();
        }

        [HttpPost]
        public ActionResult Index(FormCollection collection){
            var data = GetConfigsFromFormCollection(collection);

            if(User.IsInRole(LoginService.Admin)){
                IEmailAdminService adminService = ObjectsManager.GetInstance<IEmailAdminService>();
                adminService.SetConfigurations(data);
            }

            else{
                if(User.IsInRole(LoginService.Member)){
                    IEmailMemberService memberService = ObjectsManager.GetInstance<IEmailMemberService>();
                    memberService.SetConfigurations(data);
                }else{
                    IEmailClientService clientService = ObjectsManager.GetInstance<IEmailClientService>();
                    clientService.SetConfigurations(data);
                }
            }

            ViewBag.Who = "Email Configuration";
            return View("CUD", ActionEnum.Edited);
        }

    }
}
