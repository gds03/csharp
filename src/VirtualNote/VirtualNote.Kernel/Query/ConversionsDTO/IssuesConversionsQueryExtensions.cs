using System.Collections.Generic;
using System.Linq;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.DTO.Query;
using VirtualNote.Kernel.DTO.Query.Home;
using VirtualNote.Kernel.DTO.Query.Issues.Index;
using VirtualNote.Kernel.DTO.Query.Issues.Index.Common;
using VirtualNote.Kernel.DTO.Query.Issues.Index.Tuple;
using VirtualNote.Kernel.Query.Repository;

namespace VirtualNote.Kernel.Query.ConversionsDTO
{
    internal static class IssuesConversionsQueryExtensions
    {
        
        //
        // Used for Issues
        public static IssueMemberQueryList GetIssuesForMembersOnProjectInSpecificRange(this IQueryable<Issue> query,
            IEnumerable<KeyIdValueString> projects,
            IEnumerable<KeyIdValueString> filters,
            int projectId,
            IssuesSortBy sortBy,
            int currentPage, int take)
        {
            return new IssueMemberQueryList
                   {
                       ProjectsData = new IssueProjectsData
                                      {
                                          Projects = projects,
                                          ProjectSelectedId = projectId,
                                          Filters = filters
                                      },
                       Requests = new IssueRequestsData<IssueMemberQueryTuple>
                                  {
                                      RequestsInfo = new IssueRequestsInfo
                                                     {
                                                         CurrentPage = currentPage,
                                                         Take = take,
                                                         Total = query.Count(i => i.Project.ProjectID == projectId)
                                                     },

                                      Requests = query.Where(i => i.Project.ProjectID == projectId)
                                          .ApplySort(sortBy)
                                          .Skip(( currentPage - 1 )*take)
                                          .Take(take)
                                          .Select(i => new IssueMemberQueryTuple
                                                       {
                                                           IssueId = i.IssueID,
                                                           CreatedAt = i.CreatedDate,
                                                           ShortDescription = i.ShortDescription,
                                                           MemberSolvingName = i.Member != null ? i.Member.Name : null,
                                                           Priority = (PriorityEnum) i.Priority,
                                                           Type = (TypeEnum) i.Type,
                                                           State = (StateEnum) i.State,
                                                           ProjectName = i.Project.Name,
                                                           NumComments = i.Comments.Count(),
                                                           ReportedByClient = i.Client.Name
                                                       })
                                          .ToList()
                                  }
                   };

        }


        public static IssueClientQueryList GetIssuesForClientsOnProjectInSpecificRange(this IQueryable<Issue> query,
            IEnumerable<KeyIdValueString> projects,
            IEnumerable<KeyIdValueString> filters,
            int projectId,
            IssuesSortBy sortBy,
            int currentPage, int take){

            return new IssueClientQueryList
                   {
                       ProjectsData = new IssueProjectsData
                                      {
                                          Projects = projects,
                                          ProjectSelectedId = projectId,
                                          Filters = filters
                                      },
                       Requests = new IssueRequestsData<IssueClientQueryTuple>
                                  {
                                      RequestsInfo = new IssueRequestsInfo
                                                     {
                                                         CurrentPage = currentPage,
                                                         Take = take,
                                                         Total = query.Count(i => i.Project.ProjectID == projectId)
                                                     },
                                      Requests = query.Where(i => i.Project.ProjectID == projectId)
                                                      .ApplySort(sortBy)
                                                      .Skip(( currentPage - 1 )*take)
                                                      .Take(take)
                                                      .Select(i => new IssueClientQueryTuple
                                                                   {
                                                                       IssueId = i.IssueID,
                                                                       CreatedAt = i.CreatedDate,
                                                                       ShortDescription = i.ShortDescription,
                                                                       MemberSolvingName = i.Member != null ? i.Member.Name : null,
                                                                       Priority = (PriorityEnum) i.Priority,
                                                                       Type = (TypeEnum) i.Type,
                                                                       State = (StateEnum) i.State,
                                                                       ProjectName = i.Project.Name,
                                                                       NumComments = i.Comments.Count()
                                                                   })
                                                      .ToList()
                                              }
                   };

        }


        //
        // Used for Home
        public static IEnumerable<HomeMemberQueryDetailsRequests> GetRequests(this IQueryable<Issue> query, int take, IEnumerable<int> myProjectsIds, bool isAdmin)
        {
            int myProjectsCount = myProjectsIds.Count();

            if (!isAdmin && myProjectsCount == 0) {
                // Não há dados para mostrar
                return new List<HomeMemberQueryDetailsRequests>();
            }

            query = query.Where(i => i.Project.Enabled);

            if (!isAdmin) {
                query = query.Where(i => myProjectsIds.Contains(i.Project.ProjectID));
            }

            return query.OrderByDescending(i => i.CreatedDate)
                        .Take(take)
                        .Select(i => new HomeMemberQueryDetailsRequests {
                            IssueId = i.IssueID,
                            State = (StateEnum)i.State,
                            ProjectName = i.Project.Name,
                            Priority = (PriorityEnum)i.Priority,
                            CreatedAt = i.CreatedDate,
                            PostedBy = i.Client.Name,
                            MemberSolving = i.Member != null ? i.Member.Name : null,
                            ShortDescription = i.ShortDescription
                        }).ToList();
        }


        
    }
}
