using System.Linq;
using System.Security.Principal;
namespace MVVM.Service.Identity
{
    public class IdentityPrincipal : IPrincipal
    {
        private Identity _identity;
        public Identity Identity
        {
            get { return _identity ?? new IdentityAnonymous(); }
            set { _identity = value; }
        }
        #region IPrincipal Members
        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; }
        }
        public bool IsInRole(string role)
        {
            return _identity.Usertype.Contains(role);
        }
        #endregion
    }
}