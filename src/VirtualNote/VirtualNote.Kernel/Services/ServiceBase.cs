using System;
using System.Threading;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.Query.Repository;

namespace VirtualNote.Kernel.Services
{
    public class ServiceBase
    {
        protected readonly IRepository _db;
        private User _currentUser = null;

        protected ServiceBase(IRepository db)
        {
            if(db == null)
                throw new ArgumentNullException("db");

            _db = db;
        }

        protected User CurrentUser
        {
            get
            {
                if (_currentUser == null || (_currentUser.Name != Thread.CurrentPrincipal.Identity.Name) )
                {
                    string userName = Thread.CurrentPrincipal.Identity.Name;
                    _currentUser = _db.Query<User>().GetByName(userName);
                }
                return _currentUser;
            }
        }

        public IRepository Repository
        {
            get { return _db; }
        }

        protected Client GetDbClient() {
            User dbUser = CurrentUser;
            Client dbClient;

            // Apenas clientes podem aceder ao servico
            if (dbUser == null || (dbClient = dbUser as Client) == null)
                throw new ServiceAccessDeniedException("You must be a client to make this operation");

            return dbClient;
        }

        protected Member GetDbMember() {
            User dbUser = CurrentUser;
            Member dbMember;

            if (dbUser == null || (dbMember = dbUser as Member) == null || dbMember.IsAdmin)
                throw new ServiceAccessDeniedException("You must be a member to make this operation");

            return dbMember;
        }

        protected Member GetDbAdmin() {
            // verificar se sou membro e admin
            User dbUser = CurrentUser;
            Member dbMember;

            if (dbUser == null || (dbMember = dbUser as Member) == null || !dbMember.IsAdmin)
                throw new ServiceAccessDeniedException("You must be admin to make this operation");

                return dbMember;
        }
    }
}
