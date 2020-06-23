using System.Security.Principal;
namespace MVVM.Service.Identity
{
    public class LoginIdentity : IIdentity
    {
        #region LoginIdentity 
        public LoginIdentity(string name, string password, string[] description)
        {
            Name = name;
            Password = password;
            Description = description;
        }
        #endregion // LoginIdentity Constructor

        #region LoginIdentity Properties
        public string Name { get; private set; }
        public string Password { get; private set; }
        public string[] Description { get; private set; }
        #endregion // LoginIdentity Properties

        #region IIdentity Members
        public string AuthenticationType { get { return string.Empty; } }
        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion // IIdentity Members
    }
}