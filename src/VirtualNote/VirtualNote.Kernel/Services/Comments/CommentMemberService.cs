using System;
using System.Linq;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts.Comments;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.DTO.Extensions;
using VirtualNote.Kernel.Query.ConversionsDTO;
using VirtualNote.Kernel.Query.Repository;

namespace VirtualNote.Kernel.Services.Comments
{
    public sealed class CommentMemberService : ServiceBase, ICommentsMemberService
    {
        public CommentMemberService(IRepository db) : base(db)
        {

        }

        public void Add(CommentServiceDto commentDto) {
            if(commentDto == null)
                throw new ArgumentNullException("commentDto");

            // Necessito de ser membro
            Member dbMember = GetDbMember();

            //
            // Os membros podem inserir comentarios nos issues cujo o projecto associado
            // ao issue pertenca a lista de projectos do membro

            // Obter lista de ids de projectos do membro
            var myProjectsIds = _db.Query<Member>()
                                   .GetProjectsIdsWhereIamResponsableOrWork(dbMember.UserID);

            // Obter issue
            Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(commentDto.IssueID);

            if(!myProjectsIds.Any(pid => pid == dbIssue.Project.ProjectID))
                throw new HijackedException("You are not assigned to the project that has this issue");

            // Insere no repositorio
            _db.Insert(commentDto.CopyToDomainObject(dbMember, dbIssue));

            // Gravar persistente
            _db.SaveToDisk();
        }




        public bool Remove(int commentId) {
            // Necessito de ser membro
            Member dbMember = GetDbMember();

            //
            // Os membros podem apagar o comentario
            // se o comentario foi postado por este mesmo membro

            // Obter comentario do repositorio
            Comment dbComment = _db.Query<Comment>().GetById(commentId);
            if (dbComment.User.UserID != dbMember.UserID)
                return false;
            
            // Apaga o comentario do repositorio
            _db.Delete(dbComment);

            // Gravar persistente
            _db.SaveToDisk();

            return true;
        }
    }
}
