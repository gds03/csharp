using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;
using VirtualNote.Kernel.Configurations.StructureMap;
using VirtualNote.Kernel.Services;

namespace VirtualNote.MVC.Bootstrapper.Authentication
{
    public sealed class VnRoleProvider : RoleProvider
    {
        LoginService _service;



        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
            _service = ObjectsManager.GetInstance<LoginService>();
        }


        public override string[] GetRolesForUser(string username)
        {
            if (String.IsNullOrEmpty(username))
                throw new ProviderException("username cannot be null");

            return _service.GetRolesForUser(username);
        }


        public override bool IsUserInRole(string username, string roleName)
        {
            if(String.IsNullOrEmpty(username))
                throw new ProviderException("username cannot be null");
            if(String.IsNullOrEmpty(roleName))
                throw new ProviderException("rolename cannot be null");


            string[] rolesForUser = GetRolesForUser(username);

            if (rolesForUser.Count() == 0)
                throw new ProviderException(String.Format("No available roles for user {0}", username));
            
            roleName = roleName.ToLower();
            return rolesForUser.Any(s => s.ToLower() == roleName);
        }





        #region not implemented

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}