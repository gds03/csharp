using System;
using System.Linq;
using VirtualNote.Database.DomainObjects;
using VirtualNote.Kernel.Contracts.Comments;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.DTO;
using VirtualNote.Database;
using VirtualNote.Kernel.DTO.Extensions;
using VirtualNote.Kernel.Query.ConversionsDTO;
using VirtualNote.Kernel.Query.Repository;

namespace VirtualNote.Kernel.Services.Comments
{
    public sealed class CommentClientService : ServiceBase, ICommentsClientService
    {
        public CommentClientService(IRepository db) : base(db)
        {

        }



        public void Add(CommentServiceDto commentDto) {
            if (commentDto == null)
                throw new ArgumentNullException("commentDto");

            Client dbClient = GetDbClient();

            //
            // Os clientes podem inserir comentarios nos issues cujo o projecto associado
            // ao issue pertenca a lista de projectos do cliente

            // Obter lista de ids de projectos associados ao cliente
            var myProjectsIds = _db.Query<Client>().GetMyProjectsIds(dbClient.UserID);

            // Obter issue
            Issue dbIssue = _db.Query<Issue>().GetByIdIncludeAll(commentDto.IssueID);

            if (!myProjectsIds.Any(pid => pid == dbIssue.Project.ProjectID))
                throw new HijackedException("You are not assigned to the project that has this issue");

            // Insere no repositorio
            _db.Insert(commentDto.CopyToDomainObject(dbClient, dbIssue));

            // Gravar persistente
            _db.SaveToDisk();
        }

       


        public bool Remove(int commentId) {
            Client dbClient = GetDbClient();

            //
            // Os clientes podem apagar o comentario
            // se o comentario foi postado por este mesmo cliente

            // Obter comentario do repositorio
            Comment dbComment = _db.Query<Comment>().GetById(commentId);
            if (dbComment.User.UserID != dbClient.UserID)
                return false;

            // Apaga o comentario do repositorio
            _db.Delete(dbComment);

            // Gravar persistente
            _db.SaveToDisk();

            return true;
        }
    }
}
