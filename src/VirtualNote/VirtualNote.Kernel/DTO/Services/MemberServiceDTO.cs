using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualNote.Kernel.DTO
{
    public sealed class MemberServiceDTO : UserServiceDTO
    {
        public bool IsAdmin { get; set; }
    }
}
