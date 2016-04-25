using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualNote.Kernel.DTO.Query.User
{
    public sealed class UserQueryInfo : IQueryDTO
    {
        public String Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Enabled { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
    }
}
