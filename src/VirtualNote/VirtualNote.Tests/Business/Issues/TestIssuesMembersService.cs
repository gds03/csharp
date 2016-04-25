using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.Contracts.Issues;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Services.Issues;
using VirtualNote.Tests.EmptyServices.Notificator;

namespace VirtualNote.Tests.Business.Issues
{
    [TestClass]
    public class TestIssuesMembersService
    {
        private IIssueMemberService _service;

        [TestInitialize]
        public void Init() 
        {
            _service = new IssueMemberService(new MemoryRepository(), new TestMemberNotificator());
        }

        //
        // Change State
        [TestMethod]
        public void MemberIssueChangeStateWithoutConflict() 
        {
            //
            // psilva é worker no projecto do issueId = 1 e consegue alterar o estado devido a ser o primeiro
            // membro a faze-lo.
            // 
            new TemporaryPrincipal("psilva");

            bool result = _service.ChangeState(new IssueServiceMemberDTO {
                IssueId = 1,
                State = StateEnum.InResolution
            });

            Assert.IsTrue(result);

            // Verificar se em resolucao no estado no repositorio
            Assert.IsTrue(_service.Repository.Query<Issue>().Single(i => i.IssueID == 1).State == (int)StateEnum.InResolution);
        }

        [TestMethod]
        [ExpectedException(typeof(IssueWasAlreadyTakedByAnotherMember))]
        public void MemberIssueChangeStateConflictGustavoGSetted() {
            //
            // psilva nao consegue alterar o estado porque o GUstavoG ja o alterou e tem que ser ele a altera-lo
            // novamente
            // 
            new TemporaryPrincipal("psilva");

            Issue dbIssue = _service.Repository.Query<Issue>().Single(i => i.ShortDescription == "Issue 1 - short description");
            Assert.IsTrue(dbIssue.State == (int) StateEnum.InResolution);

            bool result = _service.ChangeState(new IssueServiceMemberDTO {
                IssueId = dbIssue.IssueID,
                State = StateEnum.Terminated
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void MemberIssueChangeStateWithConflictPermission()
        {
            //
            // zonlusomundo nao é membro
            // 
            new TemporaryPrincipal("zonlusomundo");

            _service.ChangeState(new IssueServiceMemberDTO { // Excepcao
                IssueId = 1,
                State = StateEnum.InResolution
            });
        }

        [TestMethod]
        [ExpectedException(typeof(HijackedException))]
        public void MemberIssueChangeStateWithConflictHijacked() 
        {
            //
            // ggrande nao esta associado a zonlusomundo e esta a tentar alterar o estado
            // 
            new TemporaryPrincipal("ggrande");      // nao trabalha nesse projecto

            _service.ChangeState(new IssueServiceMemberDTO {    // Excepcao
                IssueId = 1,
                State = StateEnum.InResolution
            });
        }
    }
}
