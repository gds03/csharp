using System.Data.Entity;
using VirtualNote.Database.DomainObjects;

namespace VirtualNote.Database.Configurations.Database
{
    public sealed class VirtualNoteDbContext : DbContext
    {
        readonly DiskRepository _repository;

        
        internal DiskRepository Repository
        {
            get { return _repository; }
        }
        

        public VirtualNoteDbContext(DiskRepository repository)
        {
            Configuration.LazyLoadingEnabled = false;
            _repository = repository;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SetTablesName(modelBuilder);
            SetDbContraints(modelBuilder);
            SetDbRelashionships(modelBuilder);
        }


        static void SetTablesName(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Member>().ToTable("Member");
            modelBuilder.Entity<Client>().ToTable("Client");
            modelBuilder.Entity<Project>().ToTable("Project");
            modelBuilder.Entity<Issue>().ToTable("Issue");
            modelBuilder.Entity<Comment>().ToTable("Comment");
        }

        static void SetDbContraints(DbModelBuilder modelBuilder)
        {
            SetUserConstraints(modelBuilder);
            SetProjectConstraints(modelBuilder);
            SetIssueConstraints(modelBuilder);
            SetCommentContraints(modelBuilder);
        }




        static void SetUserConstraints(DbModelBuilder modelBuilder)
        {
            // Name
            modelBuilder.Entity<User>().Property(u => u.Name).IsRequired().HasMaxLength(20);

            // Password
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired().HasMaxLength(32);

            // CreatedDate
            // Como usar default values?
            modelBuilder.Entity<User>().Property(u => u.CreatedDate).IsRequired();

            // Enabled
            // Como usar default values?
            modelBuilder.Entity<User>().Property(u => u.Enabled).IsRequired();

            // Email
            modelBuilder.Entity<User>().Property(u => u.Email).IsOptional().HasMaxLength(100);

            // Phone
            modelBuilder.Entity<User>().Property(u => u.Phone).IsOptional().HasMaxLength(30);
        }

        static void SetProjectConstraints(DbModelBuilder modelBuilder)
        {
            // Name
            // Configurar para unique
            modelBuilder.Entity<Project>().Property(p => p.Name).IsRequired().HasMaxLength(30);

            // CreatedDate
            // Default(getdate())
            modelBuilder.Entity<Project>().Property(p => p.CreatedDate).IsRequired();

            // Enabled
            // Default(1)
            modelBuilder.Entity<Project>().Property(p => p.Enabled).IsRequired();

            // Description
            modelBuilder.Entity<Project>().Property(p => p.Description).IsRequired().HasMaxLength(100);

            // Responsable must be not null
            modelBuilder.Entity<Project>().HasRequired(p => p.Responsable);

            // Client must be not null
            modelBuilder.Entity<Project>().HasRequired(p => p.Client);
        }

        static void SetIssueConstraints(DbModelBuilder modelBuilder)
        {
            // Comment
            modelBuilder.Entity<Issue>().Property(i => i.LongDescription).IsRequired().HasMaxLength(1000);

            // Description
            modelBuilder.Entity<Issue>().Property(i => i.ShortDescription).IsRequired().HasMaxLength(100);

            // State
            modelBuilder.Entity<Issue>().Property(i => i.State).IsRequired();

            // Priority
            modelBuilder.Entity<Issue>().Property(i => i.Priority).IsRequired();

            // Type
            modelBuilder.Entity<Issue>().Property(i => i.Type).IsRequired();

            // CreatedDate
            // Default(Getdate())

            // LastUpdateDate
            modelBuilder.Entity<Issue>().Property(i => i.LastUpdateDate).IsOptional();


            modelBuilder.Entity<Issue>().HasRequired(i => i.Client);
            modelBuilder.Entity<Issue>().HasRequired(i => i.Project);
        }

        private static void SetCommentContraints(DbModelBuilder modelBuilder)
        {
            // CreatedDate
            modelBuilder.Entity<Comment>().Property(c => c.CreatedDate).IsRequired();

            // LastUpdateDate
            modelBuilder.Entity<Comment>().Property(c => c.LastUpdateDate).IsOptional();

            // Description
            modelBuilder.Entity<Comment>().Property(c => c.Description)
                        .IsRequired()
                        .HasMaxLength(2000);
        }

        static void SetDbRelashionships(DbModelBuilder modelBuilder)
        {

            //
            // Configura a relação Member 1 - N Project (Responsable)
            modelBuilder.Entity<Member>()
                        .HasMany(m => m.Responsabilities)
                        .WithRequired(p => p.Responsable)
                        .WillCascadeOnDelete(false);

            //
            // Configura a relação Member N - N Project (Workers)
            modelBuilder.Entity<Member>()
                        .HasMany(m => m.AssignedProjects)
                        .WithMany(p => p.Workers)
                        .Map(middleTable => middleTable.ToTable("Member_Project")
                                                        .MapLeftKey("MemberID")
                                                        .MapRightKey("ProjectID"));

            //
            // Configura a relação Project N - 1 Client (Belongs)
            modelBuilder.Entity<Client>()
                        .HasMany(c => c.AssignedProjects)
                        .WithRequired(p => p.Client)
                        .WillCascadeOnDelete(false);



            //
            // Configura a relação User 1 - N Comment (Adds)
            modelBuilder.Entity<User>()
                        .HasMany(u => u.Comments)
                        .WithRequired(c => c.User)
                        .WillCascadeOnDelete(false);

            //
            // Configura a relação Issue 1 - N Comment (Contains)
            modelBuilder.Entity<Issue>()
                        .HasMany(i => i.Comments)
                        .WithRequired(c => c.Issue)
                        .WillCascadeOnDelete(true);         // aquando um issue é removido, 
                                                            // são removidos os comentarios associados
        }
    }
}
