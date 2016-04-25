using System;
using System.Collections.Generic;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.DTO.Query;
using VirtualNote.Kernel.DTO.Query.Clients;
using VirtualNote.Kernel.DTO.Query.Comments;
using VirtualNote.Kernel.DTO.Query.Home;
using VirtualNote.Kernel.DTO.Query.Issues.Details;
using VirtualNote.Kernel.DTO.Query.Issues.Details.InitialData;
using VirtualNote.Kernel.DTO.Query.Issues.Index;
using VirtualNote.Kernel.DTO.Query.Members;
using VirtualNote.Kernel.DTO.Query.Projects;
using VirtualNote.Kernel.Query.ConversionsDTO;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using System.Linq;
using VirtualNote.Kernel.Query.Include;
using VirtualNote.Kernel.Query.Repository;
using VirtualNote.Kernel.Types;

namespace VirtualNote.Kernel.Services
{
    public sealed class QueryService : ServiceBase, IQueryService
    {
        readonly IEmailConfigMngr _mngr;

        public QueryService(IRepository db, IEmailConfigMngr emailMngr) : base(db)
        {
            _mngr = emailMngr;
        }



        #region Clients

        public IEnumerable<ClientQueryList> GetClients() {
            return _db.Query<Client>().GetListQueryDTO();
        }

        public ClientServiceDTO GetClientById(int clientId) {
            return _db.Query<Client>().GetByIdQueryDTO(clientId);
        }

        public ClientQueryDetails GetClientDetails(int clientId) {
            return _db.Query<Client>().GetDetailsQueryDTO(clientId);
        }

        #endregion





        #region Members

        public IEnumerable<MemberQueryList> GetMembers() {
            return _db.Query<Member>().GetListQueryDTO();
        }

        public MemberServiceDTO GetMemberById(int memberId) {
            return _db.Query<Member>().GetByIdQueryDTO(memberId);
        }

        public MemberQueryDetails GetMemberDetails(int memberId) {
            return _db.Query<Member>().GetDetailsQueryDTO(memberId);
        }

        #endregion





        #region Projects

        public IEnumerable<ProjectQueryList> GetProjects()
        {
            return _db.Query<Project>().GetListQueryDTO();
        }

        public ProjectQueryDetails GetProjectDetails(int projectId)
        {
            return _db.Query<Project>().GetDetailsQueryDTO(projectId);
        }

        public ProjectServiceDTO GetProjectById(int projectId)
        {
            var initialData =  new ProjectQueryCreateUpdate
                       {
                           Clients =
                               _db.Query<Client>().Select(c => new KeyIdValueString {Id = c.UserID, Value = c.Name}).
                               ToList(),
                           Members =
                               _db.Query<Member>().Select(c => new KeyIdValueString {Id = c.UserID, Value = c.Name}).
                               ToList(),
                       };

            var returnValue = new ProjectServiceDTO {InitialData = initialData};

            if(projectId > 0)
            {
                Project dbProject = _db.Query<Project>().Where(p => p.ProjectID == projectId)
                                    .Include(p => p.Responsable)
                                    .Include(p => p.Client)
                                    .Single();

                returnValue.ClientId = dbProject.Client.UserID;
                returnValue.ResponsableId = dbProject.Responsable.UserID;
                returnValue.ProjectID = dbProject.ProjectID;
                returnValue.Name = dbProject.Name;
                returnValue.Description = dbProject.Description;
                returnValue.Enabled = dbProject.Enabled;
            }
            return returnValue;
        }

        public ProjectQueryAssign GetProjectWorkers(int projectId)
        {
            var project = _db.Query<Project>().GetByIdIncludeResponsableAndWorkers(projectId);
            int responsableId = project.Responsable.UserID;

            // All except the responsable
            var availableMembers = _db.Query<Member>().Where(m => m.UserID != responsableId)
                                                      .Select(m => new KeyIdValueString
                                                                    {
                                                                        Id = m.UserID,
                                                                        Value = m.Name
                                                                    }).ToList();

            var workers = project.Workers.Select(m => m.UserID);

            return new ProjectQueryAssign
                       {
                           AvailableWorkers = availableMembers,
                           WorkersIdsWorking = workers,
                           ProjectName = project.Name,
                           ResponsableName = project.Responsable.Name
                       };
        }


