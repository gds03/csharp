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
    public class TestCommentsClientService
    {
        ICommentsClientService _service;



        [TestInitialize]
        public void Init() {
            _service = new CommentClientService(new MemoryRepository());
        }



        // 
        // Insert
        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void ClientCommentAddCommentWithConflict(){
            new TemporaryPrincipal("gdias");

            _service.Add(
                new CommentServiceDto {
                    IssueID = 1,
                    Description = "TESTE add"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void ClientCommentAddCommentWithConflict2() {
            new TemporaryPrincipal("shanselman");

            _service.Add(
                new CommentServiceDto {
                    IssueID = 1,
                    Description = "TESTE add"
                });
        }

        [TestMethod]
        [ExpectedException(typeof(HijackedException))]
        public void ClientCommentAddCommentWithConflict3() {
            new TemporaryPrincipal("vobis");    // Vobis nao tem o projecto a que o id 1 esta associado

            _service.Add(
                new CommentServiceDto {
                    IssueID = 1,
                    Description = "TESTE add"
                });
        }

        [TestMethod]
        public void ClientCommentAddCommentWithout() {
            new TemporaryPrincipal("zonlusomundo");

            _service.Add(
                new CommentServiceDto {
                    IssueID = 1,
                    Description = "TESTE add"
                });

            // Testar se inseriu no repositorio
            Assert.IsNotNull(_service.Repository.Query<Comment>().Single(c => c.Description == "TESTE add"));
        }

        
       

        //
        // Delete
        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void ClientCommentDeleteCommentWithConflict() {
            new TemporaryPrincipal("gdias");

            _service.Remove(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void ClientCommentDeleteCommentWithConflict2() {
            new TemporaryPrincipal("shanselman");

            _service.Remove(1);
        }

        [TestMethod]
        public void ClientCommentDeleteCommentWithConflict3() {
            new TemporaryPrincipal("vobis");    // Vobis nao tem o projecto a que o id 1 esta associado

            bool result = _service.Remove(1);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ClientCommentDeleteCommentWithoutConflict() {
            new TemporaryPrincipal("zonlusomundo");

            _service.Add(
                new CommentServiceDto {
                    IssueID = 1,
                    Description = "TESTE add"
                });

            // Obter id do comentario
            int commentId = _service.Repository.Query<Comment>().Single(c => c.Description == "TESTE add").CommentID;

            bool result = _service.Remove(commentId);

            Assert.IsTrue(result);

            // Testar se removeu no repositorio
            Assert.IsNull(_service.Repository.Query<Comment>().SingleOrDefault(c => c.CommentID == commentId));
        }
    }
}
