using VirtualNote.Kernel.DTO.Services.Notificator;

namespace VirtualNote.Kernel.Contracts.Notificator
{
    public interface INotificatorClientService : IRepositoryService
    {
        /// <summary>
        ///     Notifica os trabalhadores e responsavel do projecto e todos os admins, se e so se, 
        ///     cada um deles tiver essa condição no ficheiro de configuração
        /// </summary>
        /// <param name="clientDto"></param>
        void NotifyMembers(NotificatorClientDTO clientDto);
    }
}
