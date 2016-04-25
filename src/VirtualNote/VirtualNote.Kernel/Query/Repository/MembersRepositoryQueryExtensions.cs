using System;
using System.Collections.Generic;
using System.Linq;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.Query.Repository
{
    internal static class MembersRepositoryQueryExtensions
    {
        /// <summary>
        ///     Devolve o membro do datasource com o memberId.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="memberId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static Member GetById(this IQueryable<Member> query,
            int memberId)
        {
            return query.Single(m => m.UserID == memberId);
        }


        /// <summary>
        ///     Devolve o membro do datasource com o memberId e um output parameter
        ///     a indicar se o membro tem ou não projectos associados (worker ou responsable)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="memberId"></param>
        /// <param name="result"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static Member GetHasProjects(this IQueryable<Member> query, 
            int memberId, out bool result)
        {
            var value = query.Where(m => m.UserID == memberId)
                             .Select(m => new {
                                 Member = m,
                                 ResponsableCount = m.Responsabilities.Count(),
                                 WorksOnProjectCount = m.AssignedProjects.Count()
                             }).Single();

            result = value.ResponsableCount > 0 || value.WorksOnProjectCount > 0;
            return value.Member;
        }


        public static IEnumerable<Member> GetAdminsEnabledWithEmail(this IQueryable<Member> query){
            return query.Where(m => m.IsAdmin && m.Enabled && m.Email != null).ToList();
        }
    }
}