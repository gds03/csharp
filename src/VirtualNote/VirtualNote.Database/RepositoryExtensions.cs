using System;
using VirtualNote.Common;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Database
{
    static class RepositoryExtensions
    {
        internal static void AddDefaultData(this IRepository repository, bool loadTestData) {

            #region Members

            Member admin = new Member
            {
                Name = "admin",
                CreatedDate = DateTime.Now,
                Enabled = true,
                Password = PasswordUtils.Encript("admin"),
                IsAdmin = true,
                Email = "admin@admin.admin"
            };

            repository.Insert(admin);           // id = 1

            if (!loadTestData)
                return;

            //
            // Members
            Member gdias = new Member {
                Name = "gdias",
                CreatedDate = DateTime.Now,
                Enabled = true,
                Password = PasswordUtils.Encript("admin"),
                IsAdmin = true,
                Email = "gdiasvn@gmail.com"
            };
            Member sHanselman = new Member {
                Name = "shanselman",
                CreatedDate = DateTime.Now,
                Enabled = true,
                Password = PasswordUtils.Encript("foo"),
                Email = "shanselmanvn@gmail.com"
            };
            Member pSilva = new Member {
                Name = "psilva",
                CreatedDate = DateTime.Now,
                Enabled = true,
                Password = PasswordUtils.Encript("foo2"),
                Email = "psilvavn@gmail.com"
            };

            Member gustavoG = new Member {
                Name = "gustavog",
                CreatedDate = DateTime.Now,
                Enabled = true,
                Password = PasswordUtils.Encript("foo3"),
                Email = "gustavogamavn@gmail.com"
            };

            


            repository.Insert(gdias);           // id = 1
            repository.Insert(sHanselman);      // id = 2
            repository.Insert(pSilva);          // id = 3
            repository.Insert(gustavoG);        // id = 4

            #endregion



            #region Clients

            //
            // Clients
            Client clientZON = new Client {
                Name = "zonlusomundo",
                CreatedDate = DateTime.Now,
                Enabled = true,
                Password = PasswordUtils.Encript("zonpassword")
            };

            Client clientWORTEN = new Client {
                Name = "worten",
                CreatedDate = DateTime.Now,
                Enabled = true,
                Password = PasswordUtils.Encript("wortenpassword")
            };

            Client clientVobis = new Client {
                Name = "vobis",
                CreatedDate = DateTime.Now,
                Enabled = true,
                Password = PasswordUtils.Encript("vobispassword")
            };

            Client clientOni = new Client {
                Name = "oni",
                CreatedDate = DateTime.Now,
                Enabled = true,
                Password = PasswordUtils.Encript("onipassword"),
                Email = "oninetvn@gmail.com"
            };

            repository.Insert(clientZON);       // id = 5
            repository.Insert(clientWORTEN);    // id = 6
            repository.Insert(clientVobis);     // id = 7
            repository.Insert(clientOni);       // id = 8


            #endregion




            #region Projects

            //
            // Projects
            Project zonPROJECT = new Project {
                Name = "ZonLusomundo Website",
                Enabled = true,
                CreatedDate = DateTime.Now,
                Description = "Um site feito em 2011",

                Responsable = gdias,
                Client = clientZON,
            };

            Project VobisPROJECT = new Project {
                Name = "Vobis Project",
                Enabled = true,
                CreatedDate = DateTime.Now,
                Description = "Descricao a moda da vobis",

                Responsable = gustavoG,
                Client = clientVobis
            };

            Project OniPROJECT = new Project {
                Name = "Oninet",
                Enabled = true,
                CreatedDate = DateTime.Now,
                Description = "Descricao a moda da Oninet",

                Responsable = gustavoG,
                Client = clientOni
            };

            repository.Insert(zonPROJECT);        // id = 1
            repository.Insert(VobisPROJECT);      // id = 2
            repository.Insert(OniPROJECT);        // id = 3



            //
            // Workers
            zonPROJECT.Workers.Add(pSilva);       // pSilva is working on zonProject

            OniPROJECT.Workers.Add(pSilva);       // pSilva is working on oniProject


            #endregion




            #region Issues

            //
            //  Issue
            Issue zonISSUE = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Man i found an error :/",
                LongDescription = "This is my detailed description of the error",
                Priority = 1,
                Type = 0,
                State = 0,

                Client = clientZON,
                Project = zonPROJECT
            };


            Issue oniISSUE1 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 1 - short description",
                LongDescription = "Issue 1 - long description",
                Priority = 1,
                Type = 0,
                State = 1,

                Client = clientOni,
                Project = OniPROJECT,
                Member = gustavoG
            };


            Issue oniISSUE2 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 2 - short description",
                LongDescription = "Issue 2 - long description",
                Priority = 4,
                Type = 1,
                State = 2,

                Client = clientOni,
                Project = OniPROJECT,
                Member = gustavoG
            };

            Issue oniISSUE3 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 3 - short description",
                LongDescription = "Issue 3 - long description",
                Priority = 3,
                Type = 1,
                State = 0,

                Client = clientOni,
                Project = OniPROJECT
            };

            Issue oniISSUE4 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 4 - short description",
                LongDescription = "Issue 4 - long description",
                Priority = 2,
                Type = 1,
                State = 0,

                Client = clientOni,
                Project = OniPROJECT
            };

            Issue oniISSUE5 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 5 - short description",
                LongDescription = "Issue 5 - long description",
                Priority = 1,
                Type = 1,
                State = 0,

                Client = clientOni,
                Project = OniPROJECT
            };

            Issue oniISSUE6 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 6 - short description",
                LongDescription = "Issue 6 - long description",
                Priority = 3,
                Type = 1,
                State = 0,

                Client = clientOni,
                Project = OniPROJECT
            };

            Issue oniISSUE7 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 7 - short description",
                LongDescription = "Issue 7 - long description",
                Priority = 2,
                Type = 1,
                State = 2,

                Client = clientOni,
                Project = OniPROJECT,
                Member = gustavoG
            };

            Issue oniISSUE8 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 8 - short description",
                LongDescription = "Issue 8 - long description",
                Priority = 3,
                Type = 1,
                State = 0,

                Client = clientOni,
                Project = OniPROJECT
            };

            Issue oniISSUE9 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 9 - short description",
                LongDescription = "Issue 9 - long description",
                Priority = 3,
                Type = 1,
                State = 0,

                Client = clientOni,
                Project = OniPROJECT
            };

            Issue oniISSUE10 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 10 - short description",
                LongDescription = "Issue 10 - long description",
                Priority = 1,
                Type = 1,
                State = 2,

                Client = clientOni,
                Project = OniPROJECT,
                Member = gustavoG
            };

            Issue oniISSUE11 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 11 - short description",
                LongDescription = "Issue 11 - long description",
                Priority = 3,
                Type = 1,
                State = 0,

                Client = clientOni,
                Project = OniPROJECT
            };

            Issue oniISSUE12 = new Issue {
                CreatedDate = DateTime.Now,
                LastUpdateDate = null,
                ShortDescription = "Issue 12 - short description",
                LongDescription = "Issue 12 - long description",
                Priority = 4,
                Type = 1,
                State = 0,

                Client = clientOni,
                Project = OniPROJECT
            };
            

            repository.Insert(zonISSUE);            // id = 1
            repository.Insert(oniISSUE1);           // id = 2

            repository.Insert(oniISSUE2);           // id = 3
            repository.Insert(oniISSUE3);           // id = 4
            repository.Insert(oniISSUE4);           // id = 5
            repository.Insert(oniISSUE5);           // id = 6
            repository.Insert(oniISSUE6);           // id = 7
            repository.Insert(oniISSUE7);           // id = 8
            repository.Insert(oniISSUE8);           // id = 9
            repository.Insert(oniISSUE9);           // id = 10
            repository.Insert(oniISSUE10);          // id = 11
            repository.Insert(oniISSUE11);          // id = 12
            repository.Insert(oniISSUE12);          // id = 13


            #endregion




            #region Comments


            //
            // Comments
            Comment zonISSUEComment1 = new Comment {
                CreatedDate = DateTime.Now,
                Description = "This is a comment for test purposes..",
                User = pSilva,
                Issue = zonISSUE
            };

            Comment zonISSUEComment2 = new Comment {
                CreatedDate = DateTime.Now,
                Description = "This is a comment for test purposes..",
                User = clientZON,
                Issue = zonISSUE
            };

            repository.Insert(zonISSUEComment1);
            repository.Insert(zonISSUEComment2);


            // 12 Comments for Oni

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment1",
                User = clientOni,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment2",
                User = gustavoG,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment3",
                User = gustavoG,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment4",
                User = gustavoG,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment5",
                User = gustavoG,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment6",
                User = gustavoG,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment7",
                User = gustavoG,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment8",
                User = gustavoG,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment9",
                User = gustavoG,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment10",
                User = gustavoG,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment11",
                User = gustavoG,
                Issue = oniISSUE1
            });

            repository.Insert(new Comment {
                CreatedDate = DateTime.Now,
                Description = "Comment12",
                User = gustavoG,
                Issue = oniISSUE1
            });


            #endregion
        }
    }
}
