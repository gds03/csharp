using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.Contracts.Issues;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Services.Issues;
using System.Linq;
using VirtualNote.Tests.EmptyServices.Notificator;

namespace VirtualNote.Tests.Business.Issues
{
    [TestClass]
    public class TestIssuesAdminsService
    {
        private IIssueAdminService _service;

        [TestInitialize]
        public void Init() {
            _service = new IssueAdminService(new MemoryRepository(), new TestMemberNotificator());
        }

        //
        // Change State
        [TestMethod]
        public void AdminIssueChangeStateWithoutConflict()
        {
            //
            // gdias é admin => consegue mudar o estado de todos os issues
            // 
            new TemporaryPrincipal("gdias");

            bool result = _service.ChangeState(new IssueServiceMemberDTO {
                IssueId = 1,
                State = StateEnum.InResolution
            });

            // Alterou o estado
            Assert.IsTrue(result);

            // Verificar se em resolucao no estado no repositorio
            Assert.IsTrue(_service.Repository.Query<Issue>().Single(i => i.IssueID == 1).State == (int)StateEnum.InResolution);
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void AdminIssueChangeStateWithConflict() 
        {
            //
            // shanselman nao é admin, nao consegue executar metodo
            // 
            new TemporaryPrincipal("shanselman"); // nao e admin

            _service.ChangeState(new IssueServiceMemberDTO { // Excepcao
                IssueId = 1,
                State = StateEnum.InResolution
            });
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void AdminIssueChangeStateWithConflict2() 
        {
            //
            // zonlusomundo nao é admin, nao consegue executar metodo
            // 
            new TemporaryPrincipal("zonlusomundo"); // é cliente

            _service.ChangeState(new IssueServiceMemberDTO { // Excepcao
                IssueId = 1,
                State = StateEnum.InResolution
            });
        }

        [TestMethod]
        public void AdminIssueChangeStateFromTerminatedToOther(){
            //
            // gdias é admin => consegue mudar o estado de todos os issues
            // 
            new TemporaryPrincipal("gdias");

            Issue dbIssue = _service.Repository.Query<Issue>().Single(i => i.ShortDescription == "Issue 7 - short description");
            Assert.IsTrue(dbIssue.State == (int) StateEnum.Terminated);

            bool result =_service.ChangeState(new IssueServiceMemberDTO{
                IssueId = dbIssue.IssueID,
                State = StateEnum.InResolution
            });

            Assert.IsFalse(result);

            // Verificar que nao tocou no repositorio
            Assert.IsTrue(_service.Repository.Query<Issue>().Single(i => i.IssueID == dbIssue.IssueID).State == (int) StateEnum.Terminated);
        }

        [TestMethod]
        public void AdminIssueChangeStateFromResolutionToWaitState(){
            //
            // gdias é admin => consegue mudar o estado de todos os issues
            // 
            new TemporaryPrincipal("gdias");

            Issue dbIssue = _service.Repository.Query<Issue>().Single(i => i.ShortDescription == "Issue 1 - short description");
            Assert.IsTrue(dbIssue.State == (int)StateEnum.InResolution);

            bool result = _service.ChangeState(new IssueServiceMemberDTO {
                IssueId = dbIssue.IssueID,
                State = StateEnum.Waiting
            });

            Assert.IsTrue(result);
            Assert.IsNull(dbIssue.Member);

            // Verificar repositorio
            Assert.IsTrue(_service.Repository.Query<Issue>().Single(i => i.IssueID == dbIssue.IssueID).State == (int) StateEnum.Waiting);
        }


        //
        // Remove
        [TestMethod]
        public void AdminIssueRemoveWithoutConflict() 
        {
            //
            // gdias é admin consegue remover todos os issues
            //

            new TemporaryPrincipal("gdias"); 

            _service.Remove(1); // Issue da zon

            // Verifica que removeu do repositorio
            Assert.IsNull(_service.Repository.Query<Issue>().SingleOrDefault(i => i.IssueID == 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void AdminIssueRemoveWithConflict1()
        {
            //
            // shanselman não é admin nao consegue executar metodo
            //
            new TemporaryPrincipal("shanselman");

            _service.Remove(1);  // Excepcao

        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void AdminIssueRemoveWithConflict2() {
            //
            // zonlusomundo não é admin nao consegue executar metodo
            //
            new TemporaryPrincipal("zonlusomundo");

            _service.Remove(1);  // Excepcao

        }

    }
}
