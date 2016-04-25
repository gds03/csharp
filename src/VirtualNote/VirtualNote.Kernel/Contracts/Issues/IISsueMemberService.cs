using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.DTO;

namespace VirtualNote.Kernel.Contracts.Issues
{
    public interface IIssueMemberService : IRepositoryService
    {
        /// <summary>
        ///     Tenta actualizar o estado de um issue
        /// </summary>
        /// <param name="issueMemberDto"></param>
        /// <returns>true se o alterou, false se o estado já está terminated e nao pode mudar o estado</returns>
        /// <exception cref="HijackedException">Quando o membro autenticado nao esta associado ao projecto</exception>
        /// <exception cref="IssueWasAlreadyTakedByAnotherMember">Quando o issue ja foi aceite por outro membro</exception>
        bool ChangeState(IssueServiceMemberDTO issueMemberDto);
    }
}
