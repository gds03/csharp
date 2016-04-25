using System.Collections.Generic;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Emails;

namespace VirtualNote.Kernel.Services.Emails
{
    public sealed class EmailClientService : ServiceBase, IEmailClientService
    {
        readonly IEmailConfigMngr _emailMngr;


        public EmailClientService(IRepository db, IEmailConfigMngr emailMngr)
            : base(db){
            _emailMngr = emailMngr;
        }


        public void SetConfigurations(IEnumerable<EmailConfig> configs){
            Client dbClient = GetDbClient();
            EmailCommonService.SetConfigs(_emailMngr, dbClient.UserID, UserType.client, configs);
        }
    }
}
