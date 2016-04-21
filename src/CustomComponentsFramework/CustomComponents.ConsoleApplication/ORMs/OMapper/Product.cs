using Repository.OMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
