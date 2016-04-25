using VirtualNote.Kernel.DTO;

namespace VirtualNote.Kernel.Contracts.Comments
{
    public interface ICommentsAdminService : IRepositoryService
    {
        /// <summary>
        ///     Insere um comentario para o issueId passado no DTO
        /// </summary>
        /// <param name="commentDto"></param>
        void Add(CommentServiceDto commentDto);



        /// <summary>
        ///     Remove um comentario dado commentId.
        /// </summary>
        /// <param name="commentId"></param>
        void Remove(int commentId);
    }
}
