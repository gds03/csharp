using System.Collections.Generic;

namespace VirtualNote.Kernel.Contracts.Emails
{
    public interface IEmailClientService : IRepositoryService
    {
        /// <summary>
        ///     Atribui as configurações de notificação a um cliente
        /// </summary>
        /// <param name="configs"></param>
        void SetConfigurations(IEnumerable<EmailConfig> configs);
    }
}
