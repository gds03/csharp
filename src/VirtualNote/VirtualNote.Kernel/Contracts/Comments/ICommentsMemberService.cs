using VirtualNote.Kernel.DTO;

namespace VirtualNote.Kernel.Contracts.Comments
{
    public interface ICommentsMemberService : IRepositoryService
    {
        /// <summary>
        ///     Insere um comentario para o issueId passado no DTO
        /// </summary>
        /// <param name="commentDto"></param>
        void Add(CommentServiceDto commentDto);



        /// <summary>
        ///     Tenta remover um comentario dado commentId
        /// </summary>
        /// <param name="commentId"></param>
        bool Remove(int commentId);
    }
}
