using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Query.Repository;
using VirtualNote.Kernel.Types;

namespace VirtualNote.Kernel.Services.Notificator
{
    public class NotificatorBase : ServiceBase
    {
        public NotificatorBase(IRepository repository) : base(repository) {
            
        }

        protected void SendEmailForMembersWithEmailInProject(IEmailConfigMngr mngr, int projectId,
            string subject, string messageBody,
            EmailConfig configType) 
        {
            var membersEnabledForTheProject = _db.Query<Project>()
                                                    .GetEnabledResponsableAndWorkersForProjectWithEmailFromProjectId(projectId);

            var adminsEnabled = _db.Query<Member>().GetAdminsEnabledWithEmail();
            User dbUser = CurrentUser;

            Thread t = new Thread(() => 
            {
                // Normalizar, dos workers, pode haver membros repetidos com admins => Union
                var collection = membersEnabledForTheProject.Union(adminsEnabled,
                    new LambdaComparer<Member>((m1, m2) => m1.UserID == m2.UserID));

                //
                // Este método pode ser chamado pelos serviços do cliente e do membro.
                // O Cliente invoca este metodo quando submete um pedido para informar os membros
                // O membro invoca o método para avisar os membros da progressão..
                // => Se o utilizador que invoca é membro então o endereço de email dele tem que ser removido
                //    da lista de endereços.
                Member dbMember;
                if ((dbMember = dbUser as Member) != null) {
                    collection = collection.Except(new[] { dbMember });
                }

                List<string> emailAddresses = new List<string>();

                foreach (var member in collection) {
                    // Iterar sobre os membros que teem email e verificar se tem config de 
                    // quando membro aceita pedido
                    bool hasElement;

                    UserType type = member.IsAdmin ? UserType.admin : UserType.member;
                    var configs = mngr.Find(type, member.UserID, out hasElement);

                    // Verificar se o elemento existe e se existe verificar se o membro pretende que 
                    // o sistema envie o email
                    if (hasElement && configs.Any(c => c == configType)) {
                        emailAddresses.Add(member.Email);
                    }
                }

                if (emailAddresses.Count > 0) {
                    SendEmailToMembers(emailAddresses, subject, messageBody);
                }
            });
            
            t.Start();
        }

        private static void SendEmail(string subject, string message, params string[] addresses)
        {
            MailMessage email = new MailMessage();

            foreach (var address in addresses) {
                email.To.Add(address);
            }

            email.SubjectEncoding = Encoding.ASCII;
            email.BodyEncoding = Encoding.ASCII;

            email.Subject = subject;
            email.Body = message;


            SmtpClient client = new SmtpClient();

            try {
                client.SendAsync(email, null);
            }catch(SmtpException ex){
                if (ex.InnerException.GetType() != typeof(System.Net.WebException))
                    throw;
                // Senão ignora
                // pode não haver ligação para enviar emails..
            }
        }

        public void SendEmailToMembers(IEnumerable<string> membersddresses, string subject, string message){
            SendEmail(subject, message, membersddresses.ToArray());
        }
        public void SendEmailToClient(string clientAddress, string subject, string message){
            SendEmail(subject, message, clientAddress);
        }
    }
}
