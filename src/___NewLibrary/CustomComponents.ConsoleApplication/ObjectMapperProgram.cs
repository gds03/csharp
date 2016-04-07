using Repository.ObjectMapper;
using Repository.ObjectMapper.Attributes;
using Repository.ObjectMapper.Internal.Metadata;
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
        //[PrimaryKey] --> this is removed, now we use ObjectMapper.Initialize to initialize instead of use custom attributes.
        public int id { get; set; }

        public string name { get; set; }

        public DateTime creationDate { get; set; }

        public DateTime lastModifiedDate { get; set; }

        public String extraInfo { get; set; }
    }

    public static class ObjectMapperProgram
    {

        const string CONNECTION_STRING = "Server=.\\MSSQLSERVER2012;Database=TestDB;Trusted_Connection=True;";



        private static void AddCategoriesAndUpdate(ObjectMapper mapper)
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
            //mapper.Update(c2);
            //mapper.Update(c3);

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





        public static void Main(String[] args)
        {
            ObjectMapper mapper = new ObjectMapper(CONNECTION_STRING);

            ObjectMapper.Initialization += i =>
            {
                i.For<Category>().PrimaryKey(x => x.id)
                                 .Identity(x => x.id);
                                    // .OtherFluentMethodsHere()

            };

            AddCategoriesAndUpdate(mapper);
        }
    }
}
