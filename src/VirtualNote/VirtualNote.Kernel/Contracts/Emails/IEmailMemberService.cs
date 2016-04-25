using System.Collections.Generic;

namespace VirtualNote.Kernel.Contracts.Emails
{
    public interface IEmailMemberService : IRepositoryService
    {
        /// <summary>
        ///     Atribui as configurações de notificação a um membro
        /// </summary>
        /// <param name="configs"></param>
        void SetConfigurations(IEnumerable<EmailConfig> configs);
    }
}
