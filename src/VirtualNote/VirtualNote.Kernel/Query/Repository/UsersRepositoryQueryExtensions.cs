using System;
using System.Linq;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.Query.Repository
{
    internal static class UsersRepositoryQueryExtensions
    {
        /// <summary>
        ///     Devolve um User dado o seu nome
        /// </summary>
        /// <param name="query"></param>
        /// <param name="username"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static User GetByName(this IQueryable<User> query, String username){
            username = username.Trim().ToLower();
            return query.SingleOrDefault(u => u.Name.Trim().ToLower() == username);
        }


        /// <summary>
        ///     Devolve um User dado o seu id
        /// </summary>
        /// <param name="query"></param>
        /// <param name="userId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static User GetById(this IQueryable<User> query, int userId)
        {
            return query.SingleOrDefault(u => u.UserID == userId);
        }



    }
}
