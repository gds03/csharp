using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Tests.Database.DomainObjects
{
    [TestClass]
    public class TestProjects
    {
        [TestMethod]
        public void ProjectAddWorker()
        {
            var project = new Project {
                Description = "VirtualNote"
            };

            var goncalo = new Member { UserID = 1, Name = "Gonçalo" };
            var scott   = new Member { UserID = 2, Name = "Scott" };

            project.Workers.Add(goncalo);
            project.Workers.Add(scott);


            Assert.IsTrue(goncalo.AssignedProjects.Contains(project));
            Assert.IsTrue(scott.AssignedProjects.Contains(project));
        }


        [TestMethod]
        public void ProjectAddUserRemoveUser()
        {
            var project = new Project {
                Description = "VirtualNote"
            };

            var goncalo = new Member { UserID = 1, Name = "Gonçalo" };
            var scott = new Member { UserID = 2, Name = "Scott" };

            project.Workers.Add(goncalo);
            Assert.IsTrue(goncalo.AssignedProjects.Contains(project));

            goncalo.AssignedProjects.Remove(project);
            Assert.IsTrue(!goncalo.AssignedProjects.Contains(project));
        }
    }
}
