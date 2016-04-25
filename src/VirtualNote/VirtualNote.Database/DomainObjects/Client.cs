using System.Collections.Generic;

namespace VirtualNote.Database.DomainObjects
{
    public class Client : User
    {
        ICollection<Issue>       _issuesReported;
        ICollection<Project>     _assignedProjects;


        public virtual ICollection<Issue> IssuesReported {
            get
            {
                return _issuesReported ?? (_issuesReported = new RelationalList<Issue>(
                                                                 i => i.Client = this,
                                                                 i => i.Client = null
                                                                 )
                                          );


            }
            set { _issuesReported = value; }
        }

        public virtual ICollection<Project> AssignedProjects {
            get
            {
                return _assignedProjects ?? (_assignedProjects = new RelationalList<Project>(
                                                                     p => p.Client = this,
                                                                     p => p.Client = null
                                                                     )
                                            );
            }
            set { _assignedProjects = value; }
        }
    }
}
