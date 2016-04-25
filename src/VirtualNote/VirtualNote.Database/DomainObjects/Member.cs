using System.Collections.Generic;

namespace VirtualNote.Database.DomainObjects
{
    public class Member : User
    {
        public virtual bool             IsAdmin { get; set; }

        ICollection<Project>    _responsabilities;
        ICollection<Project>    _assignedProjects;
        ICollection<Issue>      _issuesSolved;



        public virtual ICollection<Project> Responsabilities {
            get
            {
                return _responsabilities ?? (_responsabilities = new RelationalList<Project>(
                                                                            p => p.Responsable = this,
                                                                            p => p.Responsable = null
                                                                     ));
            }
            set { _responsabilities = value; }
        }

        // 2 way relashionshipc NxN
        public virtual ICollection<Project> AssignedProjects {
            get
            {
                return _assignedProjects ?? (_assignedProjects = new RelationalList<Project>(
                                            p =>
                                                {
                                                    if (!p.Workers.Contains(this))
                                                        p.Workers.Add(this);
                                                },
                                            p =>
                                                {
                                                    if (p.Workers.Contains(this))
                                                        p.Workers.Remove(this);
                                                }
                                            ));
            }
            set { _assignedProjects = value; }
        }


        public virtual ICollection<Issue> IssuesSolved {
            get
            {
                return _issuesSolved ?? (_issuesSolved = new RelationalList<Issue>(i => i.Member = this,
                                                                                    i => i.Member = null));
            }
            set { _issuesSolved = value; }
        }
    }
}
