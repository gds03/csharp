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
    public class TestCommentsMemberService
    {
        private ICommentsMemberService _service;

        [TestInitialize]
        public void Init() {
            _service = new CommentMemberService(new MemoryRepository());
        }



        // 
        // Insert
        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void MemberCommentAddCommentWithConflict() 
        {
            //
            //  Inserir um comentario de um admin causa conflicto
            // 
            new TemporaryPrincipal("gdias");    // nao e so membro 

            _service.Add(                       // Excepcao
                new CommentServiceDto {
                    IssueID = 1, // Issue do projecto zon
                    Description = "Teste sobre comentarios"
                });
        }


        [TestMethod]
        [ExpectedException(typeof(HijackedException))]
        public void MemberCommentAddCommentWithConflict2() 
        {
            //
            //  Inserir um comentario de um membro nao associado ao projecto do issue causa conflicto
            // 
            new TemporaryPrincipal("shanselman");   // Nao esta associado ao projecto deste issue

            _service.Add(
                new CommentServiceDto {
                    IssueID = 1,    // Issue do projecto zon
                    Description = "Teste sobre comentarios"
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void MemberCommentAddCommentWithConflict3() 
        {
            //
            //  Inserir um comentario atravez de um comentario causa conflicto
            // 
            new TemporaryPrincipal("zonlusomundo"); // nao e membro

            _service.Add(                                       // Excepcao
                new CommentServiceDto {
                    IssueID = 1,    // Issue do projecto zon
                    Description = "Teste sobre comentarios"
                });
        }

        [TestMethod]
        public void MemberCommentAddCommentWithoutConflict() 
        {
            //
            //  Inserir um comentario de um membro associado ao projecto do issue nao causa conflito
            // 
            new TemporaryPrincipal("psilva"); // esta associado

            _service.Add(
                new CommentServiceDto {
                    IssueID = 1,    // Issue do projecto zon
                    Description = "Teste sobre comentarios"
                });

            // Verificar que inseriu no repositorio
            Assert.IsNotNull(_service.Repository.Query<Comment>().Single(c => c.Description == "Teste sobre comentarios"));
        }






        //
        // Delete
        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void MemberCommentDeleteCommentWithConflict() 
        {
            //
            // Apagar atravez do admin causa conflicto
            // 
            new TemporaryPrincipal("gdias");    // nao e so membro 

            _service.Remove(1);
        }


        [TestMethod]
        public void MemberCommentDeleteCommentWithConflict2()
        {
            //
            // Apagar atravez de um membro que nao foi o mesmo que o inseriu causa conflito
            // 
            new TemporaryPrincipal("shanselman");   // Nao esta associado ao projecto deste issue

            bool result = _service.Remove(1);
            Assert.IsFalse(result);     // So o membro que inseriu o comentario o pode apagar

            // Verificar que nao apagou
            Assert.IsNotNull(_service.Repository.Query<Comment>().Single(c => c.CommentID == 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void MemberCommentDeleteCommentWithConflict3()
        {
            //
            // Apagar atravez de um cliente causa conflicto
            // 
            new TemporaryPrincipal("zonlusomundo"); // nao e membro

            _service.Remove(1);
        }

        [TestMethod]
        public void MemberCommentDeleteCommentWithoutConflict() 
        {
            //
            // Apagar atravez do membro que o reportou nao causa conflito
            // 
            new TemporaryPrincipal("psilva"); // esta associado

            bool result = _service.Remove(1);
            Assert.IsTrue(result);

            // Verificar que apagou
            Assert.IsNull(_service.Repository.Query<Comment>().SingleOrDefault(c => c.CommentID == 1));
        }
    }
}