        #endregion




        #region Issues


        #region Issues Auxiliary Methods


        static IssuesMemberQueryDetails GetMemberQueryDetailsFromIssue(Issue dbIssue, Member theMember, IEnumerable<KeyIdValueString> clientProjects, int numComments) {
            return new IssuesMemberQueryDetails {
                ClientName = dbIssue.Client.Name,
                InitialData = new IssueQueryInitialData {
                    ClientProjects = clientProjects,
                    Priorities = EnumUtils.GetPriorityValues(),
                    States = EnumUtils.GetStateValues(),
                    Types = EnumUtils.GetTypeValues()
                },
                ShowDeleteButton = theMember.IsAdmin,
                CreatedAt = dbIssue.CreatedDate,
                IssueId = dbIssue.IssueID,
                ProjectId = dbIssue.Project.ProjectID,
                LastUpdateAt = dbIssue.LastUpdateDate,
                LongDescription = dbIssue.LongDescription,
                ShortDescription = dbIssue.ShortDescription,
                MemberSolving = dbIssue.Member == null ? null : dbIssue.Member.Name,
                NumComments = numComments,
                Priority = (PriorityEnum)dbIssue.Priority,
                Type = (TypeEnum)dbIssue.Type,
                State = (StateEnum)dbIssue.State
            };
        }



        #endregion



        #region Issues For Admins

        public IssueMemberQueryList GetIssuesForAdmin(int currentPage, int take, int projectId, IssuesSortBy sortBy){
            GetDbAdmin();

            //
            // Os admins veem todos os issues disponiveis no sistema 
            // e podem filtrar por qualquer projecto.

            var projectsKivs = _db.Query<Project>().ToKeyIdValueString();
            var filtersKivs = EnumUtils.GetIssuesSortValues().ToKivs();

            if (projectId == -1 && projectsKivs.Count() > 0)
                projectId = projectsKivs.First().Id;

            return _db.Query<Issue>().GetIssuesForMembersOnProjectInSpecificRange(
                projectsKivs, filtersKivs, projectId, sortBy, currentPage, take
            );
        }

        public IssuesMemberQueryDetails GetIssueForAdmin(int issueId){
            Member dbAdmin = GetDbAdmin();

            //
            // Os admins podem consultar todos os issues
            int numComments;
            Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(issueId, out numComments);

            // Obtem a lista de todos os projectos do cliente
            var clientProjectsKivs = _db.Query<Project>().ToKeyIdValueString(dbIssue.Client.UserID);

            return GetMemberQueryDetailsFromIssue(dbIssue, dbAdmin, clientProjectsKivs, numComments);
        }

        

        #endregion



        #region Issues For Members

        public IssueMemberQueryList GetIssuesForMember(int currentPage, int take, int projectId, IssuesSortBy sortBy)
        {
            Member dbMember = GetDbMember();

            //
            // Os membros veem os issues em que têm projectos em que 
            // sao responsaveis e trabalhadores podem filtrar pelos mesmos

            var myProjectsIds = _db.Query<Member>()
                                   .GetProjectsIdsWhereIamResponsableOrWork(dbMember.UserID);
            
            if(myProjectsIds.Count() > 0)
            {
                if(projectId == -1){
                    projectId = myProjectsIds.First();
                }

                if(!myProjectsIds.Any(pid => pid == projectId))
                    throw new HijackedException("You are not assigned to this project");
            }

            var memberProjectsKivs = _db.Query<Project>().ToKeyIdValueString(myProjectsIds);
            var filtersKivs = EnumUtils.GetIssuesSortValues().ToKivs();


            return _db.Query<Issue>().GetIssuesForMembersOnProjectInSpecificRange(
                memberProjectsKivs, filtersKivs, projectId, sortBy, currentPage, take
            ); 
        }

