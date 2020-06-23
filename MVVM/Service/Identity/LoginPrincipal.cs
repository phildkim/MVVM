using System.Linq;
using System.Security.Principal;
namespace MVVM.Service.Identity
{
    public class LoginPrincipal : IPrincipal
    {
        #region LoginIdentity Properties
        private LoginIdentity _identity;
        public LoginIdentity Identity
        {
            get { return _identity ?? new LoginAnonymous(); }
            set { _identity = value; }
        }
        #endregion // LoginIdentity Properties

        #region IPrincipal Members
        IIdentity IPrincipal.Identity
        {
            get { return this.Identity; }
        }
        public bool IsInRole(string role)
        {
            return _identity.Description.Contains(role);
        }
        #endregion // IPrincipal Members
    }
}