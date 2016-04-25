using System.Collections.Generic;
using VirtualNote.Kernel.Contracts;

namespace VirtualNote.Kernel.Services.Emails
{
    public static class EmailCommonService
    {
        public static void SetConfigs(IEmailConfigMngr mngr, int userId, UserType type, IEnumerable<EmailConfig> configs){
            bool hasElement;

            mngr.Find(type, userId, out hasElement);

            if (!hasElement) {
                mngr.Add(type, userId, configs);
                return;
            }

            mngr.Update(type, userId, configs);
        }
    }
}
