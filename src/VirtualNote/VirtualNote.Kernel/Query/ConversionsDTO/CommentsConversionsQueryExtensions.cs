using System;
using System.Linq;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.DTO.Query.Comments;
using VirtualNote.Kernel.Types;

namespace VirtualNote.Kernel.Query.ConversionsDTO
{
    public static class CommentsConversionsQueryExtensions
    {
        public static CommentQueryList GetCommentsForIssueInSpecificRange(this IQueryable<Comment> query, 
            User currentUser,
            int issueId, int currentPage, int take)
        {
            Member m;
            bool isCurrentUserAdmin = false;
            if( (m = currentUser as Member) != null)
                isCurrentUserAdmin = m.IsAdmin;

            var data = query.OrderBy(c => c.CreatedDate)
                            .Where(c => c.Issue.IssueID == issueId)
                            .Skip((currentPage - 1) * take)
                            .Take(take)
                            .Select(c => new CommentQueryDetails
                                         {
                                             CommentId = c.CommentID,
                                             CreatedAt = c.CreatedDate,
                                             Description = c.Description,
                                             LastUpdateAt = c.LastUpdateDate,
                                             ReportedBy = c.User.Name,
                                             CanEdit = c.User is Member && isCurrentUserAdmin || c.User.UserID == currentUser.UserID
                                         })
                            .ToList();

            var total = query.Where(c => c.Issue.IssueID == issueId).Count();

            var annon = query.Where(c => c.Issue.IssueID == issueId)
                             .Select(c => new {
                                 createdAt = c.Issue.CreatedDate,
                                 reportedBy = c.Issue.Client.Name,
                                 memberSolving = c.Issue.Member,
                                 description = c.Issue.ShortDescription
                             })
                             .First();

            return new CommentQueryList
            {
                Data = new PaginatedList<CommentQueryDetails>(data, currentPage, take, total),
                IssueShortDescription = annon.description,
                IssueMemberSolving = annon.memberSolving != null ? annon.memberSolving.Name : "",
                IssueCreatedAt = annon.createdAt,
                IssueReportedBy = annon.reportedBy
            };
        }
    }
}
