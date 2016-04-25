using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.DTO;

namespace VirtualNote.Kernel.Contracts
{
    public interface IMembersService : IRepositoryService
    {
        /// <summary>
        ///     Tenta inserir um membro na bd.
        /// </summary>
        /// <param name="memberDto"></param>
        /// <returns>true se inseriu com sucesso, false se existe um utilizador com o mesmo nome </returns>
        bool Add(MemberServiceDTO memberDto);


        /// <summary>
        ///     Tenta actualizar um membro da bd.
        /// </summary>
        /// <param name="memberDto"></param>
        /// <returns>true se actualizou com sucesso, false se existe um utilizador com o mesmo nome</returns>
        bool Update(MemberServiceDTO memberDto);


        /// <summary>
        ///     Tenta apagar um membro da bd
        ///     Se retornou false o membro ficou disabled
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="memberName"></param>
        /// <exception cref="MemberLastAdminException">When the removed member is the last admin, and canoot be removed for access purposes</exception>
        /// <returns>true se removeu com sucesso, false se o membro está associado a um projecto (responsavel ou trabalhador) e não pode ser removido</returns>
        bool Remove(int memberId, out string memberName);
    }
}
