using Repository.ObjectMapper;
using Repository.ObjectMapper.Attributes;
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
        [Identity]
        [PrimaryKey]
        public int id { get; set; }

        public string name { get; set; }

        public DateTime creationDate { get; set; }

        public DateTime lastModifiedDate { get; set; }

        public String extraInfo { get; set; }
    }

    public static class ObjectMapperProgram
    {

        const string CONNECTION_STRING = "Server=.\\MSSQLSERVER2012;Database=TestDB;Trusted_Connection=True;";



        private static void AddCategories(ObjectMapper mapper)
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

            mapper.Insert(c1);
            mapper.Insert(c2);
            mapper.Insert(c3);
        }





        public static void Main(String[] args)
        {
            ObjectMapper mapper = new ObjectMapper(CONNECTION_STRING);
            AddCategories(mapper);



            Console.ReadLine();
        }
    }
}
