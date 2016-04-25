using System;
using System.Linq;
using VirtualNote.Kernel.Query.Include;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.Query.Repository
{
    internal static class CommentsRepositoryQueryExtensions
    {
        /// <summary>
        ///     Devolve o comentario e o user que o reportou, dado o seu id
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commentId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Comment GetById(this IQueryable<Comment> query, 
            int commentId)
        {
            return query.Where(c => c.CommentID == commentId)
                        .Include(c => c.User)
                        .Single();
        }


        /// <summary>
        ///     Devolve o comentario, o user que o reportou, e o issue a que pertence, dado o seu id
        /// </summary>
        /// <param name="query"></param>
        /// <param name="commentId"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Comment GetByIdIncludeAll(this IQueryable<Comment> query, 
            int commentId) 
        {
            return query.Where(c => c.CommentID == commentId)
                        .Include(c => c.User)
                        .Include(c => c.Issue)
                        .Single();
        }
    }
}