using System;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.DTO.Extensions
{
    internal static class ProjectsExtensions
    {
        /// <summary>
        ///     Copia um domain project para um DTO project (simples) 
        /// </summary>
        /// <param name="domainProject"></param>
        /// <returns></returns>
        public static ProjectServiceDTO CopyToDTO(this Project domainProject)
        {
            return new ProjectServiceDTO
                       {
                           ProjectID = domainProject.ProjectID,
                           Name = domainProject.Name,                           
                           Description = domainProject.Description,
                           Enabled = domainProject.Enabled,
                           ClientId = domainProject.Client.UserID,
                           ResponsableId = domainProject.Responsable.UserID
                       };
        }

        /// <summary>
        ///     Cria um domain project a partir de um DTO project recebendo 
        ///     por parametro os objectos obrigatorios (responsavel e o cliente)
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="domainResponsable"></param>
        /// <param name="domainMember"></param>
        /// <returns></returns>
        public static Project CopyToDomainObject(this ProjectServiceDTO dto, 
                                                 Member domainResponsable, Client domainMember) {
            return new Project
                       {
                           Description = dto.Description,
                           CreatedDate = DateTime.Now,
                           Enabled = dto.Enabled,
                           Name = dto.Name,
                           Responsable = domainResponsable,
                           Client = domainMember
                       };
        }

        /// <summary>
        ///     Actualiza o domain object com os valores do DTO project recebendo 
        ///     por parametro os objectos obrigatorios (responsavel e o cliente)
        /// </summary>
        /// <param name="domainProject"></param>
        /// <param name="dto"></param>
        /// <param name="domainResponsable"></param>
        /// <param name="domainClient"></param>
        public static void UpdateDomainObject(this Project domainProject, 
                                              ProjectServiceDTO dto, 
                                              Member domainResponsable, Client domainClient)
        {
            domainProject.Name = dto.Name;
            domainProject.Enabled = dto.Enabled;
            domainProject.Description = dto.Description;

            domainProject.Responsable = domainResponsable;
            domainProject.Client = domainClient;
        }
    }
}
