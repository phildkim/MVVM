using MVVM.Properties;
namespace MVVM.Service.Identity
{
    public class LoginAnonymous : LoginIdentity
    {
        public LoginAnonymous() : base(string.Empty, string.Empty, new string[] { }) { }
    }
}