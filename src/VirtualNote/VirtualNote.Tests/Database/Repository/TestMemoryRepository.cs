using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Tests.Database.Repository
{
    [TestClass]
    public class TestMemoryRepository
    {
        MemoryRepository _db;

        [TestInitialize]
        public void Init(){
            _db  = new MemoryRepository();
        }




        [TestMethod]
        public void TestAddMember()
        {
            //
            // Cria um novo membro e testa se o id do membro é o ultimo + 1 depois de inserid            
            // 

            var member = new Member { Name = "Gestrudes" };

            Assert.AreEqual(0, member.UserID);

            // obter o ultimo id de user na bd
            int lastUserIdOnDb = _db.Query<User>().Max(u => u.UserID);


            _db.Insert(member);

            // Verificar se o id do user foi alterado 
            Assert.AreEqual(lastUserIdOnDb + 1, member.UserID);
        }

        [TestMethod]
        public void TestRemoveMember()
        {
            //
            // Inicialmente existe gdias, depois apagamos e confirmamos se apagou do repositorio
            // 

            Member gdias;

            // Existe gdias
            Assert.IsNotNull(gdias = _db.Query<Member>().Single(m => m.Name == "gdias"));

            _db.Delete(gdias);

            // Nao existe gdias
            Assert.IsNull(_db.Query<Member>().SingleOrDefault(m => m.Name == "gdias"));
        }

        [TestMethod]
        public void TestQueryUser()
        {
            //
            // Para testar uma query a user podemos retornar um Membro e um Client,
            // por exemplo gdias e zonlusomundo
            // 
            Assert.IsNotNull(_db.Query<User>().Single(u => u.Name == "gdias"));
            Assert.IsNotNull(_db.Query<User>().Single(u => u.Name == "zonlusomundo"));
        }
    }
}
