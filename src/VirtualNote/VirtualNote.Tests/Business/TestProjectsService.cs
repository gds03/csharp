using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Services;

namespace VirtualNote.Tests.Business
{
    [TestClass]
    public class TestProjectsService
    {
        private IProjectsService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new ProjectsService(new MemoryRepository());
        }

        //
        // Insert
        [TestMethod]
        public void ProjectAddWithoutConflictName()
        {
            //
            // Inserir um nome que nao existe nao causa conflicto
            // 

            bool result = _service.Add(new ProjectServiceDTO {
                Name = "Nome Estranho",
                Description = "nenhuma",
                ClientId = 5,   // Zon project
                ResponsableId = 1 // gdias
            });

            // Nome nao se encontra repetido
            Assert.IsTrue(result);

            // Inseriu no repositorio
            Assert.IsNotNull(_service.Repository.Query<Project>().Single(c => c.Name == "Nome Estranho"));
        }

        [TestMethod]
        public void ProjectAddWithConflictName()
        {
            //
            // Inserir um nome que existe causa conflicto
            // 

            bool result = _service.Add(new ProjectServiceDTO {
                Name = "Vobis Project",
                Description = "nenhuma",

                ClientId = 5,       // Zon project      
                ResponsableId = 1   // scott
            });

            // Nome encontra-se repetido
            Assert.IsFalse(result);

            // Nao inseriu no repositorio
            Assert.AreEqual(1, _service.Repository.Query<Project>().Count(c => c.Name == "Vobis Project"));
        }


        //
        // Update
        [TestMethod]
        public void ProjectsUpdateWithoutConflictName()
        {
            //
            // Actualizar um nome que nao existe nao causa conflicto
            // 

            bool result = _service.Update(new ProjectServiceDTO {
                Name = "Nome Estranho",

                ProjectID = 1,  // zonlusomundo
                ResponsableId = 1,  // gdias
                ClientId = 5    // zon
            });

            // Nome nao se encontra repetido
            Assert.IsTrue(result);

            // Actualizou no repositorio
            Assert.IsNotNull(_service.Repository.Query<Project>().Single(c => c.Name == "Nome Estranho"));
        }

        [TestMethod]
        public void ProjectsUpdateWithConflictName()
        {
            //
            // Actualizar para um nome que existe causa conflicto
            // 

            bool result = _service.Update(new ProjectServiceDTO {
                Name = "Vobis Project",

                ProjectID = 1,  // zon project
                ResponsableId = 1, // gdias
                ClientId = 5 // zon
            });

            // Nome encontra-se repetido
            Assert.IsFalse(result);

            // Não actualizou o repositorio
            Assert.AreEqual(1, _service.Repository.Query<Project>().Count(u => u.Name == "Vobis Project"));
        }

        [TestMethod]
        public void ProjectsUpdateWithResponsableThanIsCurrentlyWorking()
        {
            //
            // Actualizar um projecto cujo tem o gdias como responsavel e o psilva como worker
            // para o psilva como responsavel.. o estado deve ficar sem workers e o psilva responsavel
            // 

            var ctxProject = _service.Repository.Query<Project>().Single(p => p.ProjectID == 1);    // zon
            Assert.AreEqual(1, ctxProject.Workers.Count);

            bool result = _service.Update(new ProjectServiceDTO {
                Name = "ZonLusomundo Website",

                ProjectID = 1,  // zonproject
                ResponsableId = 3, // Mudo o responsavel, mas este responsavel encontra-se a trabalhar. Para ver se o serviço funciona o responsavel deve ficar este e este deve sair da lista de trabalhadores.
                ClientId = 5 // zon
            });

            Assert.IsTrue(result);
            
            // Removeu o psilva dos workers
            Assert.AreEqual(0, ctxProject.Workers.Count);

            // psilva é agora responsavel
            Assert.IsTrue(ctxProject.Responsable.UserID == 3);
        }



        //
        // Delete
        [TestMethod]
        public void ProjectDeleteWithoutConflict()
        {
            //
            // Apagar um projecto sem issues nao causa conflicto
            // 
            string projectName;

            bool result = _service.Remove(2, out projectName); // vobis project

            // Projecto vobis nao tem issues
            Assert.IsTrue(result);

            // Removeu do repositorio
            Assert.IsNull(_service.Repository.Query<Project>().SingleOrDefault(c => c.ProjectID == 2));
        }

        [TestMethod]
        public void ProjectDeleteWithConflict()
        {
            //
            // Apagar um projecto com issues causa conflicto
            // 
            string projectName;

            bool result = _service.Remove(1, out projectName); // zonproject

            // Projecto zon tem issues
            Assert.IsFalse(result);

            // Nao removeu do repositorio
            Project p;
            Assert.IsNotNull(p = _service.Repository.Query<Project>().SingleOrDefault(c => c.ProjectID == 1));

            // Verificar que ficou disabled
            Assert.IsFalse(p.Enabled);
        }

       
    }
}
