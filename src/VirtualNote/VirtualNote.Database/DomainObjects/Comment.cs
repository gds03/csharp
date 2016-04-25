using System;

namespace VirtualNote.Database.DomainObjects
{
    public class Comment : IDomainObject
    {
        public virtual int            CommentID { get; set; }
        public virtual DateTime     CreatedDate { get; set; }
        public virtual DateTime? LastUpdateDate { get; set; }
        public virtual String       Description { get; set; }

        User        _user;
        Issue       _issue;

        public virtual User User {
            get { return _user; }
            set {
                if (_user == value)
                    return;

                if (_user != null)
                    _user.Comments.Remove(this);

                _user = value;
                if (_user != null)
                {
                    if (!_user.Comments.Contains(this))
                        _user.Comments.Add(this);
                }
            }
        }

        public virtual Issue Issue {
            get { return _issue; }
            set
            {
                if (_issue == value)
                    return;

                if (_issue != null)
                    _issue.Comments.Remove(this);

                _issue = value;
                if (_issue != null)
                {
                    if (!_issue.Comments.Contains(this))
                        _issue.Comments.Add(this);
                }
            }
        }
    }
}
