using System.Collections.Generic;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Emails;

namespace VirtualNote.Kernel.Services.Emails
{
    public sealed class EmailMemberService : ServiceBase, IEmailMemberService
    {
        readonly IEmailConfigMngr _emailMngr;


        public EmailMemberService(IRepository db, IEmailConfigMngr emailMngr)
            : base(db)
        {
            _emailMngr = emailMngr;
        }


        public void SetConfigurations(IEnumerable<EmailConfig> configs){
            Member dbMember = GetDbMember();
            EmailCommonService.SetConfigs(_emailMngr, dbMember.UserID, UserType.member, configs);
        }
    }
}
