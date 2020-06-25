using MVVM.Model;
using MVVM.Properties;
using MVVM.Service.Authentication;
using MVVM.Service.Identity;
using System;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
namespace MVVM.ViewModel
{
    public class AuthenticationViewModel : WorkspaceViewModel, IViewModel
    {
        #region Fields
        private readonly IAuthenticationService _authenticationService;
        private Credential _credential;
        #endregion // Fields

        #region  Commands
        public RelayCommand LoginCommand { get; }
        public RelayCommand LogoutCommand { get; }
        public RelayCommand ShowViewCommand { get; }
        #endregion // Commands

        #region Constructor
        public AuthenticationViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            Credential = Credential.CreateNewCredential();
            LoginCommand = new RelayCommand(Login, CanLogin);
            LogoutCommand = new RelayCommand(Logout, CanLogout);
            ShowViewCommand = new RelayCommand(ShowView, null);
            base.DisplayTitle = Resources.AuthenticationViewModel_DisplayTitle;
        }
        #endregion // Constructor

        #region Credential Properties
        public Credential Credential
        {
            get { return _credential; }
            set { SetProperty(ref _credential, value); }
        }
        #endregion Credential Properties

        #region Principal Properties
        public bool IsAuthenticated
        {
            get { return Thread.CurrentPrincipal.Identity.IsAuthenticated; }
        }
        public bool IsNotAuthenticated
        {
            get { return !Thread.CurrentPrincipal.Identity.IsAuthenticated; }
        }
        public string AuthenticatedUser
        {
            get
            {

                if (IsAuthenticated)
                    return string.Format("Signed in as {0}. {1}",
                        Thread.CurrentPrincipal.Identity.Name,
                        Thread.CurrentPrincipal.IsInRole(Resources.LoginIdentity_Administrators) ?
                        Resources.LoginIdentity_Administrators : Resources.LoginIdentity_Customers);
                return Resources.AuthenticationService_UnauthorizedAccessException;
            }
        }
        #endregion Principal Properties

        #region Login Methods
        private bool CanLogin(object parameter)
        {
            return !IsAuthenticated && !Credential.HasErrors;
        }
        private void Login(object parameter)
        {
            PasswordBox passwordBox = parameter as PasswordBox;
            this.Credential.Password = passwordBox.Password;
            try
            {
                // Validate credentials through the authentication service
                Credential user = _authenticationService.AuthenticateUser(this.Credential.Username, this.Credential.Password);

                // Get the current principal object
                if (!(Thread.CurrentPrincipal is LoginPrincipal loginPrincipal))
                    throw new ArgumentException(Resources.AuthenticationViewModel_ArgumentException);

                // Authenticate the user
                loginPrincipal.Identity = new LoginIdentity(user.Username, user.Password, user.Description);

                // Update UI
                NotifyProperty();
            }
            catch (UnauthorizedAccessException ex)
            {
                this.Credential.Status = ex.StackTrace;
            }
            catch (Exception ex)
            {
                this.Credential.Status = string.Format("ERROR: {0}", ex.Message);
            }
        }
        #endregion // Login Methods

        #region Logout Methods
        private bool CanLogout(object parameter)
        {
            return IsAuthenticated;
        }
        private void Logout(object parameter)
        {
            if (MessageBox.Show("Logout?", "LOGOUT", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (Thread.CurrentPrincipal is LoginPrincipal loginPrincipal)
                {
                    loginPrincipal.Identity = new LoginAnonymous();
                    NotifyProperty();
                    
                    
                }
                return;
            }
        }
        #endregion // Logout Methods

        #region NotifyPropertyChanged
        void NotifyProperty()
        {
            NotifyPropertyChanged(Resources.NotifyPropertyChanged_Authenticated);
            NotifyPropertyChanged(Resources.NotifyPropertyChanged_IsAuthenticated);
            NotifyPropertyChanged(Resources.NotifyPropertyChanged_IsNotAuthenticated);
            NotifyPropertyChanged(Resources.NotifyPropertyChanged_Status);
            LoginCommand.RaiseCanExecuteChanged();
            LogoutCommand.RaiseCanExecuteChanged();
            this.Credential.Username = string.Empty;
            this.Credential.Password = string.Empty;
            this.Credential.Status = string.Empty;
            Credential = Credential.CreateNewCredential();
        }
        #endregion // NotifyPropertyChanged

        #region Show View
        private void ShowView(object parameter)
        {
            try
            {
                this.Credential.Status = string.Empty;
          
            }
            catch (SecurityException ex)
            {
                this.Credential.Status = ex.Message;
            }
        }
        #endregion // Show View

    }
}