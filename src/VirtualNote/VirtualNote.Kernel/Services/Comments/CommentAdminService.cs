using System;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts.Comments;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.Query.Repository;
using VirtualNote.Kernel.DTO.Extensions;

namespace VirtualNote.Kernel.Services.Comments
{
    public sealed class CommentAdminService : ServiceBase, ICommentsAdminService
    {
        public  CommentAdminService(IRepository db) : base(db)
        {

        }

        public void Add(CommentServiceDto commentDto) {
            if(commentDto == null)
                throw new ArgumentNullException("commentDto");

            // Verificar se sou admin
            Member dbAdmin = GetDbAdmin();

            // 
            // Os admins podem inserir comentários nos issues de qualquer projecto
            Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(commentDto.IssueID);

            // Insere no repositorio
            _db.Insert(commentDto.CopyToDomainObject(dbAdmin, dbIssue));

            // Gravar persistente
            _db.SaveToDisk();
        }



        public void Remove(int commentId) {
            // Verificar se sou admin
            GetDbAdmin();

            // 
            // Os admins podem remover todos os comentários 

            // Obter comentario do repositorio
            Comment dbComment = _db.Query<Comment>().GetById(commentId);

            // Remover do repositorio
            _db.Delete(dbComment);

            // Gravar persistente
            _db.SaveToDisk();
        }
    }
}
