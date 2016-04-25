using System;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.DTO.Extensions.IssuesExt
{
    internal static class IssueClientExtensions
    {
        /// <summary>
        ///     Cria um domain issue a partir de um DTO issue recebendo por parametro os objectos
        ///     obrigatorios (Client e Project)
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="project"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Issue CopyToDomainObject(this IssueServiceClientDTO dto, 
            Project project, Client client)
        {
            return new Issue {
                CreatedDate = DateTime.Now,
                LongDescription = dto.LongDescription,
                ShortDescription = dto.ShortDescription,
                Type = (int)dto.Type,
                Priority = (int)dto.Priority,
                State = (int)StateEnum.Waiting,
                Project = project,
                Client = client
            };
        }

        /// <summary>
        ///     Actualiza o domain object com os valores do DTO issue, 
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="dto"></param>
        public static void UpdateDomainObjectFromDTO(this Issue issue, 
                                                     IssueServiceClientDTO dto)
        {
            issue.LastUpdateDate = DateTime.Now;
            issue.LongDescription = dto.LongDescription;
            issue.ShortDescription = dto.ShortDescription;
            issue.Type = (int)dto.Type;
            issue.Priority = (int)dto.Priority;
        }
    }
}
