using System;
using System.Collections.Generic;
using System.Linq;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.DTO.Extensions;
using VirtualNote.Kernel.DTO.Query.Clients;
using VirtualNote.Kernel.DTO.Query.User;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Query.Repository;

namespace VirtualNote.Kernel.Query.ConversionsDTO
{
    internal static class ClientsConversionsQueryExtensions
    {
        public static IEnumerable<ClientQueryList> GetListQueryDTO(this IQueryable<Client> query)
        {
            return query.Select(c => new ClientQueryList()
            {
                ClientId = c.UserID,
                UserInfo = new UserQueryInfo
                               {
                                   Name = c.Name,
                                   CreatedDate = c.CreatedDate,
                                   Email = c.Email,
                                   Enabled = c.Enabled,
                                   Phone = c.Phone,
                               },
                Issues = c.IssuesReported.Count(),
                EnabledProjects = c.AssignedProjects.Count(p => p.Enabled),
                DisabledProjects = c.AssignedProjects.Count(p => !p.Enabled)
            })
            .ToList();
        }

        public static ClientServiceDTO GetByIdQueryDTO(this IQueryable<Client> query, int clientId)
        {
            return query.GetById(clientId).CopyToDTO();
        }

        public static ClientQueryDetails GetDetailsQueryDTO(this IQueryable<Client> query, int clientId)
        {
            return query.Where(c => c.UserID == clientId)
                        .Select(c => new ClientQueryDetails
                        {
                            ClientId = c.UserID,
                            UserInfo = new UserQueryInfo {
                                Name = c.Name,
                                CreatedDate = c.CreatedDate,
                                Email = c.Email,
                                Enabled = c.Enabled,
                                Phone = c.Phone,
                            },
                            Issues = c.IssuesReported.Count(),
                            EnabledProjects = c.AssignedProjects.Where(p => p.Enabled).Select(p => p.Name),
                            DisabledProjects = c.AssignedProjects.Where(p => !p.Enabled).Select(p => p.Name)
                        }).Single();
        }

        /// <summary>
        ///     Devolve uma lista de inteiros que representam os ids dos projectos
        ///     que o cliente tem no sistema.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="clientId"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IEnumerable<int> GetMyProjectsIds(this IQueryable<Client> query,
            int clientId) 
        {
            return query.Where(c => c.UserID == clientId)
                        .Select(c => c.AssignedProjects
                                      .Select(p => p.ProjectID)
                         ).FirstOrDefault();
        }
    }
}
