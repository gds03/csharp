using System.Web.Mvc;
using VirtualNote.Common.ExtensionMethods;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Services;
using VirtualNote.MVC.Attributes.ActionFilters;
using VirtualNote.MVC.Attributes.ActionFilters.ModelState;
using VirtualNote.MVC.Attributes.Authorization;
using VirtualNote.MVC.Enums;
using VirtualNote.MVC.Models;

namespace VirtualNote.MVC.Controllers
{
    [Authorized(Roles = LoginService.Admin)]
    public class MembersController : Controller
    {
        readonly IMembersService _membersService;
        readonly IQueryService _queryService;

        public MembersController(IQueryService queryService, IMembersService membersService)
        {
            _queryService = queryService;
            _membersService = membersService;
        }


        //
        // GET: /Members/
        public ActionResult Index()
        {
            var data = _queryService.GetMembers();
            return View(data);
        }


        //
        // GET: /Members/Create
        public ActionResult Create()
        {
            return View("CreateUpdate");
        }

        //
        // POST: /Members/Create
        [HttpPost]
        [DatabaseSaveChanges]
        public ActionResult Create(MemberServiceDTO memberDto)
        {
            // Verificar erros no modelo
            if (!ModelState.IsValid)
                return View("CreateUpdate", memberDto);

            // Verificar erros de negocio
            if (!_membersService.Add(memberDto))
            {
                ModelState.AddModelError("Name", "This name belongs to another user");
                return View("CreateUpdate", memberDto);
            }

            // Sucesso
            ViewBag.Who = memberDto.Name;
            return View("CUD", ActionEnum.Created);
        }

        //
        // GET: /Members/Details/5
        [ImportModelStateFromTempData]
        public ActionResult Details(int id)
        {
            var data = _queryService.GetMemberDetails(id);
            return View("Details", data);
        }


        //
        // GET: /Members/Edit/5
        public ActionResult Edit(int id)
        {
            var data = _queryService.GetMemberById(id);
            return View("CreateUpdate", data);
        }

        //
        // POST: /Members/Edit/5
        [HttpPost]
        [DatabaseSaveChanges]
        public ActionResult Edit(int id, MemberServiceDTO memberDto)
        {
            // Verificar erros no modelo
            if (!ModelState.IsValid)
                return View("CreateUpdate", memberDto);

            memberDto.UserID = id;

            // Verificar erros de negocio
            if (!_membersService.Update(memberDto))
            {
                ModelState.AddModelError("Name", "This name belongs to another user");
                return View("CreateUpdate", memberDto);
            }

            // Sucesso
            ViewBag.Who = memberDto.Name;
            return View("CUD", ActionEnum.Edited);
        }

        //
        // GET: /Members/Delete/5
        [DatabaseSaveChanges]
        [ExportModelStateToTempData]
        public ActionResult Delete(int id){
            string memberName;
            
            // Verificar erros de negocio
            try{
                if (!_membersService.Remove(id, out memberName)) {
                    ModelState.AddModelError(string.Empty, "This member is assigned to projects, so cannot be removed, but was setted disabled");
                    return RedirectToAction("Details", new { id });
                }
            }
            catch (MemberLastAdminException) {
                ModelState.AddModelError(string.Empty, "This member is the last admin in the system, so cannot be removed");
                return RedirectToAction("Details", new { id });
            }

            // Sucesso
            ViewBag.Who = memberName;
            return View("CUD", ActionEnum.Deleted);
        }
    }
}
