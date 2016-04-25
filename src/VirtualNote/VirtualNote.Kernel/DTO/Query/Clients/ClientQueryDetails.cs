using System;
using System.Collections.Generic;
using VirtualNote.Kernel.DTO.Query.User;

namespace VirtualNote.Kernel.DTO.Query.Clients
{
    public sealed class ClientQueryDetails : IQueryDTO
    {
        public int ClientId                 { get; set; }
        public UserQueryInfo UserInfo { get; set; }

        public IEnumerable<String> EnabledProjects { get; set; }
        public IEnumerable<String> DisabledProjects { get; set; }

        public int                   Issues { get; set; }
    }
}
