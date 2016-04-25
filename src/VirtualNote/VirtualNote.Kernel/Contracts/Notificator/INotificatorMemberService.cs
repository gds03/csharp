using VirtualNote.Kernel.DTO.Services.Notificator;

namespace VirtualNote.Kernel.Contracts.Notificator
{
    public interface INotificatorMemberService : IRepositoryService
    {
        /// <summary>
        ///     Notifica o cliente que o pedido foi aceite por um membro
        /// </summary>
        /// <param name="memberDto"></param>
        void NotifyClientAboutAcceptedRequest(NotificatorMemberDTO memberDto);


        /// <summary>
        ///     Notifica o cliente que o pedido voltou ao estado inicial 
        /// </summary>
        /// <param name="memberDto"></param>
        void NotifyClientAboutInWaitStateAgain(NotificatorMemberDTO memberDto);

        /// <summary>
        ///     Notifica o cliente que o pedido está terminado
        /// </summary>
        /// <param name="memberDto"></param>
        void NotifyClientAboutTerminateRequest(NotificatorMemberDTO memberDto);


        /// <summary>
        ///     Notifica os membros do projecto que algum membro procedeu a resolução
        ///     de um issue.
        /// </summary>
        /// <param name="memberDto"></param>
        void NotifyMembersThatRequestWasAcceptedByAnotherMember(NotificatorMemberDTO memberDto);
    }
}
