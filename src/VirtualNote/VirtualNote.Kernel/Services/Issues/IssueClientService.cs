using System;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.Contracts.Issues;
using VirtualNote.Kernel.Contracts.Notificator;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.DTO.Extensions.IssuesExt;
using VirtualNote.Kernel.DTO.Services.Notificator;
using VirtualNote.Kernel.Query.Repository;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.Services.Issues
{
    public sealed class IssueClientService : ServiceBase, IIssueClientService
    {
        readonly INotificatorClientService _notificator;


        public IssueClientService(IRepository db, INotificatorClientService notificator)
            : base(db) 
        {
            _notificator = notificator;
        }

        public void Add(IssueServiceClientDTO issueClientDto) {
            if (issueClientDto == null)
                throw new ArgumentNullException("issueClientDto");

            //
            // O cliente so pode inserir um issue se, o projecto a que pertence o issue
            // pertence ao cliente.

            // Verificar se sou cliente
            Client dbClient = GetDbClient();

            // Obter o projecto a partir do Dto data e ver se o cliente sou eu..
            Project dbProject = _db.Query<Project>().GetByIdIncludeClient(issueClientDto.ProjectId);

            if (dbProject == null || dbProject.Client.UserID != dbClient.UserID) {
                throw new HijackedException("ProjectID hijacked");
            }

            if(dbProject != null && !dbProject.Enabled)
                throw new ProjectDisabledException("Project disabled");

            // Se estou aqui então eu sou o owner do projecto
            _db.Insert(issueClientDto.CopyToDomainObject(dbProject, dbClient));

            //
            // Executar o serviço de enviar emails para os membros do projecto deste issue.
            NotificatorClientDTO clientDto = new NotificatorClientDTO(
                dbClient.Name,
                dbProject.ProjectID,
                dbProject.Name,
                
                issueClientDto.Priority,
                issueClientDto.Type,
                issueClientDto.ShortDescription,
                issueClientDto.LongDescription
            );

            // Notificar
            _notificator.NotifyMembers(clientDto);
        }

        

        public bool Update(IssueServiceClientDTO issueClientDto)
        {
            if (issueClientDto == null)
                throw new ArgumentNullException("issueClientDto");

            //
            // O cliente so pode actualizar um issue se, foi ele a inserir esse issue
            // e so pode actualizar se o estado nao for terminated

            // Verificar se sou cliente
            Client dbClient = GetDbClient();    

            // Obter o issue a partir do Dto data (ignorando o projectId) e vou verificar
            // se eu sou quem inseriou o issue porque só eu posso editar o issue.
            Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(issueClientDto.IssueId);
            if (dbClient.UserID != dbIssue.Client.UserID)
                throw new HijackedException("This issue doesnt belongs to you");

            if (dbIssue.Project != null && !dbIssue.Project.Enabled)
                throw new ProjectDisabledException("Project disabled");

            //
            // Seguro realizar acções
            if (dbIssue.State == (int)StateEnum.Terminated)
                return false;

            // A partir daqui é seguro realizar acções
            dbIssue.UpdateDomainObjectFromDTO(issueClientDto);
            return true;
        }

        public bool Remove(int issueId)
        {
            //
            // O cliente so pode apagar um issue, se o issue pertence ao cliente e 
            // se e só se o issue se encontra num estado waiting

            // Verificar se sou cliente
            Client dbClient = GetDbClient();    


            // Verificar se fui eu que adicionei o issue, pois só eu o posso apagar..
            Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(issueId);
            if (dbClient.UserID != dbIssue.Client.UserID)
                throw new HijackedException("This issue doesnt belongs to you");

            if (dbIssue.Project != null && !dbIssue.Project.Enabled)
                throw new ProjectDisabledException("Project disabled");

            // A partir daqui é seguro realizar acções
            if (dbIssue.State == (int)StateEnum.Waiting)
            {
                //
                // Executar o serviço de enviar emails para os membros do projecto deste issue. 
                _db.Delete(dbIssue);
                return true;
            }
            return false;
        }
    }
}
