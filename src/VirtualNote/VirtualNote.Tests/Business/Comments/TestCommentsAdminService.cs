using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts.Comments;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Services.Comments;

namespace VirtualNote.Tests.Business.Comments
{
    [TestClass]
    public class TestCommentsAdminService
    {
        private ICommentsAdminService _service;

        [TestInitialize]
        public void Init() 
        {
            _service = new CommentAdminService(new MemoryRepository());
        }



        // 
        // Insert
        [TestMethod]
        public void AdminCommentAddCommentWithoutConflict()
        {
            //
            // Admin consegue inserir um comentario em qualquer issue
            // 
            new TemporaryPrincipal("gdias");

            _service.Add(
                new CommentServiceDto
                {
                    IssueID = 1, // Issue do projecto zon
                    Description = "Teste sobre comentarios"
                });

            // Verificar que inseriu no repositorio
            Assert.IsNotNull(_service.Repository.Query<Comment>().Single(c => c.Description == "Teste sobre comentarios"));
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void AdminCommentAddCommentWithConflictRoles()
        {
            //
            // shanselman nao é admin
            // 
            new TemporaryPrincipal("shanselman");

            _service.Add(               // Excepcao
                new CommentServiceDto {
                    IssueID = 1,    // Issue do projecto zon
                    Description = "Teste sobre comentarios"
                });
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void AdminCommentAddCommentWithConflictRoles2() 
        {
            //
            // zonlusomundo nao é admin
            // 
            new TemporaryPrincipal("zonlusomundo");

            _service.Add(                      // Excepcao 
                new CommentServiceDto {
                    IssueID = 1,    // Issue do projecto zon
                    Description = "Teste sobre comentarios"
                });
        }




       


        //
        // Delete
        [TestMethod]
        public void AdminCommentDeleteCommentWithout() 
        {
            //
            // gdias e admin e remove comentario
            // 
            new TemporaryPrincipal("gdias");

            _service.Remove(1);

            // Verificar que apagou no repositorio
            Assert.IsNull(_service.Repository.Query<Comment>().SingleOrDefault(c => c.CommentID == 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void AdminCommentDeleteCommentWithConflicts() 
        {
            //
            // shanselman n e admin e n remove comentario
            // 
            new TemporaryPrincipal("shanselman");

            _service.Remove(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void AdminCommentDeleteCommentWithConflicts2()
        {
            //
            // zonlusomundo n e admin e n remove comentario
            // 
            new TemporaryPrincipal("zonlusomundo");

            _service.Remove(1);
        }
    }
}
