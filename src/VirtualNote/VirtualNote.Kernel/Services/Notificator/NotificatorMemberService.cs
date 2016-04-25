using System;
using System.Linq;
using System.Threading;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Notificator;
using VirtualNote.Kernel.DTO.Services.Notificator;
using VirtualNote.Kernel.Query.Repository;

namespace VirtualNote.Kernel.Services.Notificator
{
    public sealed class NotificatorMemberService : NotificatorBase, INotificatorMemberService
    {
        readonly IEmailConfigMngr _mngr;        

        public NotificatorMemberService(IRepository db, IEmailConfigMngr emailMngr) : base(db) {
            _mngr = emailMngr;
        }


        private void NotifyClientAux(NotificatorMemberDTO memberDto,  EmailConfig config,
            string subject, string message){
            if(memberDto == null)
                throw new ArgumentNullException("memberDto");

            Client dbClient = _db.Query<Client>().GetById(memberDto.ClientId);
            Thread t = new Thread(() => 
            {
                if (dbClient.Enabled && dbClient.Email != null) {
                    // Pesquisar opcoes do cliete no xmlFile

                    bool hasElement;
                    var configs = _mngr.Find(UserType.client, memberDto.ClientId, out hasElement);

                    if (hasElement && configs.Any(c => c == config)) {
                        SendEmailToClient(dbClient.Email, subject, message);
                    }
                }                          
            });

            t.Start();
        }


        public void NotifyClientAboutAcceptedRequest(NotificatorMemberDTO memberDto)
        {
            string subject = string.Format("Request accepted with id {0} by {1} on project {2}", memberDto.IssueId, memberDto.MemberName, memberDto.ProjectName);
            string message = string.Format("{0} started to solving your issue at {1}. \n\n Subject: {2} \n  Project: {3} \n",
                                           memberDto.MemberName,
                                           DateTime.Now,
                                           memberDto.IssueShortDescription,
                                           memberDto.ProjectName
                );

            NotifyClientAux(memberDto, EmailConfig.Client_RequestAccepted, subject, message);
        }

        public void NotifyClientAboutInWaitStateAgain(NotificatorMemberDTO memberDto) {
            string subject = string.Format("Request with id {0} is on waiting state again on project {1}", 
                memberDto.IssueId, 
                memberDto.ProjectName
            );

            string message = string.Format("{0} setted your issue at {1} in wait state. \n\n Subject: {2} \n  Project: {3} \n",
                                           memberDto.MemberName,
                                           DateTime.Now,
                                           memberDto.IssueShortDescription,
                                           memberDto.ProjectName
                );
            
            NotifyClientAux(memberDto, EmailConfig.Client_RequestWaitingStateAgain, subject, message);
        }

        public void NotifyClientAboutTerminateRequest(NotificatorMemberDTO memberDto) {
            string subject = string.Format("Request with id {0} was terminated on project {1}",
                memberDto.IssueId,
                memberDto.ProjectName
            );

            string message = string.Format("{0} terminated your issue at {1}. \n\n Subject: {2} \n  Project: {3} \n",
                                           memberDto.MemberName,
                                           DateTime.Now,
                                           memberDto.IssueShortDescription,
                                           memberDto.ProjectName
                );

            NotifyClientAux(memberDto, EmailConfig.Client_RequestTerminated, subject, message);
        }





        public void NotifyMembersThatRequestWasAcceptedByAnotherMember(NotificatorMemberDTO memberDto) {
            if (memberDto == null)
                throw new ArgumentNullException("memberDto");

            string subject = string.Format("Request with id {0} for project {1} was accepted by {2}",
                       memberDto.IssueId,
                       memberDto.ProjectName,
                       memberDto.MemberName
                   );

            string message = string.Format("{0} accepted reported issue at {1}. \n\n Subject: {2} \n  Project: {3} \n",
                memberDto.MemberName,
                DateTime.Now,
                memberDto.IssueShortDescription,
                memberDto.ProjectName
            );

            SendEmailForMembersWithEmailInProject(
                _mngr, 
                memberDto.ProjectId, 
                subject, message, 
                EmailConfig.Member_RequestAcceptedByOtherMember
           );
        }
    }
}
