using System;
using System.Collections.Generic;
using System.Diagnostics;
using CustomComponents.Repository.Interfaces;
using Repository.EntityFramework.Types.DbContextRepository;
using OMapper;
using Repository.OMapper;

using CustomComponents.ConsoleApplication.ORMs.OMapper;
// using CustomComponents.ConsoleApplication.ORMs.EFGenerated;

namespace CustomComponents.ConsoleApplication
{



    public static class OMapperProgram
    {
        // const string CONNECTION_STRING = "Server=VMWin2012Server\\MSSQLSERVER12;User Id=user; password=Password1!"; 
        const string CONNECTION_STRING = "Server=.\\MSSQLSERVER2012;Database=TestDB;Trusted_Connection=True;";



        private static void AddCategoriesAndUpdate(OMapperContextExecuter mapper)
        {
            Category c1 = new Category
            {
                name = "Kitchen",
                creationDate = DateTime.Now,
                lastModifiedDate = DateTime.Now
            };

            Category c2 = new Category
            {
                name = "Sports",
                creationDate = DateTime.Now,
                lastModifiedDate = DateTime.Now
            };

            Category c3 = new Category
            {
                name = "Eletronics",
                creationDate = DateTime.Now,
                lastModifiedDate = DateTime.Now
            };

            mapper.InsertMany(c1, c2, c3);
            //mapper.Insert(c2);
            //mapper.Insert(c3);

            c1.name = "kitchen_updated";
            c2.name = "Sports_updated";
            c3.name = "Eletronics_updated";

            // mapper.UpdateMany(c1, c2, c3);
            //mapper.Update(c1);

            mapper.Submit();

            IList<Category> categories = mapper.Select<Category>();

            if (categories.Count > 0)
            {
                categories[0].lastModifiedDate = DateTime.Now;
                categories[0].name += "updated";
            }

            if (categories.Count > 1)
            {
                categories[1].lastModifiedDate = DateTime.Now;
                categories[1].name += "updated";
                categories[1].extraInfo = "ExtraINFO here";
            }

            mapper.Delete(c1);

            // mapper.Update(c3);
            mapper.Submit();


        }



        private static void AddCategoriesMassRandomOperations<TORM>(TORM oMapper, string ORMName)
            where TORM : IRepository, IDatabaseStored
        {
            if (string.IsNullOrEmpty(ORMName))
            {
                throw new ArgumentException("ORMName");
            }

            const int ITERATIONS = 10000;
            Console.WriteLine($"---------------- { ORMName } --------------- \n\n");
            Console.WriteLine($"--- Number of objects to insert { ITERATIONS } --- \n\n");

            Random r = new Random();

            Category c;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < ITERATIONS; i++)
            {
                c = new Category
                {
                    creationDate = DateTime.Now,
                    name = i.ToString(),
                    lastModifiedDate = DateTime.Now,
                    extraInfo = "abc" + r.Next(0, 99)
                };
                oMapper.Insert(c);
            }
            watch.Stop();
            Console.WriteLine($"Took { watch.ElapsedMilliseconds }ms to insert { ITERATIONS } objects into the context");

            Console.WriteLine($"Sending commands to database");
            watch.Restart();
            oMapper.Submit();
            watch.Stop();
            Console.WriteLine($"Took { watch.ElapsedMilliseconds }ms to Commit transaction and grab update CLR Identity properties.");

            const int afterId = ITERATIONS - 50;
            watch.Restart();
            Console.WriteLine($"Making a select operation.");
            Console.WriteLine("Mapping objects into memory");
            IList<Category> categoriesObjs = oMapper.Query<Category>(x => x.id > afterId);
            Console.WriteLine($"Took { watch.ElapsedMilliseconds }ms to Select { categoriesObjs.Count } objects");
            watch.Stop();

            int updatedObjects = 0;
            for (int i = 0; i < ITERATIONS; i++)
            {
                if (i % 2 == 0)
                {
                    updatedObjects++;
                    categoriesObjs[i].name += "_%2";
                    categoriesObjs[i].extraInfo = "UPDATED" + i.ToString();
                }
            }

            int deletedObjects = 0;
            oMapper.Delete(categoriesObjs[0]);
            deletedObjects++;
            oMapper.Delete(categoriesObjs[1]);
            deletedObjects++;


            watch.Restart();
            Console.WriteLine($"Sending commands to database");

            oMapper.Submit();
            watch.Stop();

            Console.WriteLine($"Took { watch.ElapsedMilliseconds }ms to Submit the changes with { updatedObjects } Updated objects and { deletedObjects } deleted Objects");
            Console.WriteLine("Press something to leave");
            Console.Read();
        }

        public static void Main(String[] args)
        {
            OMapperEagerExecuter oMapperEager = new OMapperEagerExecuter(CONNECTION_STRING);
            OMapperContextExecuter oMapperInstance = new OMapperContextExecuter(CONNECTION_STRING);

            var repository = new OMapperRepository(oMapperInstance);

            OMapperContextExecuter oMapper = new OMapperContextExecuter(CONNECTION_STRING);

            oMapperInstance.Configuration(i =>
            {
                i.For<Category>().PrimaryKey(x => x.id)
                                 .Identity(x => x.id)
                                // .BindFrom()
                                // .BindTo()
                                ;

                //i.For<Product>().PrimaryKey(x => x.name)
                //                .Identity(x => x.id);
            });
            AddCategoriesMassRandomOperations(repository, "OMAPPER");



            //DbContextRepository repository = new DbContextRepository(new TestDBEntities());

            //AddCategoriesMassRandomOperations(repository, "Entity Framework");
        }
    }
}
