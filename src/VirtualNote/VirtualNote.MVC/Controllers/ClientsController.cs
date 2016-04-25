using System.Web.Mvc;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Services;
using VirtualNote.MVC.Attributes.ActionFilters;
using VirtualNote.MVC.Attributes.ActionFilters.ModelState;
using VirtualNote.MVC.Attributes.Authorization;
using VirtualNote.MVC.Enums;

namespace VirtualNote.MVC.Controllers
{
    [Authorized(Roles = LoginService.Admin)]
    public class ClientsController : Controller
    {
        readonly IClientsService _clientsService;
        readonly IQueryService   _queryService;

        public ClientsController(IQueryService queryService, IClientsService clientsService)
        {
            _queryService = queryService;
            _clientsService = clientsService;
        }

       

        //
        // GET: /Clients/
        public ActionResult Index()
        {
            var data = _queryService.GetClients();
            return View(data);
        }


        //
        // GET: /Clients/Create
        public ActionResult Create()
        {
            return View("CreateUpdate");
        }

        //
        // POST: /Clients/Create
        [HttpPost]
        [DatabaseSaveChanges]
        public ActionResult Create(ClientServiceDTO clientDto)
        {
            // Verificar erros do modelo
            if (!ModelState.IsValid)
                return View("CreateUpdate", clientDto);

            // Verificar erros de negocio
            if (!_clientsService.Add(clientDto)){
                ModelState.AddModelError("Name", "This name belongs to another user");
                return View("CreateUpdate", clientDto);
            }

            // Sucesso
            ViewBag.Who = clientDto.Name;
            return View("CUD", ActionEnum.Created);
        }

        //
        // GET: /Clients/Details/5
        [ImportModelStateFromTempData]
        public ActionResult Details(int id)
        {
            var data = _queryService.GetClientDetails(id);
            return View(data);
        }


        //
        // GET: /Clients/Edit/5
        public ActionResult Edit(int id)
        {
            var client = _queryService.GetClientById(id);
            return View("CreateUpdate", client);
        }

        //
        // POST: /Clients/Edit/5
        [HttpPost]
        [DatabaseSaveChanges]
        public ActionResult Edit(int id, ClientServiceDTO clientDto) 
        {
            // Verificar erros do modelo
            if (!ModelState.IsValid)
                return View("CreateUpdate", clientDto);

            clientDto.UserID = id;

            // Verificar erros de negocio
            if (!_clientsService.Update(clientDto)) {
                ModelState.AddModelError("Name", "This name belongs to another user");
                return View("CreateUpdate", clientDto);
            }

            // Sucesso
            ViewBag.Who = clientDto.Name;
            return View("CUD", ActionEnum.Edited);
        }

        //
        // Get: /Clients/Delete/5
        [DatabaseSaveChanges]
        [ExportModelStateToTempData]
        public ActionResult Delete(int id){
            string clientName;

            // Verificar erros de negocio
            if(!_clientsService.Remove(id, out clientName)){
                ModelState.AddModelError(string.Empty, "This client contains projects, so cannot be removed, but was setted disabled");
                return RedirectToAction("Details", new { id });
            }

            // Sucesso
            ViewBag.Who = clientName;
            return View("CUD", ActionEnum.Deleted);
        }
    }
}
