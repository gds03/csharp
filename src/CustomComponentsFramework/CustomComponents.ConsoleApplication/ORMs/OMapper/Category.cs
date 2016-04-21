using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomComponents.ConsoleApplication.ORMs.OMapper
{
    public class Category
    {
        //[Identity]
        //[PrimaryKey] --> this is removed, now we use OMapper.Configure() method to configure instead of use custom attributes.
        public virtual int id { get; set; }

        public virtual string name { get; set; }

        public virtual DateTime creationDate { get; set; }

        public virtual DateTime lastModifiedDate { get; set; }

        public virtual String extraInfo { get; set; }
    }
}
