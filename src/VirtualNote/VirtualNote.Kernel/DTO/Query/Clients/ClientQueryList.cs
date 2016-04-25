using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualNote.Kernel.DTO.Query.User;

namespace VirtualNote.Kernel.DTO.Query.Clients
{
    public sealed class ClientQueryList : IQueryDTO
    {
        public int ClientId { get; set; }
        public UserQueryInfo UserInfo { get; set; }

        public int EnabledProjects { get; set; }
        public int DisabledProjects { get; set; }

        public int Issues { get; set; }
    }
}
