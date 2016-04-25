using System;
using System.Collections.Generic;
using System.Linq;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.DTO.Extensions;
using VirtualNote.Kernel.DTO.Query.Members;
using VirtualNote.Kernel.DTO.Query.User;
using VirtualNote.Kernel.Query.Include;
using VirtualNote.Kernel.Query.Repository;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.Query.ConversionsDTO
{
    internal static class MembersConversionsQueryExtensions
    {
        public static IEnumerable<MemberQueryList> GetListQueryDTO(this IQueryable<Member> query)
        {
            return query.Select(m => new MemberQueryList() {
                MemberId = m.UserID,
                IsAdmin = m.IsAdmin,
                UserInfo = new UserQueryInfo {
                    Name = m.Name,
                    CreatedDate = m.CreatedDate,
                    Email = m.Email,
                    Enabled = m.Enabled,
                    Phone = m.Phone,
                },
                
                ResponsableInProjects = m.Responsabilities.Count(),
                EnabledProjects = m.AssignedProjects.Count(p => p.Enabled),
                DisabledProjects = m.AssignedProjects.Count(p => !p.Enabled),
                IssuesSolved = m.IssuesSolved.Count(),
            })
            .ToList();
        }


        public static MemberServiceDTO GetByIdQueryDTO(this IQueryable<Member> query, int memberId)
        {
            return query.GetById(memberId).CopyToDTO();
        }




        public static MemberQueryDetails GetDetailsQueryDTO(this IQueryable<Member> query, int memberId)
        {
            return query.Where(m => m.UserID == memberId)
                        .Select(m => new MemberQueryDetails {
                            MemberId = m.UserID,
                            IsAdmin = m.IsAdmin,
                            UserInfo = new UserQueryInfo {
                                Name = m.Name,
                                CreatedDate = m.CreatedDate,
                                Email = m.Email,
                                Enabled = m.Enabled,
                                Phone = m.Phone,
                            },
                            ResponsableInProjects = m.Responsabilities.Select(p => p.Name),
                            EnabledProjects = m.AssignedProjects.Where(p => p.Enabled).Select(p => p.Name),
                            DisabledProjects = m.AssignedProjects.Where(p => !p.Enabled).Select(p => p.Name),
                            IssuesSolved = m.IssuesSolved.Count()
                        }).Single();
        }


        public static IEnumerable<int> GetProjectsIdsWhereIamResponsableOrWork(this IQueryable<Member> query,
            int memberId
            )  
        {
            var member = query.Where(m => m.UserID == memberId)
                .Include(m => m.Responsabilities)
                .Include(m => m.AssignedProjects)
                .Single();

            return member.Responsabilities
                .Select(p => p.ProjectID)
                .Concat(member.AssignedProjects.Select(p => p.ProjectID))
                .ToList();

        }

        /// <summary>
        ///     Devolve uma lista de membros com os ids passados por parametro
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ids"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static IEnumerable<Member> GetByIdBundle(this IQueryable<Member> query,
            IEnumerable<int> ids){
            return query.Where(m => ids.Contains(m.UserID)).ToList();
        }


        /// <summary>
        ///     Tipicamente usado para saber se o membro se encontra a trabalhar ou é responsavel no projecto passado por parametro
        /// </summary>
        /// <param name="query"></param>
        /// <param name="memberId"></param>
        /// <param name="projectId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>true se o membro é responsavel ou worker no projectId, caso contrario false</returns>
        public static bool IsThisMemberActiveOnProject(this IQueryable<Member> query,
            int memberId, int projectId) {
            var annon = query.Where(m => m.UserID == memberId)
                             .Select(m => new {
                                 IsResponsable = m.Responsabilities.Any(p => p.ProjectID == projectId),
                                 IsWorker = m.AssignedProjects.Any(p => p.ProjectID == projectId)
                             }).Single();

            return annon.IsResponsable || annon.IsWorker;
        }


        public static int CountAdmins(this IQueryable<Member> query){
            return query.Count(m => m.IsAdmin);
        }
    }
}
