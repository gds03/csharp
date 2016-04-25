using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Tests.Database.DomainObjects
{
    [TestClass]
    public class TestUser
    {
        [TestMethod]
        public void UserAddComment()
        {
            //
            // Adicionar a lista de comentarios de um user, e verificar se esse user
            // fica referenciado no comentario
            // 
            User u = new Member {
                Name = "Gonçalo"
            };

            var c = new Comment {
                Description = "Goncalo Comment"
            };

            Assert.IsNull(c.User);
            
            u.Comments.Add(c);

            Assert.IsTrue(c.User == u); // 2-way established
        }

        [TestMethod]
        public void UserRemoveComment()
        {
            //
            // Inserimos um comentario na lista de comentarios de gonçalo 
            // e removemos, para verificar se o User fica a null.
            //
            User u = new Member {
                Name = "Gonçalo"
            };

            var c = new Comment {
                Description = "Goncalo Comment"
            };

            u.Comments.Add(c);

            Assert.IsTrue(c.User == u); // 2-way established

            u.Comments.Remove(c);

            Assert.IsNull(c.User);
            Assert.IsTrue(u.Comments.Count == 0);
        }
    }
}
