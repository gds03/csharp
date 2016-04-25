using System;
using VirtualNote.Kernel.Configurations;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Exceptions;
using VirtualNote.Kernel.DTO;
using VirtualNote.Kernel.DTO.Extensions;
using VirtualNote.Kernel.Query.ConversionsDTO;
using VirtualNote.Kernel.Query.Repository;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.Services
{
    public sealed class MembersService : ServiceBase, IMembersService
    {
        readonly IEmailConfigMngr _mngr;

        public MembersService(IRepository db, IEmailConfigMngr emailMngr) : base(db)
        {
            _mngr = emailMngr;
        }

        public bool Add(MemberServiceDTO memberDto) 
        {
            if (memberDto == null)
                throw new ArgumentNullException("memberDto");

            User dbUser = _db.Query<User>().GetByName(memberDto.Name);
            if (dbUser != null)
                return false;

            _db.Insert(memberDto.CopyToDomainObject());
            return true;
        }

        public bool Update(MemberServiceDTO memberDto)
        {
            if (memberDto == null)
                throw new ArgumentNullException("memberDto");
            
            Member dbMember = _db.Query<Member>().GetById(memberDto.UserID);

            User dbUser;
            if (dbMember.Name != memberDto.Name && ((dbUser = _db.Query<User>().GetByName(memberDto.Name)) != null) && dbUser.UserID != dbMember.UserID)
                return false;

            dbMember.UpdateDomainObjectFromDTO(memberDto);
            return true;
        }

        public bool Remove(int memberId, out string memberName)
        {
            bool hasProjects;
            Member dbMember = _db.Query<Member>().GetHasProjects(memberId, out hasProjects);
            int adminsCount = _db.Query<Member>().CountAdmins();

            if (adminsCount == 1 && dbMember.IsAdmin)
                throw new MemberLastAdminException();

            memberName = dbMember.Name;
            if(!hasProjects)
            {
                _db.Delete(dbMember);
                _mngr.Delete(dbMember.IsAdmin ? UserType.admin : UserType.member, memberId);
                return true;
            }

            dbMember.Enabled = false;
            return false;
        }
    }
}
