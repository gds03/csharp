using System;
using VirtualNote.Kernel.Configurations;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.DTO.Extensions;
using VirtualNote.Kernel.Query.Repository;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.Services
{
    public sealed class ClientsService : ServiceBase, IClientsService
    {
        readonly IEmailConfigMngr _mngr;


        public ClientsService(IRepository db, IEmailConfigMngr emailMngr) : base(db)
        {
            _mngr = emailMngr;
        }


        public bool Add(ClientServiceDTO clientDto) 
        {
            if(clientDto == null)
                throw new ArgumentNullException("clientDto");

            User dbUser = _db.Query<User>().GetByName(clientDto.Name);
            if (dbUser != null)
                return false;

            _db.Insert(clientDto.CopyToDomainObject());
            return true;
        }


        
        public bool Update(ClientServiceDTO clientDto)
        {
            if (clientDto == null)
                throw new ArgumentNullException("clientDto");

            Client dbClient = _db.Query<Client>().GetById(clientDto.UserID);

            User dbUser;
            if (dbClient.Name != clientDto.Name && ((dbUser = _db.Query<User>().GetByName(clientDto.Name)) != null) && dbUser.UserID != dbClient.UserID)
                return false;

            dbClient.UpdateDomainObjectFromDTO(clientDto);
            return true;
        }


        public bool Remove(int clientId, out string clientName)
        {
            bool hasProjects;
            Client dbClient = _db.Query<Client>().GetHasProjects(clientId, out hasProjects);
            
            clientName = dbClient.Name;
            if(!hasProjects)
            {
                _db.Delete(dbClient);
                _mngr.Delete(UserType.client, clientId);
                return true;
            }

            dbClient.Enabled = false;
            return false;
        }
    }
}