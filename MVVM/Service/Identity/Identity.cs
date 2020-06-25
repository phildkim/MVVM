using System.Security.Principal;
namespace MVVM.Service.Identity
{
    public class Identity : IIdentity
    {
        public Identity(string name, string pass, string[] types)
        {
            Name = name;
            Password = pass;
            Usertype = types;
        }
        public string Name { get; private set; }
        public string Password { get; private set; }
        public string[] Usertype { get; private set; }

        #region IIdentity Members
        public string AuthenticationType { get { return "LOGIN AUTHENTICATION"; } }
        public bool IsAuthenticated { get { return !string.IsNullOrEmpty(Name); } }
        #endregion
    }
}