using System;
using StructureMap;
using StructureMap.Configuration.DSL;
using VirtualNote.Database;
using VirtualNote.Kernel.Contracts;
using VirtualNote.Kernel.Contracts.Comments;
using VirtualNote.Kernel.Contracts.Emails;
using VirtualNote.Kernel.Contracts.Issues;
using VirtualNote.Kernel.Contracts.Notificator;
using VirtualNote.Kernel.Managers;
using VirtualNote.Kernel.Services;
using VirtualNote.Kernel.Services.Comments;
using VirtualNote.Kernel.Services.Emails;
using VirtualNote.Kernel.Services.Issues;
using VirtualNote.Kernel.Services.Notificator;

namespace VirtualNote.Kernel.Configurations.StructureMap
{
    sealed class RegistryStructureMap : Registry
    {
        public RegistryStructureMap()
        {
            ConfigureDatabaseInstances();
            ConfigureServicesInstances();
        }

        private void ConfigureDatabaseInstances()
        {
            // For<IRepository>().Singleton().Use<MemoryRepository>();
            For<IRepository>().HttpContextScoped().Use<DiskRepository>();
        }

        private void ConfigureServicesInstances()
        {
            // Query & Login
            For<LoginService>().HttpContextScoped().Use<LoginService>();

            // Query Service
            For<IQueryService>().HttpContextScoped().Use<QueryService>();

            // Configurations
            For<IClientsService>().HttpContextScoped().Use<ClientsService>();
            For<IMembersService>().HttpContextScoped().Use<MembersService>();
            For<IProjectsService>().HttpContextScoped().Use<ProjectsService>();

            // Issues
            For<IIssueClientService>().HttpContextScoped().Use<IssueClientService>();
            For<IIssueMemberService>().HttpContextScoped().Use<IssueMemberService>();
            For<IIssueAdminService>().HttpContextScoped().Use<IssueAdminService>();

            // Comments
            For<ICommentsAdminService>().HttpContextScoped().Use<CommentAdminService>();
            For<ICommentsMemberService>().HttpContextScoped().Use<CommentMemberService>();
            For<ICommentsClientService>().HttpContextScoped().Use<CommentClientService>();

            // Emails
            For<IEmailAdminService>().HttpContextScoped().Use<EmailAdminService>();
            For<IEmailMemberService>().HttpContextScoped().Use<EmailMemberService>();
            For<IEmailClientService>().HttpContextScoped().Use<EmailClientService>();

            // Notificators
            For<INotificatorClientService>().HttpContextScoped().Use<NotificatorClientService>();
            For<INotificatorMemberService>().HttpContextScoped().Use<NotificatorMemberService>();

            // Emails
            For<IEmailConfigMngr>().Use(x => EmailConfigMngr.Instance);
        }
        
    }

    public static class ObjectsManager
    {
        public static void Initialize()
        {
            ObjectFactory.Initialize(x =>
                x.AddRegistry(new RegistryStructureMap())
            );
        }

        public static Object GetInstance(Type type)
        {
            return ObjectFactory.GetInstance(type);
        }

        public static TEntity GetInstance<TEntity>()
        {
            return ObjectFactory.GetInstance<TEntity>();
        }
    }
}