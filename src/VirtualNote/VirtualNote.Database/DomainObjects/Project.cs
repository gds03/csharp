using System;
using System.Collections.Generic;

namespace VirtualNote.Database.DomainObjects
{
    public class Project : IDomainObject
    {
        public virtual int                ProjectID { get; set; }
        public virtual String                  Name { get; set; }
        public virtual bool                 Enabled { get; set; }
        public virtual DateTime         CreatedDate { get; set; }
        public virtual String           Description { get; set; }

        Member              _responsable;
        Client              _client;

        ICollection<Member> _workers;
        ICollection<Issue>  _issues;

        public virtual Member Responsable {
            get { return _responsable; }
            set
            {
                if (_responsable == value)
                    return;

                if (_responsable != null)
                    _responsable.Responsabilities.Remove(this);

                _responsable = value;
                if (_responsable != null)
                {
                    if (!_responsable.Responsabilities.Contains(this))
                        _responsable.Responsabilities.Add(this);
                }
            }
        }

        public virtual Client Client {
            get { return _client; }
            set
            {
                if (_client == value)
                    return;

                if (_client != null)
                    _client.AssignedProjects.Remove(this);

                _client = value;
                if (_client != null)
                {
                    if (!_client.AssignedProjects.Contains(this))
                        _client.AssignedProjects.Add(this);
                }
            }
        }

        
        public virtual ICollection<Member> Workers
        {
            get
            {
                return _workers ?? (_workers = new RelationalList<Member>(
                                                   m => {
                                                       if (!m.AssignedProjects.Contains(this))
                                                           m.AssignedProjects.Add(this);
                                                   },

                                                   m => {
                                                       if (m.AssignedProjects.Contains(this))
                                                           m.AssignedProjects.Remove(this);
                                                   }));
            }
            set { _workers = value; }
        }


        public virtual ICollection<Issue> Issues {
            get
            {
                return _issues ?? (_issues = new RelationalList<Issue>(i => i.Project = this,
                                                                        i => i.Project = null));
            }
            set { _issues = value; }
        }
    }
}
