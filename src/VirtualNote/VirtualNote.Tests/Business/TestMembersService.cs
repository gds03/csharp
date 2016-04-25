using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Services;
using VirtualNote.Tests.EmptyServices;

namespace VirtualNote.Tests.Business
{
    [TestClass]
    public class TestMembersService
    {
        IMembersService _service;


        [TestInitialize]
        public void Init()
        {
            _service = new MembersService(new MemoryRepository(), new TestEmailConfigMngr());
        }

        //
        // Insert
        [TestMethod]
        public void MembersAddWithoutConflictName()
        {
            //
            // Inserir um nome que nao existe nao causa conflicto
            // 

            bool result = _service.Add(new MemberServiceDTO {
                Name = "Nome Estranho",
                Password = "Pass Estranha",
                Email = "Email Estranho..."
            });

            // Nome nao se encontra repetido
            Assert.IsTrue(result);

            // Inseriu no repositorio
            Assert.IsNotNull(_service.Repository.Query<Member>().Single(c => c.Name == "Nome Estranho"));
        }

        [TestMethod]
        public void MembersAddWithConflictName()
        {
            //
            // Inserir um nome que existe causa conflicto
            // 

            bool result = _service.Add(new MemberServiceDTO {
                Name = "shanselman",
                Password = "Pass Estranha",
                Email = "Email Estranho..."
            });

            // Nome existe num membro
            Assert.IsFalse(result);

            // Nao inseriu no repositorio
            Assert.AreEqual(1, _service.Repository.Query<User>().Count(c => c.Name == "shanselman"));
        }


        //
        // Update
        [TestMethod]
        public void MembersUpdateWithoutConflictName()
        {
            //
            // Actualizar um nome que nao existe nao causa conflicto
            // 

            bool result = _service.Update(new MemberServiceDTO {
                UserID = 1,
                Name = "xptoName",
                Password = "Pass Estranha",
                Email = "Email Estranho..."
            });

            // Nome nao se encontra repetido
            Assert.IsTrue(result);

            // Actualizou no repositorio
            Assert.IsNotNull(_service.Repository.Query<Member>().Single(c => c.Name == "xptoName"));
        }

        [TestMethod]
        public void MembersUpdateWithConflictName()
        {
            //
            // Actualizar para um nome que existe causa conflicto
            // 

            bool result = _service.Update(new MemberServiceDTO {
                UserID = 1,
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
        public void MembersDeleteWithoutConflict()
        {
            //
            // Apagar o shanselman nao causa conflicto (Sem projectos associados)
            // 
            string memberName;
            bool result = _service.Remove(2, out memberName); // shanselman

            // Não contem projectos associados
            Assert.IsTrue(result);

            // Removeu do repositorio
            Assert.IsNull(_service.Repository.Query<Member>().SingleOrDefault(c => c.UserID == 2));
        }


        
        [TestMethod]
        public void MembersDeleteWithConflictResponsable()
        {
            //
            // Apagar o ggrande causa conflicto (Com projectos associados)
            // 
            string memberName;
            bool result = _service.Remove(4, out memberName);

            // Contem um projecto responsavel e nao pode ser apagado.
            Assert.IsFalse(result);

            // Não Removeu do repositorio
            Member ggrande;
            Assert.IsNotNull(ggrande = _service.Repository.Query<Member>().SingleOrDefault(c => c.UserID == 4));

            // Verificar se ficou disabled
            Assert.IsFalse(ggrande.Enabled);
        }

        [TestMethod]
        [ExpectedException(typeof(MemberLastAdminException))]
        public void MembersDeleteWithConflictLastAdmin() {
            //
            // Apagar o gdias causa conflicto (é o unico admin e nao pode ser apagado)
            // 
            string memberName;
            bool result = _service.Remove(1, out memberName);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MembersDeleteWithConflictWorker()
        {
            //
            // Apagar o psilva causa conflicto (Com projectos associados)
            // 
            string memberName;
            bool result = _service.Remove(3, out memberName);   // psilva

            // Contem um projecto associado(worker) e nao pode ser apagado.
            Assert.IsFalse(result);

            // Não Removeu do repositorio
            Member psilva;
            Assert.IsNotNull(psilva = _service.Repository.Query<Member>().SingleOrDefault(c => c.UserID == 3));

            // Verificar se ficou disabled
            Assert.IsFalse(psilva.Enabled);
        }
    }
}
