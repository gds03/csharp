using System.Collections.Generic;
using System.Linq;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.DTO.Query;
using VirtualNote.Kernel.DTO.Query.Projects;

namespace VirtualNote.Kernel.Query.ConversionsDTO
{
    internal static class ProjectsConversionsQueryExtensions
    {
        public static IEnumerable<ProjectQueryList> GetListQueryDTO(this IQueryable<Project> query) {
            return query.Select(p => new ProjectQueryList {
                ProjectId = p.ProjectID,
                ProjectName = p.Name,
                AssignedWorkers = p.Workers.Count(),
                ClientName = p.Client.Name,
                CreatedAt = p.CreatedDate,
                Description = p.Description,
                Enabled = p.Enabled,
                Issues = p.Issues.Count(),
                ResponsableName = p.Responsable.Name
            }).ToList();
        }

        public static ProjectQueryDetails GetDetailsQueryDTO(this IQueryable<Project> query, int projectId) {
            return query.Where(p => p.ProjectID == projectId)
                          .Select(p => new ProjectQueryDetails {
                              ProjectId = p.ProjectID,
                              ProjectName = p.Name,
                              AssignedWorkers = p.Workers.Select(m => m.Name),
                              ClientName = p.Client.Name,
                              CreatedAt = p.CreatedDate,
                              Description = p.Description,
                              Enabled = p.Enabled,
                              Issues = p.Issues.Count(),
                              ResponsableName = p.Responsable.Name
                          }).Single();
        }

        public static IEnumerable<int> GetAllIds(this IQueryable<Project> query) {
            return query.Select(p => p.ProjectID).ToList();
        }

        public static IEnumerable<KeyIdValueString> ToKeyIdValueString(this IQueryable<Project> query){
            return query.Select(p => new KeyIdValueString
                                     {
                                         Id = p.ProjectID,
                                         Value = p.Name
                                     }).ToList();
        }

        public static IEnumerable<KeyIdValueString> ToKeyIdValueString(this IQueryable<Project> query, IEnumerable<int> projectIds) {
            return projectIds.Select(pid => query.Where(p => p.ProjectID == pid)
                                                 .Select(p => new KeyIdValueString
                                                 {
                                                    Id = p.ProjectID,
                                                    Value = p.Name
                                                 }).Single()
            ).ToList();
        }

        public static IEnumerable<KeyIdValueString> ToKeyIdValueString(this IQueryable<Project> query, int clientId) {
            return query.Where(p => p.Client.UserID == clientId)
                        .Select(p => new KeyIdValueString
                        {
                            Id = p.ProjectID,
                            Value = p.Name
                        }).ToList();
        }
    }
}
    