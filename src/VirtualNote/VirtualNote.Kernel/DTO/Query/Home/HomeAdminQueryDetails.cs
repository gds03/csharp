using System;
using System.Collections.Generic;

namespace VirtualNote.Kernel.DTO.Query.Home
{
    public sealed class HomeAdminQueryDetails : IQueryDTO
    {
        // Welcome data
        public HomeMemberQueryDetailsWelcomeData WelcomeData { get; set; }

        // Configs data
        public HomeAdminQueryDetailsConfigData ConfigsData { get; set; }

        // Requests data
        public IEnumerable<HomeMemberQueryDetailsRequests> Requests { get; set; }
    }

    // Welcome data
    public sealed class HomeMemberQueryDetailsWelcomeData
    {
        public int ProjectsResponsable { get; set; }
        public int ProjectsWorking { get; set; }
        public int PendingRequests { get; set; }
        public int SupportedRequests { get; set; }
    }

    // Configs data
    public sealed class HomeAdminQueryDetailsConfigData
    {
        public int ClientsSubscribed { get; set; }
        public int ClientsSubscribedEnabled { get; set; }

        public int MembersRegistered { get; set; }
        public int MembersRegisteredEnabled { get; set; }

        public int ProjectsRegistered { get; set; }
        public int ProjectsRegisteredEnabled { get; set; }
    }

    // Usado para a lista de requests
    public sealed class HomeMemberQueryDetailsRequests
    {
        public int IssueId { get; set; }
        public StateEnum State { get; set; }
        public string ProjectName { get; set; }
        public PriorityEnum Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PostedBy { get; set; }
        public string MemberSolving { get; set; }
        public string ShortDescription { get; set; }
    }
}
