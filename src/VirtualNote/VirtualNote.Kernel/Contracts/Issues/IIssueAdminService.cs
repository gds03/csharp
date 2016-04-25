using VirtualNote.Kernel.DTO;

namespace VirtualNote.Kernel.Contracts.Issues
{
    public interface IIssueAdminService : IRepositoryService
    {
        /// <summary>
        ///     Tenta actualiza o estado de um issue
        /// </summary>
        /// <param name="issueMemberDto"></param>
        /// <returns>true se o alterou, false se o estado já está terminated e nao pode mudar o estado</returns>
        bool ChangeState(IssueServiceMemberDTO issueMemberDto);


        /// <summary>
        ///     Apaga um determinado issue.
        ///     Note: este método remove os comentarios associados ao Issue.
        /// </summary>
        /// <param name="issueId"></param>
        void Remove(int issueId);
    }
}
