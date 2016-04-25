using System;
using VirtualNote.Common;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.DTO.Query;

namespace VirtualNote.Kernel.DTO.Extensions
{
    internal static class MemberExtensions
    {
        /// <summary>
        ///     Copia um domain member para um DTO member (simples) 
        /// </summary>
        /// <param name="domainMember"></param>
        /// <returns></returns>
        public static MemberServiceDTO CopyToDTO(this Member domainMember)
        {
            return new MemberServiceDTO
            {
                UserID = domainMember.UserID,
                Email = domainMember.Email,
                Enabled = domainMember.Enabled,
                Name = domainMember.Name,
                Phone = domainMember.Phone,
                IsAdmin = domainMember.IsAdmin
            };
        }

        /// <summary>
        ///     Cria um domain member a partir de um DTO member e encripta a password
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static Member CopyToDomainObject(this MemberServiceDTO dto)
        {
            return new Member
            {
                CreatedDate = DateTime.Now,
                Email = dto.Email,
                Enabled = dto.Enabled,
                Name = dto.Name,
                Password = PasswordUtils.Encript(dto.Password),
                Phone = dto.Phone,
                IsAdmin = dto.IsAdmin
            };
        }

        /// <summary>
        ///     Actualiza o domain object com os valores do DTO member
        /// </summary>
        /// <param name="member"></param>
        /// <param name="dto"></param>
        public static void UpdateDomainObjectFromDTO(this Member member,
                                                     MemberServiceDTO dto)
        {
            member.Email = dto.Email;
            member.Enabled = dto.Enabled;
            member.Name = dto.Name;
            member.Phone = dto.Phone;
            member.IsAdmin = dto.IsAdmin;

            if (dto.Password != PasswordUtils.Confuse())
            {
                member.Password = PasswordUtils.Encript(dto.Password);
            }
        }

        /// <summary>
        ///     KeyIdValueString { id = idClient, Value = nameClient }
        /// </summary>
        /// <param name="domainMember"></param>
        /// <returns></returns>
        public static KeyIdValueString ToKeyIdValueString(this Member domainMember) {
            return new KeyIdValueString { Id = domainMember.UserID, Value = domainMember.Name };
        }
    }
}
