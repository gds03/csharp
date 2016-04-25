using System.Security.Principal;
using System.Threading;

namespace VirtualNote.Tests
{
    class TemporaryPrincipal : IPrincipal
    {
        readonly GenericIdentity _identity;

        public TemporaryPrincipal(string name)
        {
            _identity = new GenericIdentity(name);
            Thread.CurrentPrincipal = this;
        }

        public bool IsInRole(string role)
        {
            return false;
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }
    }
}
