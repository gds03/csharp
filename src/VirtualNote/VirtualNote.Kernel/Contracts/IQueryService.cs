using System.Collections.Generic;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.DTO.Query.Clients;
using VirtualNote.Kernel.DTO.Query.Comments;
using VirtualNote.Kernel.DTO.Query.Home;
using VirtualNote.Kernel.DTO.Query.Issues.Details;
using VirtualNote.Kernel.DTO.Query.Issues.Index;
using VirtualNote.Kernel.DTO.Query.Members;
using VirtualNote.Kernel.DTO.Query.Projects;

namespace VirtualNote.Kernel.Contracts
{
    public static class QueryServiceExtensions
    {
        public static IssueMemberQueryList GetIssuesForAdmin(this IQueryService service, int currentPage, int take){
            return service.GetIssuesForAdmin(currentPage, take, -1, IssuesSortBy.DescendingDate);
        }

        public static IssueMemberQueryList GetIssuesForMember(this IQueryService service, int currentPage, int take) {
            return service.GetIssuesForMember(currentPage, take, -1, IssuesSortBy.DescendingDate);
        }

        public static IssueClientQueryList GetIssuesForClient(this IQueryService service, int currentPage, int take) {
            return service.GetIssuesForClient(currentPage, take, -1, IssuesSortBy.DescendingDate);
        }
    }

    public interface IQueryService : IRepositoryService
    {
        #region Clients


        /// <summary>
        ///     Devolve uma lista de todos os clientes registados na bd.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ClientQueryList> GetClients();



        /// <summary>
        ///     Devolve a informação de um determinado cliente.
        ///     Tipicamente usado para Edit
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        ClientServiceDTO GetClientById(int clientId);


        /// <summary>
        ///     Devolve a informação de um determinado cliente detalhada.
        ///     Tipicamente usado para Details
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        ClientQueryDetails GetClientDetails(int clientId);


        #endregion

        



        #region Members

        /// <summary>
        ///     Devolve uma lista de todos os membros registados na bd incluido os admins
        /// </summary>
        /// <returns></returns>
        IEnumerable<MemberQueryList> GetMembers();



        /// <summary>
        ///     Devolve a informação de um determinado membro.
        ///     Tipicamente usado para Edit
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        MemberServiceDTO GetMemberById(int memberId);



        /// <summary>
        ///     Devolve a informação de um determinado membro detalhada.
        ///     Tipicamente usado para Details
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        MemberQueryDetails GetMemberDetails(int memberId);

        #endregion




        #region Projects


        /// <summary>
        ///     Devolve uma lista de projectos
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProjectQueryList> GetProjects();



        /// <summary>
        ///     Devolve informação de um determinado projecto.
        ///     Tipicamente usado para Edit
        /// </summary>
        /// <param name="projectId"> Se especificar projectId com 0 retorna informação de Clientes e Membros. Se especificar maior que 0 retorna ainda informação do projecto </param>
        /// <returns></returns>
        ProjectServiceDTO GetProjectById(int projectId);




        /// <summary>
        ///     Devolve informação detalhada de um determinado projecto
        ///     Tipicamente usado para Details
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        ProjectQueryDetails GetProjectDetails(int projectId);


        



        /// <summary>
        ///     Devolve o nome do projecto, o responsavel, uma lista de todos os trabalhadores disponiveis, e os ids
        ///     dos trabalhadores nesse projecto.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        ProjectQueryAssign GetProjectWorkers(int projectId);



       


        #endregion




        #region Issues


        #region Issues For Admins

        /// <summary>
        ///     Devolve um DTO com informacao de todos os projectos e o id do projecto 
        ///     pela qual foi realizada a query que devolve um conjunto de issues entre currentPage + take.
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="take"></param>
        /// <param name="projectId"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        IssueMemberQueryList GetIssuesForAdmin(int currentPage, int take, int projectId, IssuesSortBy sortBy);


        /// <summary>
        ///     Devolve um DTO com informação do estado do issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        IssuesMemberQueryDetails GetIssueForAdmin(int issueId);


        #endregion



        #region Issues For Members

        /// <summary>
        ///     Devolve um DTO com informacao dos projectos do membro e o id do projecto 
        ///     pela qual foi realizada a query que devolve um conjunto de issues entre currentPage + take.
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="take"></param>
        /// <param name="projectId"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        IssueMemberQueryList GetIssuesForMember(int currentPage, int take, int projectId, IssuesSortBy sortBy);



        /// <summary>
        ///     Devolve um DTO com informação do estado do issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        IssuesMemberQueryDetails GetIssueForMember(int issueId);


        #endregion



        #region Issues For Clients

        /// <summary>
        ///     Devolve um DTO com informacao de todos os projectos do cliente e o id do projecto
        ///     pela qual foi realizada a query que devolve um conjunto de issues entre currentPage + take.
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="take"></param>
        /// <param name="projectId"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        IssueClientQueryList GetIssuesForClient(int currentPage, int take, int projectId, IssuesSortBy sortBy);


        /// <summary>
        ///     Devolve um DTO com informação do estado do issue
        /// </summary>
        /// <param name="issueId"></param>
        /// <returns></returns>
        IssuesClientQueryDetails GetIssueForClient(int issueId);


        #endregion





        #endregion




        #region Comments

        /// <summary>
        ///     Devolve um objecto que contem informação sobre o Issue e uma lista de 
        ///     comentarios entre um determinado intervalo.
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="take"></param>
        /// <param name="issueId"></param>
        /// <returns></returns>
        CommentQueryList GetComments(int currentPage, int take, int issueId);



        #endregion






        #region Home



        HomeAdminQueryDetails GetHomeFeedForAdmin();


        HomeMemberQueryDetails GetHomeFeedForMember();


        HomeClientQueryDetails GetHomeFeedForClient();

        #endregion





        #region Emails


        IEnumerable<EmailConfig> GetEmailConfigsFor();



        #endregion
    }
}
