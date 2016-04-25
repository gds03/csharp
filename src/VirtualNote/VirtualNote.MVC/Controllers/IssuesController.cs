using System;
using System.Web.Mvc;
using VirtualNote.Kernel;
using VirtualNote.Kernel.Configurations.StructureMap;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.Contracts.Issues;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Services;
using VirtualNote.MVC.Attributes.ActionFilters;
using VirtualNote.MVC.Attributes.ActionFilters.ModelState;
using VirtualNote.MVC.Attributes.Authorization;
using VirtualNote.MVC.Enums;

namespace VirtualNote.MVC.Controllers
{
    [Authorized(new [] { LoginService.Admin, LoginService.Member, LoginService.Client })]
    public class IssuesController : Controller
    {
        private const int DefaultTake = 7;


        readonly IQueryService _queryService;


        public IssuesController(IQueryService queryService)
        {
            _queryService = queryService;
        }


        //
        // GET: /Issue/
        public ActionResult Index(int currentPage = 1, int take = DefaultTake)   // OK
        { 
            if(User.IsInRole(LoginService.Admin)){
                return View("IndexMembers", _queryService.GetIssuesForAdmin(currentPage, take));
            }

            if(User.IsInRole(LoginService.Member)){
                return View("IndexMembers", _queryService.GetIssuesForMember(currentPage, take));
            }

            return View("IndexClients", _queryService.GetIssuesForClient(currentPage, take));
        }


        public PartialViewResult IndexPartial(int projectId, IssuesSortBy sortBy, int currentPage = 1, int take = DefaultTake)   // OK
        {
            if (User.IsInRole(LoginService.Admin)){
                var adminService = _queryService.GetIssuesForAdmin(currentPage, take, projectId, sortBy);
                return PartialView("_IndexMembersRequests", adminService.Requests);
            }

            if (User.IsInRole(LoginService.Member)){
                var memberService = _queryService.GetIssuesForMember(currentPage, take, projectId, sortBy);
                return PartialView("_IndexMembersRequests", memberService.Requests);
            }

            var clientService = _queryService.GetIssuesForClient(currentPage, take, projectId, sortBy);
            return PartialView("_IndexClientsRequests", clientService.Requests);
        } 



        //
        // GET: /Issue/Create
        [ImportModelStateFromTempData]
        public ActionResult Create()                  
        { 
            if (!User.IsInRole(LoginService.Client))
                return new RedirectResult("/");

            return View(_queryService.GetIssueForClient(0));
        }