        public IssuesMemberQueryDetails GetIssueForMember(int issueId){
            Member dbMember = GetDbMember();

            
            int numComments;
            Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(issueId, out numComments);

            //
            // Os membros podem consultar os issues onde trabalham
            // e onde são responsaveis
            IEnumerable<int> myProjectsIds = _db.Query<Member>().GetProjectsIdsWhereIamResponsableOrWork(dbMember.UserID);
            if(!myProjectsIds.Any(pid => pid == dbIssue.Project.ProjectID))
                throw new HijackedException("You are not assigned to the project that has this issue");

            // Obtem a lista de todos os projectos do cliente
            var clientProjectsKivs = _db.Query<Project>().ToKeyIdValueString(dbIssue.Client.UserID);

            return GetMemberQueryDetailsFromIssue(dbIssue, dbMember, clientProjectsKivs, numComments);
        }


        #endregion



        #region Issues For Clients

        public IssueClientQueryList GetIssuesForClient(int currentPage, int take, int projectId, IssuesSortBy sortBy)
        {
            Client dbClient = GetDbClient();

            //
            // Os clientes veem os issues em que têm projectos
            // e podem filtrar pelos mesmos

            var myProjectsKivs = _db.Query<Project>().ToKeyIdValueString(dbClient.UserID);
            var filtersKivs = EnumUtils.GetIssuesSortValues().ToKivs();

            if (myProjectsKivs.Count() > 0) {
                if (projectId == -1){
                    projectId = myProjectsKivs.First().Id;
                }

                if (!myProjectsKivs.Any(kivs => kivs.Id == projectId))
                    throw new HijackedException("You are not assigned to this project");
            }

            return _db.Query<Issue>().GetIssuesForClientsOnProjectInSpecificRange(
                myProjectsKivs, filtersKivs, projectId, sortBy, currentPage, take
            ); 
        }

        public IssuesClientQueryDetails GetIssueForClient(int issueId) {
            Client dbClient = GetDbClient();
            
            var result = new IssuesClientQueryDetails();

            if(issueId > 0){
                // 
                // É operacao de edit/details
                //
                // Os clientes so podem consultar os issues que reportaram
                // 
                int numComments;
                Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(issueId, out numComments);

                if (dbIssue.Client.UserID != dbClient.UserID)
                    throw new HijackedException("You are not the client that reported this issue");

                dbClient = dbIssue.Client;

                result.CreatedAt = dbIssue.CreatedDate;
                result.IssueId = dbIssue.IssueID;
                result.ProjectId = dbIssue.Project.ProjectID;
                result.LastUpdateAt = dbIssue.LastUpdateDate;
                result.LongDescription = dbIssue.LongDescription;
                result.ShortDescription = dbIssue.ShortDescription;
                result.MemberSolving = dbIssue.Member == null ? null : dbIssue.Member.Name;
                result.NumComments = numComments;
                result.Priority = (PriorityEnum)dbIssue.Priority;
                result.Type = (TypeEnum)dbIssue.Type;
                result.State = (StateEnum)dbIssue.State;
            }

            // Obtem a lista de todos os projectos do cliente
            var clientProjectsKivs = _db.Query<Project>().ToKeyIdValueString(dbClient.UserID);

            //
            // InitialData é sempre iniciado.
            result.InitialData = new IssueQueryInitialData
            {
                Priorities = EnumUtils.GetPriorityValues(),
                States = EnumUtils.GetStateValues(),
                Types = EnumUtils.GetTypeValues(),
                ClientProjects = clientProjectsKivs
            };

            return result;
        }


        #endregion


        #endregion




