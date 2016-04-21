using OMapper.Attributes;
using System;

namespace CustomComponents.ConsoleApplication.ORMs.OMapper
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




}
