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
    public class TestIssuesClientsService
    {
        private IIssueClientService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new IssueClientService(new MemoryRepository(), new TestClientNotificator());
        }

        //
        // Insert
        [TestMethod]        
        public void ClientIssueAddWithoutConflict()
        {
            //
            // Inserir um issue do cliente associado nao causa conflicto
            // 
            new TemporaryPrincipal("zonlusomundo");

            _service.Add(new IssueServiceClientDTO {
                LongDescription = "LONNNGG description",
                ShortDescription = "SHORT D",
                Priority = PriorityEnum.Highest,
                Type = TypeEnum.Bug,
                ProjectId = 1
            });

            // Testar se inseriu no repositorio
            Assert.IsNotNull(_service.Repository.Query<Issue>().Single(i => i.LongDescription == "LONNNGG description" && i.Project.ProjectID == 1));

        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void ClientIssueAddWithConflictOnPermission()
        {
            //
            // Inserir um issue atravez de um admin causa conflicto
            // 
            new TemporaryPrincipal("gdias");

            _service.Add(new IssueServiceClientDTO {    // Excepcao
                LongDescription = "LONNNGG description",
                ShortDescription = "SHORT D",
                Priority = PriorityEnum.Highest,
                Type = TypeEnum.Bug,
                ProjectId = 1
            });
        }

        [TestMethod]
        [ExpectedException(typeof(HijackedException))]
        public void ClientIssueAddWithConflictOnHijacking()
        {
            //
            // Inserir um issue do cliente não associado causa conflicto.
            // Neste caso tentamos inserir um issue num projecto com o id=2
            // cujo é vobis. Mas este cliente nao esta associado a esse projecto.
            // 
            new TemporaryPrincipal("zonlusomundo");

            _service.Add(new IssueServiceClientDTO {    // Excepcao
                LongDescription = "LONNNGG description",
                ShortDescription = "SHORT D",
                Priority = PriorityEnum.Highest,
                Type = TypeEnum.Bug,
                ProjectId = 2
            });
        }

        //
        // Update
        [TestMethod]
        public void ClientIssueUpdateWithoutConflict()
        {
            //
            // Actualizar um issue do cliente associado nao causa conflicto
            // 
            new TemporaryPrincipal("zonlusomundo");

            bool result = _service.Update(new IssueServiceClientDTO {
                LongDescription = "Update Test",
                ShortDescription = "SHORT D",
                Priority = PriorityEnum.Highest,
                Type = TypeEnum.Bug,
                ProjectId = 1, // ignored
                IssueId = 1
            });

            Assert.IsTrue(result);

            // Verificar que actualizou no repositorio
            Assert.AreEqual("Update Test", _service.Repository.Query<Issue>().Single(i => i.IssueID == 1).LongDescription);
        }


        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void ClientIssueUpdateWithConflictOnPermission() 
        {
            //
            // Actualizar um issue atravez de um admin causa conflicto
            //
            new TemporaryPrincipal("gdias");

            _service.Update(new IssueServiceClientDTO {
                LongDescription = "Update Test",
                ShortDescription = "SHORT D",
                Priority = PriorityEnum.Highest,
                Type = TypeEnum.Bug,
                ProjectId = 1, // ignored
                IssueId = 1
            });
        }

        [TestMethod]
        [ExpectedException(typeof(HijackedException))]
        public void ClientIssueUpdateWithConflictOnHijacking() 
        {
            //
            // Actualizar um issue num cliente nao associado causa excepcao
            //
            new TemporaryPrincipal("worten");

            _service.Update(new IssueServiceClientDTO { // Excepcao
                LongDescription = "Update Test",
                ShortDescription = "SHORT D",
                Priority = PriorityEnum.Highest,
                Type = TypeEnum.Bug,
                ProjectId = 1, // ignored
                IssueId = 1
            });
        }

        //
        // Delete
        [TestMethod]
        public void ClientIssueDeleteWithoutConflict() 
        {
            //
            // Apagar um issue de um cliente associado nao causa conflicto
            // 
            new TemporaryPrincipal("zonlusomundo");

            bool result = _service.Remove(1);
            Assert.IsTrue(result);

            // Verificar que removeu do repositorio
            Assert.IsNull(_service.Repository.Query<Issue>().SingleOrDefault(i => i.IssueID == 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceAccessDeniedException))]
        public void ClientIssueDeleteWithConflictOnPermission() 
        {
            //
            // Apagar um issue atravez de um admin causa conflicto
            //
            new TemporaryPrincipal("gdias");

            _service.Remove(1); // Excepcao
        }

        [TestMethod]
        [ExpectedException(typeof(HijackedException))]
        public void ClientIssueDeleteWithConflictOnHijacking() 
        {
            //
            // Apagar um issue atravez de outro cliente causa Excepcao
            //
            new TemporaryPrincipal("worten");

            _service.Remove(1); // Excepcao
        }

        [TestMethod]
        public void ClientIssueDeleteWithConflictIssueNotWaiting(){
            //
            // Apagar um issue num estado != waiting causa conflicto

            new TemporaryPrincipal("oni");
            Issue dbIssue = _service.Repository.Query<Issue>().Single(i => i.ShortDescription == "Issue 1 - short description");
            Assert.IsTrue((StateEnum) dbIssue.State == StateEnum.InResolution);

            bool result = _service.Remove(dbIssue.IssueID);
            Assert.IsFalse(result);
        }
    }
}
