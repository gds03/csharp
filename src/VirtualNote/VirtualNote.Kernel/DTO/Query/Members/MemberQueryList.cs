using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualNote.Kernel.DTO.Query.User;

namespace VirtualNote.Kernel.DTO.Query.Members
{
    public sealed class MemberQueryList : IQueryDTO
    {
        public int MemberId { get; set; }
        public bool IsAdmin { get; set; }

        public UserQueryInfo UserInfo { get; set; }

        public int ResponsableInProjects { get; set; }

        public int EnabledProjects { get; set; }
        public int DisabledProjects { get; set; }

        public int IssuesSolved { get; set; }
    }
}
