using System.Collections.Generic;

namespace VirtualNote.Kernel.Contracts.Emails
{
    public interface IEmailAdminService : IRepositoryService
    {
        /// <summary>
        ///     Atribui as configurações de notificação a um admin
        /// </summary>
        /// <param name="configs"></param>
        void SetConfigurations(IEnumerable<EmailConfig> configs);
    }
}
