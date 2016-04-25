using System.Collections.Generic;
using VirtualNote.Kernel.Contracts;

namespace VirtualNote.Tests.EmptyServices
{
    public sealed class TestEmailConfigMngr : IEmailConfigMngr
    {
        public IEnumerable<EmailConfig> Find(UserType userType, int userId, out bool hasElement){
            hasElement = false;
            return new List<EmailConfig>();
        }

        public bool Add(UserType userType, int userId, IEnumerable<EmailConfig> values){
            return false;
        }

        public bool Update(UserType userType, int userId, IEnumerable<EmailConfig> values) {
            return false;
        }

        public bool Delete(UserType userType, int userId) {
            return false;
        }
    }
}
