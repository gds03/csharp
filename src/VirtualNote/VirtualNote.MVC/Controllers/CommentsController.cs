using System;
using System.Web.Mvc;
using VirtualNote.Kernel.Configurations;
using VirtualNote.Kernel.Configurations.StructureMap;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Comments;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Services;
using VirtualNote.MVC.Attributes;
using VirtualNote.MVC.Attributes.Authorization;
using VirtualNote.MVC.Extensions;


namespace VirtualNote.MVC.Controllers
{
    [Authorized(new[] { LoginService.Admin, LoginService.Member, LoginService.Client })]
    public class CommentsController : Controller
    {
        readonly IQueryService _queryService;
        
        public CommentsController(IQueryService queryService)
        {
            _queryService = queryService;
        }


        String RenderViewToString(int issueId, int page = 1, int take = 5) {
            var data = _queryService.GetComments(page, take, issueId);
            return this.RenderPartialViewToString("_ListPartial", data.Data);   // Retorna uma vista parcial
        }



        //
        // GET: /Issue/{issueId}/Comments
        public ActionResult Index(int issueId, int take = 5)
        {
            var data = _queryService.GetComments(1, take, issueId);     // Explicitamente pede-se dados a partir da pagina 1
            return View(data);      // Retorna uma vista completa
        }



        // => Tipicamente usado na paginação..
        // GET: /Issue/{issueId}/Comments/IndexPaging
        public PartialViewResult IndexPaging(int issueId, int page = 1, int take = 5){
            var data = _queryService.GetComments(page, take, issueId);
            return PartialView("_ListPartial", data.Data); // Retorna uma vista parcial
        }

      
        // 
        // GET: /Issue/{issueId}/Comments/Create?description=...
        public ActionResult Create(int issueId, CommentServiceDto commentDto){
            if (!ModelState.IsValid)
                return Json(new { invalid = true });

            // É valido, atribui issueId
            commentDto.IssueID = issueId;

            // Saber qual o serviço a invocar
            if (User.IsInRole(LoginService.Admin)){
                var adminService = ObjectsManager.GetInstance<ICommentsAdminService>();
                adminService.Add(commentDto); // Internamente invoca SaveChanges
            } else{
                if (User.IsInRole(LoginService.Member)){
                    var memberService = ObjectsManager.GetInstance<ICommentsMemberService>();
                    memberService.Add(commentDto); // Internamente invoca SaveChanges
                } else{
                    var clientService = ObjectsManager.GetInstance<ICommentsClientService>();
                    clientService.Add(commentDto); // Internamente invoca SaveChanges
                }
            }
            return IndexPaging(issueId);
        }




        //
        // GET: /Issue/{issueId}/Comments/Remove/{id}
        public ActionResult Remove(int issueId, int id)
        {
            if(User.IsInRole(LoginService.Admin)){
                var adminService = ObjectsManager.GetInstance<ICommentsAdminService>();
                adminService.Remove(id);                        // Internamente invoca SaveChanges

                // Return result
                return Json(new { Success = true, ListItems = RenderViewToString(issueId) });
            }

            bool result;
            if(User.IsInRole(LoginService.Member)){
                var memberService = ObjectsManager.GetInstance<ICommentsMemberService>();
                result = memberService.Remove(id);         // Internamente invoca SaveChanges

                // Return result
                if(!result){
                    return Json(new { Success = false });
                }
                return Json(new { Success = true, ListItems = RenderViewToString(issueId) });
            }

            var clientService = ObjectsManager.GetInstance<ICommentsClientService>();
            result = clientService.Remove(id);        // Internamente invoca SaveChanges

            // Return result
            if (!result) {
                return Json(new { Success = false });
            }
            return Json(new { Success = true, ListItems = RenderViewToString(issueId) });
        }
    }
}
