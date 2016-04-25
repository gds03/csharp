using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.DTO.Services.Notificator;

namespace VirtualNote.Kernel.Services.Issues
{
    static class IssueMembersCommon
    {
        // 
        // Porque o Membro pode ser null, passamos o membro que está a alterar o issue por parametro.

        public static NotificatorMemberDTO BuildNotificatorForIssue(Issue dbIssue, Member currentMember) {
            return new NotificatorMemberDTO {
                ClientId = dbIssue.Client.UserID,
                IssueId = dbIssue.IssueID,
                ProjectId = dbIssue.Project.ProjectID,

                IssueShortDescription = dbIssue.ShortDescription,
                IssueState = (StateEnum)dbIssue.State,
                MemberName = currentMember.Name,
                ProjectName = dbIssue.Project.Name
            };
        }
    }
}
