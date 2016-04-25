using System;
using VirtualNote.Database;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Notificator;
using VirtualNote.Kernel.DTO.Services.Notificator;

namespace VirtualNote.Kernel.Services.Notificator
{
    public sealed class NotificatorClientService : NotificatorBase, INotificatorClientService
    {
        readonly IEmailConfigMngr _mngr;

        public NotificatorClientService(IRepository db, IEmailConfigMngr emailMngr) : base(db) {
            _mngr = emailMngr;
        }


        public void NotifyMembers(NotificatorClientDTO clientDto)
        {
            if(clientDto == null)
                throw new ArgumentNullException("clientDto");

            string subject = string.Format("{0} reported a new issue on the system with {1} Priority", clientDto.ClientName, clientDto.Priority);
            string message = string.Format("New issue reported at {0} by {1} with {2} priority. \n Subject: {3}. \n\n Description: {4} \n",
                DateTime.Now,
                clientDto.ClientName,
                clientDto.Priority,
                clientDto.ShortDescription,
                clientDto.DetailedDescription
            );

            SendEmailForMembersWithEmailInProject(
                _mngr,
                clientDto.ProjectId,
                subject, message,
                EmailConfig.Member_NewRequestSubmited
           );

        }
    }
}
