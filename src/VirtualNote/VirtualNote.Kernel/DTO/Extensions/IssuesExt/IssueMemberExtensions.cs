using System;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.DTO.Extensions.IssuesExt
{
    internal static class IssueMemberExtensions
    {
        /// <summary>
        ///     Actualiza o issue com o estado do dto.
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="dto"></param>
        public static void UpdateDomainObjectFromDTO(this Issue issue,
                                                     IssueServiceMemberDTO dto){
            issue.State = (int)dto.State;
            issue.LastUpdateDate = DateTime.Now;
        }
    }
}
