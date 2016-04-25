using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Tests.Database.DomainObjects
{
    [TestClass]
    public class TestComments
    {
        [TestMethod]
        public void CommentAddUser()
        {
            var comment = new Comment {
                CommentID = 1,
                Description = "Goncalo Comment",
                User = new Member {
                    UserID = 1,
                    Name = "Gonçalo"
                }
            };

            Assert.IsTrue(comment.User.Comments.First() == comment);
        }

        [TestMethod]
        public void CommentReplaceUser()
        {
            var comment = new Comment {
                CommentID = 1,
                Description = "Goncalo Comment",
                User = new Member {
                    UserID = 1,
                    Name = "Gonçalo"
                }
            };

            User goncalo = comment.User;
            Assert.IsTrue(goncalo.Comments.First() == comment);

            comment.User = new Member {
                UserID = 2,
                Name = "Scoot"
            };

            Assert.IsTrue(goncalo.Comments.FirstOrDefault() == null);
            Assert.IsTrue(comment.User.Comments.First() == comment);
        }

        [TestMethod]
        public void CommentReplaceUserWithNull()
        {
            var comment = new Comment {
                CommentID = 1,
                Description = "Goncalo Comment",
                User = new Member {
                    UserID = 1,
                    Name = "Gonçalo"
                }
            };

            User goncalo = comment.User;
            Assert.IsTrue(goncalo.Comments.First() == comment);
            
            comment.User = null;
            Assert.IsTrue(goncalo.Comments.FirstOrDefault() == null);
            Assert.IsTrue(comment.User == null);
        }

        
    }
}
