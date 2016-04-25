using System;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.DTO.Extensions
{
    internal static class CommentExtensions
    {
        public static Comment CopyToDomainObject(this CommentServiceDto commentServiceDto, User user, Issue issue)
        {
            return new Comment {
                CreatedDate = DateTime.Now,
                Description = commentServiceDto.Description,
                Issue = issue,
                User = user
            };
        }

        public static void UpdateDomainObjectFromDTO(this Comment comment, 
            CommentServiceDto commentServiceDto,
            User user)
        {
            comment.Description = commentServiceDto.Description;
            comment.LastUpdateDate = DateTime.Now;
            comment.User = user;
        }
    }
}