        //
        // GET: /Issue/Edit/id
        [ImportModelStateFromTempData]
        public ActionResult Edit(int id)           
        {
            if (User.IsInRole(LoginService.Admin)) {
                return View("EditMembers", _queryService.GetIssueForAdmin(id));
            }


            if (User.IsInRole(LoginService.Member)) {
                return View("EditMembers", _queryService.GetIssueForMember(id));
            }

            return View("EditClients", _queryService.GetIssueForClient(id));
        }







        
        //
        // POST: /Issue/Create
        [HttpPost]
        [DatabaseSaveChanges]
        [ExportModelStateToTempData]
        public ActionResult Create(IssueServiceClientDTO clientDto) 
        {
            //
            // So clientes podem criar issues
            if (!User.IsInRole(LoginService.Client))
                return new RedirectResult("/");

            // Verificar erros no modelo
            if (!ModelState.IsValid)
                return RedirectToAction("Create");

            // Obter serviço cliente de factory
            IIssueClientService clientService = ObjectsManager.GetInstance<IIssueClientService>();

            try {
                clientService.Add(clientDto);
            }
            catch(ProjectDisabledException ex){
                ModelState.AddModelError(string.Empty, "This project is currently disabled. You cannot add a new issue");
                return RedirectToAction("Create");
            }

            // Sucesso
            ViewBag.Who = "New Issue";
            return View("CUD", ActionEnum.Created);
        }


        
        //
        // POST: /Issue/Edit/5
        [HttpPost]
        [DatabaseSaveChanges]
        [ExportModelStateToTempData]
        public ActionResult Edit(int id, FormCollection collection)
        {
            //
            // Não posso usar modelbinder devido a só depois de saber a role à
            // qual estou associado posso construir o objecto DTO

            if (User.IsInRole(LoginService.Admin)) 
            {
                //
                // Criar objecto membro dto com os dados do Form
                IssueServiceMemberDTO memberDto = new IssueServiceMemberDTO();
                UpdateModel(memberDto);
                memberDto.IssueId = id;

                // Obter e invocar changeState no serviço
                IIssueAdminService adminService = ObjectsManager.GetInstance<IIssueAdminService>();
                
                // Verificar erros de negocio
                if (!adminService.ChangeState(memberDto)) 
                {
                    // Se estamos aqui então o estado do issue está terminado e nao pode ser alterado
                    ModelState.AddModelError(string.Empty, "This issue is already terminated");
                    return RedirectToAction("Edit", new { id });
                }
            }


            if (User.IsInRole(LoginService.Member)) 
            {
                //
                // Criar objecto dto com os dados do Form
                IssueServiceMemberDTO memberDto = new IssueServiceMemberDTO();
                UpdateModel(memberDto);
                memberDto.IssueId = id;

                // Obter e invocar changeState no serviço
                IIssueMemberService memberService = ObjectsManager.GetInstance<IIssueMemberService>();

                // Verificar erros de negocio
                try {
                    if(!memberService.ChangeState(memberDto)){
                        // Se estamos aqui então o estado do issue está terminado e nao pode ser alterado
                        ModelState.AddModelError(string.Empty, "This issue is already terminated");
                        return RedirectToAction("Edit", new { id });
                    }
                }
                catch (IssueWasAlreadyTakedByAnotherMember ex) {
                    // Se estamos aqui então o issue ja foi aceite por outro membro
                    ModelState.AddModelError(string.Empty, string.Format("This issue is in resolution by {0}", ex.Message));
                    return RedirectToAction("Edit", new { id });
                }
            }

            if (User.IsInRole(LoginService.Client)) {
                //
                // Criar objecto dto com os dados do Form
                IssueServiceClientDTO clientDto = new IssueServiceClientDTO();
                if (!TryUpdateModel(clientDto)){

                    // Verificar erros no modelo
                    return RedirectToAction("Edit", new { id });
                }

                clientDto.IssueId = id;


                // Obter e invocar update no serviço
                IIssueClientService clientService = ObjectsManager.GetInstance<IIssueClientService>();

                try {
                    // Verificar erros de negocio
                    if (!clientService.Update(clientDto)) {
                        // Se estamos aqui então o estado do issue está terminado e nao pode ser alterado
                        ModelState.AddModelError(string.Empty, "This issue is already terminated");
                        return RedirectToAction("Edit", new { id });
                    }
                }
                catch (ProjectDisabledException ex) {
                    ModelState.AddModelError(string.Empty, "This project is currently disabled. You cannot update the issue");
                    return RedirectToAction("Edit", new { id });
                }
            }

            // Sucesso
            ViewBag.Who = "Existing Issue";
            return View("CUD", ActionEnum.Edited);
        }



        //
        // GET: /Issues/Delete/5
        [DatabaseSaveChanges]
        [ExportModelStateToTempData]
        public ActionResult Delete(int id)
        {
            if(User.IsInRole(LoginService.Admin)){
                var adminService = ObjectsManager.GetInstance<IIssueAdminService>();

                adminService.Remove(id);

                // Sucesso
                ViewBag.Who = "Issue";
                return View("CUD", ActionEnum.Deleted);
            }

            if(User.IsInRole(LoginService.Client)){
                var clientService = ObjectsManager.GetInstance<IIssueClientService>();

                try {
                    if (!clientService.Remove(id)) {
                        ModelState.AddModelError(string.Empty, "Issue is not in Waiting State, so cannot be removed");
                        return RedirectToAction("Edit", new { id });
                    }
                }
                catch (ProjectDisabledException ex) {
                    ModelState.AddModelError(string.Empty, "This project is currently disabled. You cannot delete the issue");
                    return RedirectToAction("Edit", new { id });
                }

                return View("CUD", ActionEnum.Deleted);
            }
            throw new InvalidOperationException();
        }
    }
}
