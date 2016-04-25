using System;
using System.Collections.Generic;
using System.Linq;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Query.Include;

namespace VirtualNote.Kernel.Query.Repository
{
    internal static class ProjectsRepositoryQueryExtensions
    {
        /// <summary>
        ///     Devolve o projecto dado o nome
        /// </summary>
        /// <param name="query"></param>
        /// <param name="name"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static Project GetByName(this IQueryable<Project> query, 
            string name)
        {
            name = name.ToLower();
            return query.SingleOrDefault(p => p.Name.ToLower() == name);
        }

        /// <summary>
        ///     Devolve o projecto dado o seu id
        /// </summary>
        /// <param name="query"></param>
        /// <param name="projectId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Project GetById(this IQueryable<Project> query, 
            int projectId)
        {
            return query.Single(p => p.ProjectID == projectId);
        }

        /// <summary>
        ///     Devolve o projecto, o responsavel e os workers dado o seu id.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="projectId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Project GetByIdIncludeResponsableAndWorkers(this IQueryable<Project> query,
            int projectId)
        {
            return query.Where(p => p.ProjectID == projectId)
                        .Include(p => p.Responsable)
                        .Include(p => p.Workers)
                        .Single();
        }

        /// <summary>
        ///     Devolve o projecto e o cliente dado o seu id
        /// </summary>
        /// <param name="query"></param>
        /// <param name="projectId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Project GetByIdIncludeClient(this IQueryable<Project> query, 
            int projectId)
        {
            return query.Where(p => p.ProjectID == projectId)
                        .Include(p => p.Client)
                        .SingleOrDefault();
        }


        /// <summary>
        ///     Devolve o projecto e um output parameter a indicar se o projecto tem issues
        /// </summary>
        /// <param name="query"></param>
        /// <param name="projectId"></param>
        /// <param name="result"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Project GetHasIssues(this IQueryable<Project> query, int projectId, out bool result)
        {
            var value =  query.Where(p => p.ProjectID == projectId)
                              .Select(p => new { Project = p, IssuesCount = p.Issues.Count() })
                              .Single();

            result = value.IssuesCount > 0;
            return value.Project;
        }

        /// <summary>
        ///     Devolve os membros que estão activos e que estão associados a determinado projecto
        /// </summary>
        /// <param name="query"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static IEnumerable<Member> GetEnabledResponsableAndWorkersForProjectWithEmailFromProjectId(this IQueryable<Project> query, int projectId) {
            //return query.Where(p => p.ProjectID == projectId)
            //            .Select(p => p.Workers.Where(m => m.Enabled).Concat(new List<Member> { p.Responsable }))
            //            .Single();

            var annon = query.Where(p => p.ProjectID == projectId)
                             .Select(p => new {
                                 EmailWorkers = p.Workers.Where(m => m.Enabled && m.Email != null),
                                 Responsable = p.Responsable
                             }).Single();

            var list = annon.EmailWorkers.ToList();

            if(annon.Responsable.Email != null)
                list.Add(annon.Responsable);

            return list;
        }
    }
}