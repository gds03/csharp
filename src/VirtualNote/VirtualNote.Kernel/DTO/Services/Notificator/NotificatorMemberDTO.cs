using System;

namespace VirtualNote.Kernel.DTO.Services.Notificator
{
    public sealed class NotificatorMemberDTO : IServiceDTO
    {
        public int IssueId { get; set; }
        public int ClientId { get; set; }
        public int ProjectId { get; set; }

        public string IssueShortDescription { get; set; }
        public StateEnum IssueState { get; set; }

        public string ProjectName { get; set; }
        public string MemberName { get; set; }

        
    }
}
