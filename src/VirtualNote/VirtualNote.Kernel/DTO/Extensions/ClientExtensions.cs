using System;
using System.Collections.Generic;
using VirtualNote.Common;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.DTO.Query;

namespace VirtualNote.Kernel.DTO.Extensions
{
    internal static class ClientExtensions
    {
        /// <summary>
        ///     Copia um domain client para um DTO client (simples)
        /// </summary>
        /// <param name="domainClient"></param>
        /// <returns></returns>
        public static ClientServiceDTO CopyToDTO(this Client domainClient)
        {
            return new ClientServiceDTO
            {
                UserID = domainClient.UserID,
                Email = domainClient.Email,
                Enabled = domainClient.Enabled,
                Name = domainClient.Name,
                Phone = domainClient.Phone
            };
        }

        /// <summary>
        ///     Cria um domain client a partir de um DTO client e encripta a password
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static Client CopyToDomainObject(this ClientServiceDTO dto)
        {
            return new Client
            {
                CreatedDate = DateTime.Now,
                Email = dto.Email,
                Enabled = dto.Enabled,
                Name = dto.Name,
                Phone = dto.Phone,
                Password = PasswordUtils.Encript(dto.Password),
            };
        }

        /// <summary>
        ///     Actualiza o domain object com os valores do DTO client
        /// </summary>
        /// <param name="domainClient"></param>
        /// <param name="dto"></param>
        public static void UpdateDomainObjectFromDTO(this Client domainClient, 
                                                     ClientServiceDTO dto)
        {
            domainClient.Email = dto.Email;
            domainClient.Enabled = dto.Enabled;
            domainClient.Name = dto.Name;
            domainClient.Phone = dto.Phone;

            if (dto.Password != PasswordUtils.Confuse())
            {
                domainClient.Password = PasswordUtils.Encript(dto.Password);
            }
        }

        /// <summary>
        ///     KeyIdValueString { id = idClient, Value = nameClient }
        /// </summary>
        /// <param name="domainClient"></param>
        /// <returns></returns>
        public static KeyIdValueString ToKeyIdValueString(this Client domainClient){
            return new KeyIdValueString { Id = domainClient.UserID, Value = domainClient.Name };
        }

        public static List<KeyIdValueString> ToKeyIdValueStringList(this Client domainClient){
            return new List<KeyIdValueString>
                       {
                           new KeyIdValueString
                           {
                               Id = domainClient.UserID,
                               Value = domainClient.Name
                           }
                       };
        }

    }
}
