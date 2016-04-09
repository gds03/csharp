using Repository.OMapper;
using Repository.OMapper.Attributes;
using Repository.OMapper.Internal.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.ConsoleApplication
{
    public class Product
    {
        [Identity]
        [PrimaryKey]
        public int id { get; set; }
        public int categoryId { get; set; }

        public string name { get; set; }

        public DateTime creationDate { get; set; }

        public DateTime lastModifiedDate { get; set; }

        public String extraInfo { get; set; }
    }


    public class Category
    {
        //[Identity]
        //[PrimaryKey] --> this is removed, now we use OMapper.Configure() method to configure instead of use custom attributes.
        public int id { get; set; }

        public string name { get; set; }

        public DateTime creationDate { get; set; }

        public DateTime lastModifiedDate { get; set; }

        public String extraInfo { get; set; }
    }

    public static class OMapperProgram
    {

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



        private static void AddCategoriesMassRandomOperations(OMapperContextExecuter oMapper)
        {
            const int ITERATIONS = 20000;
            Random r = new Random();

            Category c;
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
            oMapper.Submit();

            IList<Category> categoriesHalf = oMapper.Select<Category>(x => x.id > 1000);

            for (int i = 0; i < ITERATIONS; i++)
            {
                if (i % 2 == 0)
                {
                    categoriesHalf[i].name += "_%2";
                    categoriesHalf[i].extraInfo = "UPDATED" + i.ToString();
                }
            }

            oMapper.Delete(categoriesHalf[0]);
            oMapper.Delete(categoriesHalf[1]);

            oMapper.Submit();
        }

        public static void Main(String[] args)
        {
            // OMapperEagerExecuter oMapperEager = new OMapperEagerExecuter(CONNECTION_STRING);


            OMapperContextExecuter oMapper = new OMapperContextExecuter(CONNECTION_STRING);

            OMapperContextExecuter.Configuration(i =>
            {
                i.For<Category>().PrimaryKey(x => x.id)
                                 .Identity(x => x.id)
                                 // .BindFrom()
                                 // .BindTo()
                                ;

                //i.For<Product>().PrimaryKey(x => x.name)
                //                .Identity(x => x.id);
            });

            AddCategoriesMassRandomOperations(oMapper);
            // AddCategoriesAndUpdate(mapper);
        }

        
    }
}
