using VirtualNote.Database;
using VirtualNote.Kernel.Contracts.Notificator;
using VirtualNote.Kernel.DTO.Services.Notificator;

namespace VirtualNote.Tests.EmptyServices.Notificator
{
    public class TestClientNotificator : INotificatorClientService
    {
        public void NotifyMembers(NotificatorClientDTO clientDto) {
            
        }

        public IRepository Repository{
            get { return null; }
        }
    }
}
