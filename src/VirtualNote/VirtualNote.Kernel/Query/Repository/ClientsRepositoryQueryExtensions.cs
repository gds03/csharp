using System;
using System.Linq;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.Query.Repository
{
    internal static class ClientsRepositoryQueryExtensions
    {
        /// <summary>
        ///     Devolve o cliente do datasource com o clientId.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="clientId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static Client GetById(this IQueryable<Client> query, 
            int clientId)
        {
            return query.Single(c => c.UserID == clientId);
        }

        


        /// <summary>
        ///     Devolve o cliente do datasource com o clientId e um output parameter
        ///     a indicar se o cliente tem ou não projectos associados 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="clientId"></param>
        /// <param name="result"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static Client GetHasProjects(this IQueryable<Client> query, 
            int clientId, out bool result)
        {
            var value =  query.Where(c => c.UserID == clientId)
                              .Select(c => new { Client = c, ProjectsCount = c.AssignedProjects.Count()} )
                              .Single();

            result = value.ProjectsCount > 0;
            return value.Client;
        }
    }
}
