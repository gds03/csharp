using System.Text;
using System.Web.Mvc;

namespace VirtualNote.MVC.Attributes.Authorization
{
    public class AuthorizedAttribute : AuthorizeAttribute
    {
        public AuthorizedAttribute()
        {
            

        }
        public AuthorizedAttribute(string[] roles) {
            StringBuilder rolesSb = new StringBuilder();

            foreach (string s in roles) {
                rolesSb.Append(s);
                rolesSb.Append(",");
            }
            rolesSb.Remove(rolesSb.Length - 1, 1);
            Roles = rolesSb.ToString();
        }
    }
}