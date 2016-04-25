using System;
using System.Collections.Generic;
using System.Linq;
using VirtualNote.Kernel.Query.Include;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.Query.Repository
{
    internal static class IssuesRepositoryQueryExtensions
    {
        static readonly IDictionary<IssuesSortBy, Func<IQueryable<Issue>, IOrderedQueryable<Issue>>> Cache =
            new Dictionary<IssuesSortBy, Func<IQueryable<Issue>, IOrderedQueryable<Issue>>>();

        static IssuesRepositoryQueryExtensions() 
        {
            //
            // Quando o construtor de tipo é chamado adiciona na cache dado o valor do enumerado a expressao de ordenação
            Cache.Add(IssuesSortBy.DescendingState, query => query.OrderByDescending(i => i.State));
            Cache.Add(IssuesSortBy.DescendingDate, query => query.OrderByDescending(i => i.CreatedDate));
            Cache.Add(IssuesSortBy.DescendingPriority, query => query.OrderByDescending(i => i.Priority));
            Cache.Add(IssuesSortBy.DescendingType, query => query.OrderByDescending(i => i.Type));

            Cache.Add(IssuesSortBy.AscendingState, query => query.OrderBy(i => i.State));
            Cache.Add(IssuesSortBy.AscendingDate, query => query.OrderBy(i => i.CreatedDate));
            Cache.Add(IssuesSortBy.AscendingPriority, query => query.OrderBy(i => i.Priority));
            Cache.Add(IssuesSortBy.AscendingType, query => query.OrderBy(i => i.Type));
        }


        /// <summary>
        ///     Aplica sorts de um enumerado num IQueryable
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="sortBy">A coluna que se pretende ordenar</param>
        /// <returns></returns>
        public static IOrderedQueryable<Issue> ApplySort(this IQueryable<Issue> issue,
            IssuesSortBy sortBy) 
        {
            return Cache[sortBy](issue);
        }



        /// <summary>
        ///     Devolve o issue, o cliente que o reportou, o membro que o resolve, e o projecto a que
        ///     o issue está associado dado o seu id.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="issueId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Issue GetByIdIncludeAll(this IQueryable<Issue> query, 
            int issueId)
        {
            return query.Where(i => i.IssueID == issueId)
                        .Include(i => i.Client)
                        .Include(i => i.Project)
                        .Include(i => i.Member)
                        .Single();
        }


        /// <summary>
        ///     Devolve o issue, o cliente que o reportou, o membro que o resolve, o projecto a que
        ///     o issue está associado e os comentarios dado o seu id.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="issueId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Issue GetByIdIncludeAllPlusComments(this IQueryable<Issue> query, 
            int issueId) 
        {
            return query.Where(i => i.IssueID == issueId)
                        .Include(i => i.Client)
                        .Include(i => i.Project)
                        .Include(i => i.Member)
                        .Include(i => i.Comments)
                        .Single();
        }

        /// <summary>
        ///     Devolve o issue, o cliente que o reportou, o membro que o resolve, e o projecto a que
        ///     o issue está associado dado o seu id e um output parameter a indicar o numero de comentarios que o isse tem.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="issueId"></param>
        /// <param name="numComments"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Issue GetByIdIncludeAll(this IQueryable<Issue> query, 
            int issueId, out int numComments)
        {
            var issue = query.Where(i => i.IssueID == issueId)
                        .Include(i => i.Client)
                        .Include(i => i.Project)
                        .Include(i => i.Member)
                        .Single();

            numComments = query.Where(i => i.IssueID == issueId)
                               .Select(i => i.Comments)
                               .Count();
            return issue;
        } 
    }
}