using System;
using System.Collections.Generic;

namespace VirtualNote.Kernel.DTO.Query.Home
{
    public sealed class HomeClientQueryDetails : IQueryDTO
    {
        // Welcome data
        public HomeClientQueryDetailsWelcomeData WelcomeData { get; set; }

        // Lista de estatisticas de issues
        public IEnumerable<HomeClientQueryDetailsIssuesStatistic> IssuesStatistics { get; set; }

        // Lista de requests
        public IEnumerable<HomeCLientQueryDetailsRequests> Requests { get; set; }
    }

    // Welcome data
    public sealed class HomeClientQueryDetailsWelcomeData
    {
        public int Projects { get; set; }
        public int PendingRequests { get; set; }
        public int SupportedRequests { get; set; }
    }


    // Usado para a progressbar
    public sealed class HomeClientQueryDetailsIssuesStatistic
    {
        public string ProjectName { get; set; }
        public int SupportedRequests { get; set; }
        public int TotalRequests { get; set; }
    }

    // Usado para a lista de requests
    public sealed class HomeCLientQueryDetailsRequests
    {
        public int IssueId { get; set; }
        public StateEnum State { get; set; }
        public string ShortDescription { get; set; }
        public string MemberSolving { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
