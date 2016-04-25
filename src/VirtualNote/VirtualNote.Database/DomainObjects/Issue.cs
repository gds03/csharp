using System;
using System.Collections.Generic;

namespace VirtualNote.Database.DomainObjects
{
    public class Issue : IDomainObject
    {
        public virtual int              IssueID { get; set; }
        public virtual String  ShortDescription { get; set; }
        public virtual String   LongDescription { get; set; }
        public virtual int                State { get; set; }
        public virtual int             Priority { get; set; }
        public virtual int                 Type { get; set; }
        public virtual DateTime     CreatedDate { get; set; }
        public virtual DateTime? LastUpdateDate { get; set; }


        Project             _project;
        Member              _member;
        Client              _client;

        ICollection<Comment> _comments;



        public virtual Project Project {
            get { return _project; }
            set
            {
                if (_project == value)
                    return;

                if (_project != null)
                    _project.Issues.Remove(this);

                _project = value;
                if (_project != null)
                {
                    if (!_project.Issues.Contains(this))
                        _project.Issues.Add(this);
                }
            }
        }

        public virtual Member Member {
            get { return _member; }
            set
            {
                if (_member == value)
                    return;

                if (_member != null)
                    _member.IssuesSolved.Remove(this);

                _member = value;
                if (_member != null)
                {
                    if (!_member.IssuesSolved.Contains(this))
                        _member.IssuesSolved.Add(this);
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
                    _client.IssuesReported.Remove(this);

                _client = value;
                if (_client != null)
                {
                    if (!_client.IssuesReported.Contains(this))
                        _client.IssuesReported.Add(this);
                }
            }
        }


        public virtual ICollection<Comment> Comments {
            get { 
                return _comments ?? (_comments = new RelationalList<Comment>(
                                                    c => c.Issue = this,
                                                    c => c.Issue = null));
            }
            set { _comments = value; }
        }
    }
}
