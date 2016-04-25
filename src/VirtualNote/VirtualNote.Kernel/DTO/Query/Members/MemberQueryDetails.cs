using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualNote.Kernel.DTO.Query.User;

namespace VirtualNote.Kernel.DTO.Query.Members
{
    public sealed class MemberQueryDetails : IQueryDTO
    {
        public int MemberId { get; set; }
        public bool IsAdmin { get; set; }

        public UserQueryInfo UserInfo { get; set; }

        public IEnumerable<String> ResponsableInProjects { get; set; }

        public IEnumerable<String> EnabledProjects { get; set; }
        public IEnumerable<String> DisabledProjects { get; set; }

        public int IssuesSolved { get; set; }
    }
}
