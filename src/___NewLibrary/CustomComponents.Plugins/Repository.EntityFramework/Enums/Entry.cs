using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EntityFramework.Enums
{
    public enum Entry
    {
        Unchanged = (int)EntityState.Unchanged,
        Added = (int)EntityState.Added,
        Deleted = (int)EntityState.Deleted,
        Modified = (int)EntityState.Modified,
    }
}
