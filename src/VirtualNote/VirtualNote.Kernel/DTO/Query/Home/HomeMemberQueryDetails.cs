using System.Collections.Generic;

namespace VirtualNote.Kernel.DTO.Query.Home
{
    public sealed class HomeMemberQueryDetails : IQueryDTO
    {
        // Welcome data
        public HomeMemberQueryDetailsWelcomeData WelcomeData { get; set; }

        // Requests data
        public IEnumerable<HomeMemberQueryDetailsRequests> Requests { get; set; }
    }
}