        #region Comments

        
        public CommentQueryList GetComments(int currentPage, int take, int issueId)
        {
            Member tryMember;
            bool isCurrentUserAdmin = false;
            if ((tryMember = CurrentUser as Member) != null)
                isCurrentUserAdmin = tryMember.IsAdmin;

            var query = _db.Query<Comment>();

            // Lista de Comentarios para determinado issue
            var issueCommentsSet = query.Where(c => c.Issue.IssueID == issueId)
                            .OrderByDescending(c => c.CreatedDate)
                            .Skip((currentPage - 1) * take)
                            .Take(take)
                            .Select(c => new CommentQueryDetails {
                                CommentId = c.CommentID,
                                CreatedAt = c.CreatedDate,
                                Description = c.Description,
                                ReportedBy = c.User.Name,
                                CanEdit = isCurrentUserAdmin || c.User.UserID == CurrentUser.UserID
                            })
                            .ToList();

            // Total de comentarios para determinado issue
            var issueCommentsTotal = query.Count(c => c.Issue.IssueID == issueId);

            // Informacao do issue
            var issueData = _db.Query<Issue>().Where(i => i.IssueID == issueId)
                             .Select(i => new {
                                 issueId = i.IssueID,
                                 createdAt = i.CreatedDate,
                                 reportedBy = i.Client.Name,
                                 memberSolving = i.Member,
                                 description = i.ShortDescription
                             })
                             .First();

            return new CommentQueryList {
                Data = new PaginatedList<CommentQueryDetails>(issueCommentsSet, currentPage, take, issueCommentsTotal),
                IssueShortDescription = issueData.description,
                IssueMemberSolving = issueData.memberSolving != null ? issueData.memberSolving.Name : "",
                IssueCreatedAt = issueData.createdAt,
                IssueReportedBy = issueData.reportedBy,
                IssueId = issueData.issueId
            };
        }


        #endregion





        #region Home


        HomeMemberQueryDetailsWelcomeData GetWelcomeData(Member member, IEnumerable<int> myProjectsIds)
        {
            HomeMemberQueryDetailsWelcomeData result = new HomeMemberQueryDetailsWelcomeData();

            //
            // Obter welcome data
            var annon1 = _db.Query<Member>()
                            .Where(m => m.UserID == member.UserID)
                            .Select(m => new {
                                Responsabilities = m.Responsabilities.Count(),
                                WorkingOn = m.AssignedProjects.Count()
                            }).Single();



            //
            // SupportedRequests são os pedidos que dei suporte
            int supportedRequests = _db.Query<Issue>()
                                       .Count(i => i.Member != null && i.Member.UserID == member.UserID);


            //
            // PendingRequests são os pedidos em espera onde estou a trabalhar
            int pendingRequests;
            if(member.IsAdmin){
                pendingRequests = _db.Query<Issue>().Count(i => i.State == (int)StateEnum.Waiting);
            }else{
                pendingRequests = _db.Query<Issue>().Count(i => myProjectsIds.Contains(i.Project.ProjectID) && i.State == (int)StateEnum.Waiting); 
            }

            result.ProjectsResponsable = annon1.Responsabilities;
            result.ProjectsWorking = annon1.WorkingOn;
            result.PendingRequests = pendingRequests;
            result.SupportedRequests = supportedRequests;

            return result;
        }

        HomeAdminQueryDetailsConfigData GetConfigsData()
        {
            HomeAdminQueryDetailsConfigData result = new HomeAdminQueryDetailsConfigData();
            
            // Como fazer numa so query?
            result.ClientsSubscribed = _db.Query<Client>().Count();
            result.ClientsSubscribedEnabled = _db.Query<Client>().Count(c => c.Enabled);

            result.MembersRegistered = _db.Query<Member>().Count();
            result.MembersRegisteredEnabled = _db.Query<Member>().Count(m => m.Enabled);

            result.ProjectsRegistered = _db.Query<Project>().Count();
            result.ProjectsRegisteredEnabled = _db.Query<Project>().Count(p => p.Enabled);

            return result;
        }


