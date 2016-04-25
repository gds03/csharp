using System;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts.Issues;
using VirtualNote.Kernel.Contracts.Notificator;
using VirtualNote.Kernel.DTO;
using VirtualNote.Database;
using VirtualNote.Kernel.DTO.Extensions.IssuesExt;
using VirtualNote.Kernel.Query.Repository;

namespace VirtualNote.Kernel.Services.Issues
{
    public sealed class IssueAdminService : ServiceBase, IIssueAdminService
    {
        readonly INotificatorMemberService _notificator;


        public IssueAdminService(IRepository db, INotificatorMemberService notificator) : base(db)
        {
            _notificator = notificator;
        }

        public bool ChangeState(IssueServiceMemberDTO issueMemberDto)
        {
            if(issueMemberDto == null)
                throw new ArgumentNullException("issueMemberDto");

            //
            // Os admins podem mudar o estado de qualquer Issue, desde 
            // que não esteja terminado

            Member dbAdmin = GetDbAdmin();

            Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(issueMemberDto.IssueId);

            //
            // Seguro realizar acção
            if (dbIssue.State == (int)StateEnum.Terminated)
                return false;

            if (dbIssue.Member != null) 
            {
                dbIssue.UpdateDomainObjectFromDTO(issueMemberDto);

                if (issueMemberDto.State == StateEnum.Waiting) {
                    // Caso isto aconteça então o membro a realizar a acção é o membro que aceitou o pedido
                    dbIssue.Member = null;
                    _notificator.NotifyClientAboutInWaitStateAgain(IssueMembersCommon.BuildNotificatorForIssue(dbIssue, dbAdmin));
                }
                else{
                    dbIssue.Member = dbAdmin;
                    if (issueMemberDto.State == StateEnum.Terminated) {
                        _notificator.NotifyClientAboutTerminateRequest(IssueMembersCommon.BuildNotificatorForIssue(dbIssue, dbAdmin));
                    }
                }
            }
            else {
                dbIssue.Member = dbAdmin;
                dbIssue.UpdateDomainObjectFromDTO(issueMemberDto);
                var notificatorDto = IssueMembersCommon.BuildNotificatorForIssue(dbIssue, dbAdmin);

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

        public void Remove(int issueId)
        {
            //
            // Os admins podem remover qualquer Issue mesmo que este
            // contenha comentarios.

            GetDbAdmin();

            Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(issueId);

            //
            // Seguro realizar acção
            _db.Delete(dbIssue);
        }
    }
}
