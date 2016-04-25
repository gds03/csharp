using System;
using System.Collections.Generic;

namespace VirtualNote.Database.DomainObjects
{
    public abstract class User : IDomainObject
    {
        public virtual int            UserID { get; set; }
        public virtual String           Name { get; set; }
        public virtual byte[]       Password { get; set; }
        public virtual DateTime  CreatedDate { get; set; }
        public virtual bool          Enabled { get; set; }
        public virtual String          Email { get; set; }
        public virtual String          Phone { get; set; }

        ICollection<Comment>           _comments;


        public virtual ICollection<Comment> Comments {
            get
            {
                return _comments ?? (_comments = new RelationalList<Comment>(
                                                     c => c.User = this,
                                                     c => c.User = null
                                                     )
                                    );
            }
            set { _comments = value; }
        }
    }
}
