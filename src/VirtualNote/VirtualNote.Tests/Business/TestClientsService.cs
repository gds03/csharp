using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Services;
using System.Linq;
using VirtualNote.Tests.EmptyServices;

namespace VirtualNote.Tests.Business
{
    [TestClass]
    public class TestClientsService
    {
        private IClientsService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new ClientsService(new MemoryRepository(), new TestEmailConfigMngr());
        }

        //
        // Insert
        [TestMethod]
        public void ClientsAddWithoutConflictName()
        {
            //
            // Inserir um nome que nao existe nao causa conflicto
            // 

            bool result = _service.Add(new ClientServiceDTO {
                Name = "Nome Estranho",
                Password = "Pass Estranha",
                Email = "Email Estranho..."
            });

            // Nome nao se encontra repetido
            Assert.IsTrue(result);

            // Inseriu no repositorio
            Assert.IsNotNull(_service.Repository.Query<Client>().Single(c => c.Name == "Nome Estranho"));
        }

        [TestMethod]
        public void ClientsAddWithConflictName()
        {
            //
            // Inserir um nome que existe causa conflicto
            // 

            bool result = _service.Add(new ClientServiceDTO {
                Name = "gdias",
                Password = "Pass Estranha",
                Email = "Email Estranho..."
            });

            // Nome existe num membro
            Assert.IsFalse(result);
            
            // Nao inseriu no repositorio
            Assert.AreEqual(1, _service.Repository.Query<User>().Count(c => c.Name == "gdias"));
        }




        //
        // Update
        [TestMethod]
        public void ClientsUpdateWithoutConflictName()
        {
            //
            // Actualizar para um nome que nao existe nao causa conflicto
            // 

            bool result = _service.Update(new ClientServiceDTO {
                UserID = 5, // zonlusomundo
                Name = "xptoName",
                Password = "Pass Estranha",
                Email = "Email Estranho..."
            });
            
            // Nome nao se encontra repetido
            Assert.IsTrue(result);

            // Actualizou no repositorio
            Assert.IsNotNull(_service.Repository.Query<Client>().Single(c => c.Name == "xptoName"));
        }

        [TestMethod]
        public void ClientsUpdateWithConflictName()
        {
            //
            // Actualizar para um nome que existe causa conflicto
            // 

            bool result = _service.Update(new ClientServiceDTO {
                UserID = 5, // zonlusomundo
                Name = "shanselman",
                Password = "Pass Estranha",
                Email = "Email Estranho..."
            });

            // Nome existe num membro
            Assert.IsFalse(result);

            // Não actualizou o repositorio
            Assert.AreEqual(1, _service.Repository.Query<User>().Count(u => u.Name == "shanselman"));
        }


        //
        // Delete
        [TestMethod]
        public void ClientsDeleteWithoutConflict(){
            //
            // Apagar um cliente sem projectos nao causa conflicto
            // 
            string clientName;
            bool result = _service.Remove(6, out clientName);   // worten

            // Não contem projectos associados
            Assert.IsTrue(result);

            // Removeu do repositorio
            Assert.IsNull(_service.Repository.Query<Client>().SingleOrDefault(c => c.UserID == 6));
        }


        [TestMethod]
        public void ClientsDeleteWithConflict()
        {
            //
            // Apagar um cliente com projectos causa conflicto
            // 
            string clientName;

            bool result = _service.Remove(5, out clientName);   // zonlusomundo

            // Contem um projecto associado e nao pode ser apagado.
            Assert.IsFalse(result);

            // Não removeu do repositorio
            Client zonClient;
            Assert.IsNotNull(zonClient = _service.Repository.Query<Client>().SingleOrDefault(c => c.UserID == 5));

            // Verificar a flag enabled a false
            Assert.IsFalse(zonClient.Enabled);
        }
    }
}
