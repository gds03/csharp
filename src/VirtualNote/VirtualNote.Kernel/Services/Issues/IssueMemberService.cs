using System;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.Contracts.Issues;
using VirtualNote.Kernel.Contracts.Notificator;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.DTO.Extensions.IssuesExt;
using VirtualNote.Kernel.DTO.Services.Notificator;
using VirtualNote.Kernel.Query.ConversionsDTO;
using VirtualNote.Kernel.Query.Repository;

namespace VirtualNote.Kernel.Services.Issues
{
    public sealed class IssueMemberService : ServiceBase, IIssueMemberService
    {
        readonly INotificatorMemberService _notificator;

        public IssueMemberService(IRepository db, INotificatorMemberService notificator)
            : base(db) {
            _notificator = notificator;
        }





        public bool ChangeState(IssueServiceMemberDTO issueMemberDto) {
            if (issueMemberDto == null)
                throw new ArgumentNullException("issueMemberDto");

            //
            // Os membros podem mudar o estado de qualquer issue desde que:
            // Estejam associados ao projecto desse issue e que se o estado ja foi alterado
            // por um membro, ele seja esse membro, ou que o issue nao esteja terminado.

            Member dbMember = GetDbMember();
            Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(issueMemberDto.IssueId);

            // verificar se sou responsavel ou worker no projecto que recebo no Dto
            bool iAmReponsableOrWorkingOnThisProject = _db.Query<Member>()
                                                          .IsThisMemberActiveOnProject(
                                                                dbMember.UserID,
                                                                dbIssue.Project.ProjectID
                                                        );

            if (!iAmReponsableOrWorkingOnThisProject)
                throw new HijackedException("You are not assigned to the project of this issue");

            //
            // O membro pertence ao projecto em questão..
            if (dbIssue.State == (int)StateEnum.Terminated)
                return false;

            if (dbIssue.Member != null) {
                // Se estamos aqui então já foi atribuido um membro
                // e so o mebro associado pode mudar o estado
                if (dbIssue.Member.UserID != dbMember.UserID) {
                    throw new IssueWasAlreadyTakedByAnotherMember(dbIssue.Member.Name); // Retorna o nome do membro que esta a resolver o issue
                }

                dbIssue.UpdateDomainObjectFromDTO(issueMemberDto);
                if (issueMemberDto.State == StateEnum.Waiting) {
                    // Caso isto aconteça então o membro a realizar a acção é o membro que aceitou o pedido
                    dbIssue.Member = null;
                    _notificator.NotifyClientAboutInWaitStateAgain(IssueMembersCommon.BuildNotificatorForIssue(dbIssue, dbMember));
                }
                else{
                    if(issueMemberDto.State == StateEnum.Terminated){
                        _notificator.NotifyClientAboutTerminateRequest(IssueMembersCommon.BuildNotificatorForIssue(dbIssue, dbMember));
                    }
                }
            }
            else {
                dbIssue.Member = dbMember;
                dbIssue.UpdateDomainObjectFromDTO(issueMemberDto);
                var notificatorDto = IssueMembersCommon.BuildNotificatorForIssue(dbIssue, dbMember);

                if (issueMemberDto.State == StateEnum.InResolution) {
                    _notificator.NotifyMembersThatRequestWasAcceptedByAnotherMember(notificatorDto);
                    _notificator.NotifyClientAboutAcceptedRequest(notificatorDto);
                }

                if (issueMemberDto.State == StateEnum.Terminated) {
                    _notificator.NotifyClientAboutTerminateRequest(notificatorDto);
                }
            }
            return true;
        }
    }
}
