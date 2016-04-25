using VirtualNote.Database;
using VirtualNote.Kernel.Contracts.Notificator;
using VirtualNote.Kernel.DTO.Services.Notificator;

namespace VirtualNote.Tests.EmptyServices.Notificator
{
    public class TestMemberNotificator : INotificatorMemberService
    {
        public void NotifyClientAboutAcceptedRequest(NotificatorMemberDTO memberDto) {
            
        }

        public void NotifyClientAboutInWaitStateAgain(NotificatorMemberDTO memberDto) {
            
        }

        public void NotifyClientAboutTerminateRequest(NotificatorMemberDTO memberDto) {
            
        }

        public void NotifyMembersThatRequestWasAcceptedByAnotherMember(NotificatorMemberDTO memberDto) {
            
        }

        public IRepository Repository {
            get { return null; }
        }
    }
}
