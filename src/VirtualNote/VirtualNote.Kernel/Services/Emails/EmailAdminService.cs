using System.Collections.Generic;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Emails;

namespace VirtualNote.Kernel.Services.Emails
{
    public sealed class EmailAdminService : ServiceBase, IEmailAdminService
    {
        readonly IEmailConfigMngr _emailMngr;


        public EmailAdminService(IRepository db, IEmailConfigMngr emailMngr) : base(db){
            _emailMngr = emailMngr;
        }


        public void SetConfigurations(IEnumerable<EmailConfig> configs){
            Member dbAdmin = GetDbAdmin();
            EmailCommonService.SetConfigs(_emailMngr, dbAdmin.UserID, UserType.admin, configs);
        }
    }
}
