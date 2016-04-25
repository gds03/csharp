using System.Linq;
using System.Web.Mvc;
using VirtualNote.Common.ExtensionMethods;
using VirtualNote.Kernel.Contracts;
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
    public class ProjectsController : Controller
    {
        readonly IProjectsService _projectsService;
        readonly IQueryService _queryService;

        public ProjectsController(IProjectsService projectsService, IQueryService queryService)
        {
            _projectsService = projectsService;
            _queryService = queryService;
        }


        //
        // GET: /Projects/
        public ActionResult Index()
        {
            var data = _queryService.GetProjects();
            return View(data);
        }

        //
        // GET: /Projects/Details/5
        [ImportModelStateFromTempData]
        public ActionResult Details(int id)
        {
            var data = _queryService.GetProjectDetails(id);
            return View(data);
        }


        //
        // GET: /Projects/Create
        public ActionResult Create()
        {
            var data = _queryService.GetProjectById(0);
            return View(data);
        }

        [HttpPost]
        [DatabaseSaveChanges]
        public ActionResult Create(ProjectServiceDTO projectDto)
        {
            // Verificar erros no modelo
            if (!ModelState.IsValid)
                return View(projectDto);

            // Verificar erros de negocio
            if (!_projectsService.Add(projectDto))
            {
                ModelState.AddModelError("Name", "This name belongs to another project");
                return View(projectDto);
            }

            // Sucesso
            ViewBag.Who = projectDto.Name;
            return View("CUD", ActionEnum.Created);
        }
        



        //
        // GET: /Projects/Edit/5
        public ActionResult Edit(int id)
        {
            var data = _queryService.GetProjectById(id);
            return View(data);
        }

        //
        // POST: /Projects/Edit/5
        [HttpPost]
        [DatabaseSaveChanges]
        public ActionResult Edit(int id, ProjectServiceDTO projectDto)
        {
            // Verificar erros no modelo
            if (!ModelState.IsValid)
                return View(projectDto);

            projectDto.ProjectID = id;

            // Verificar erros no negocio
            if (!_projectsService.Update(projectDto))
            {
                ModelState.AddModelError("Name", "This name belongs to another project");
                return View(projectDto);
            }

            // Sucesso
            ViewBag.Who = projectDto.Name;
            return View("CUD", ActionEnum.Edited);
        }


        //
        // Get: /Projects/Assign/5
        public ActionResult Assign(int id)
        {
            var data = _queryService.GetProjectWorkers(id);
            return View(data);
        }


        

        //
        // Post: /Projects/Assign/5
        [HttpPost]
        [DatabaseSaveChanges]
        public ActionResult Assign(int id, FormCollection collection){
            string projectName;

            ProjectServiceAssignWorkersDTO createAssignDto = CreateAssignDto(id, collection);
            _projectsService.Assign(createAssignDto, out projectName);

            // Sucesso
            ViewBag.Who = projectName;
            return View("CUD", ActionEnum.Edited);
        }


        static ProjectServiceAssignWorkersDTO CreateAssignDto(int id, FormCollection collection) {
            return new ProjectServiceAssignWorkersDTO {
                workersIds = collection.AllKeys.Select(k => collection[k].ToInt()),
                ProjectId = id
            };
        }



        //
        // GET: /Projects/Delete/5
        [DatabaseSaveChanges]
        [ExportModelStateToTempData]
        public ActionResult Delete(int id) {
            string projectName;

            // Verificar erros de negocio
            if (!_projectsService.Remove(id, out projectName)) {
                ModelState.AddModelError(string.Empty, "This project contains issues, so cannot be removed, but was setted disabled");
                return RedirectToAction("Details", new { id });
            }

            // Sucesso
            ViewBag.Who = projectName;
            return View("CUD", ActionEnum.Deleted);
        }

    }
}
