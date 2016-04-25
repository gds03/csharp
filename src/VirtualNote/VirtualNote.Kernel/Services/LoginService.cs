using System;
using VirtualNote.Common;
using VirtualNote.Common.ExtensionMethods;
using VirtualNote.Kernel.Query.Repository;
using VirtualNote.Database;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Kernel.Services
{
    public sealed class LoginService : ServiceBase
    {
        public const String Admin = "Admin";
        public const String Member = "Member";
        public const String Client = "Client";


        public LoginService(IRepository db) : base(db)
        {

        }
        /// <summary>
        ///     Try authenticate the username
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns> true if password are equal, otherwise false </returns>
        public bool Authenticate(string username, string password)
        {
            username = username.Trim().ToLower();
            password = password.Trim().ToLower();

            User userDb = _db.Query<User>().GetByName(username);
            if (userDb == null)
                return false;

            byte[] encriptedPassword = PasswordUtils.Encript(password);
            return encriptedPassword.AllBytesAreEqual(userDb.Password);
        }


        /// <summary>
        ///     Get all roles for the username
        /// </summary>
        /// <param name="username"></param>
        /// <returns> An empty array if no username match, otherwise return an array with one element - the role </returns>
        public string[] GetRolesForUser(string username)
        {
            User userDb = _db.Query<User>().GetByName(username);
            if (userDb == null)
                return new string[] { };

            String[] returnValue = new String[1];

            if (userDb is Client)
            {
                returnValue[0] = Client;
            }
            else
            {
                Member member;
                if ((member = userDb as Member) != null)
                {
                    returnValue[0] = member.IsAdmin ? Admin : Member;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            return returnValue;
        }
    }
}