        public HomeClientQueryDetails GetHomeFeedForClient() 
        {
            Client dbClient = GetDbClient();
            HomeClientQueryDetails result = new HomeClientQueryDetails();            
            
            result.WelcomeData = new HomeClientQueryDetailsWelcomeData();
            
            //
            // Obter welcome data
            var annon = _db.Query<Client>()
                           .Where(c => c.UserID == dbClient.UserID)
                           .Select(c => new {
                               Projects = c.AssignedProjects.Count(),
                               PendingRequests = c.IssuesReported.Count(i => i.State == (int)StateEnum.Waiting),
                               SupportedRequests = c.IssuesReported.Count(i => i.State != (int)StateEnum.Waiting)
                           }).Single();

            result.WelcomeData.Projects = annon.Projects;
            result.WelcomeData.PendingRequests = annon.PendingRequests;
            result.WelcomeData.SupportedRequests = annon.SupportedRequests;



            //
            // Obter lista de issues (Inicialmente mostra apenas os 7 ultimos)
            const int take = 7;
            var firstTakeIssues =_db.Query<Issue>().OrderByDescending(i => i.CreatedDate)
                                  .Where(i => i.Client.UserID == dbClient.UserID)
                                  .Take(take)
                                  .Select(i => new {
                                        IssueId = i.IssueID,
                                        State = (StateEnum)i.State,
                                        ShortDescription = i.ShortDescription,
                                        MemberSolving = i.Member == null ? null : i.Member.Name,
                                        Updated = i.LastUpdateDate
                                  }).ToList();

            result.Requests = firstTakeIssues.Select(x => new HomeCLientQueryDetailsRequests
                                        {
                                            IssueId = x.IssueId,
                                            State = x.State,
                                            MemberSolving = x.MemberSolving,
                                            ShortDescription = x.ShortDescription,
                                            UpdatedAt = x.Updated
                                        }).ToList();

            //
            // Obter lista de estatisticas..
            // Obter os takeStatistics primeiros projectos do cliente com mais issues 
            // e devolver estatisticas
            const int takeStatistics = 4;
            result.IssuesStatistics = _db.Query<Issue>()
                                        
                                        .Where(i => i.Client.UserID == dbClient.UserID)                 // Filtrar cliente
                                        .GroupBy(i => i.Project.Name)                                   // Agrupar pelo nome do projecto
                                        .OrderByDescending(ig => ig.Count())                            // Depois de agrupar ordernar pelo count do kvp (ig)
                                        .Take(takeStatistics)                                           // Obter os takeStatistics primeiros
                                        .Select(x => new HomeClientQueryDetailsIssuesStatistic
                                                     {
                                                         ProjectName = x.Key,
                                                         SupportedRequests = x.Count(i => i.Project.Name == x.Key && i.State != (int)StateEnum.Waiting ),
                                                         TotalRequests = x.Count(i => i.Project.Name == x.Key),
                                                     })
                                        .ToList();
            return result;

        }

        public HomeAdminQueryDetails GetHomeFeedForAdmin(){
            Member dbAdmin = GetDbAdmin();

            HomeAdminQueryDetails result = new HomeAdminQueryDetails
            {
                WelcomeData = GetWelcomeData(dbAdmin, new int[] { }),
                ConfigsData = GetConfigsData(),
                Requests = _db.Query<Issue>().GetRequests(10, new int[]{ }, true)
            };

            return result;
        }

        public HomeMemberQueryDetails GetHomeFeedForMember() {
            Member dbMember = GetDbMember();

            // Obter os projectos do membro
            var myProjectsIds = _db.Query<Member>().GetProjectsIdsWhereIamResponsableOrWork(dbMember.UserID);

            HomeMemberQueryDetails result = new HomeMemberQueryDetails
            {
                WelcomeData = GetWelcomeData(dbMember, myProjectsIds),
                Requests = _db.Query<Issue>().GetRequests(7, myProjectsIds, false)
            };

            return result;
        }

        #endregion




        #region Emails


        public IEnumerable<EmailConfig> GetEmailConfigsFor(){
            User dbUser = CurrentUser;
            UserType type;
            if(dbUser is Client)
                type = UserType.client;
            else{
                Member m;
                if( (m = dbUser as Member) == null )
                    throw new InvalidOperationException();

                type = m.IsAdmin ? UserType.admin : UserType.member;
            }

            bool hasElement;
            return _mngr.Find(type, dbUser.UserID, out hasElement);
        }



        #endregion

    }
}
