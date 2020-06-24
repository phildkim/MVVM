using MVVM.Model;
using MVVM.Properties;
using MVVM.Service.Authentication;
using MVVM.Service.Identity;
using MVVM.Service.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace MVVM.ViewModel
{
    public interface IViewModel { }
    public class AuthenticationViewModel : WorkspaceViewModel, IViewModel
    {
        #region Fields
        private readonly IAuthenticationService _authenticationService;
        private readonly CustomerRepository _customerRepository;
        ObservableCollection<WorkspaceViewModel> _workspaces;
        ReadOnlyCollection<CommandViewModel> _commands;
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
            _customerRepository = new CustomerRepository("Service/Repository/customers.xml");
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

        #region Workspaces
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _workspaces.CollectionChanged += this.OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }
        void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;
            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }
        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel;
            workspace.Dispose();
            this.Workspaces.Remove(workspace);
        }
        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if (_commands == null)
                {
                    List<CommandViewModel> cmds = this.CreateCommands();
                    _commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _commands;
            }
        }
        List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel(Resources.AuthenticationViewModel_Command_CreateNewCustomer, new RelayCommand(CreateNewCustomer)),
                new CommandViewModel(Resources.AuthenticationViewModel_Command_AllCustomers, new RelayCommand(param => this.ShowAllCustomers())),
                new CommandViewModel(Resources.AuthenticationViewModel_Command_LogoutCommand, new RelayCommand(Logout, CanLogout)),
            };
        }
        void CreateNewCustomer(object parameter)
        {

            Customer newCustomer = Customer.CreateNewCustomer();
            CustomerViewModel newworkspace = new CustomerViewModel(newCustomer, _customerRepository);
            this.Workspaces.Add(newworkspace);
            this.SetActiveWorkspace(newworkspace);
        }
        void ShowAllCustomers()
        {
            if (!(this.Workspaces.FirstOrDefault(vm => vm is AllCustomersViewModel) is AllCustomersViewModel workspace))
            {
                workspace = new AllCustomersViewModel(_customerRepository);
                this.Workspaces.Add(workspace);
            }
            this.SetActiveWorkspace(workspace);
        }
        void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(this.Workspaces.Contains(workspace));
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
            if (collectionView != null)
                collectionView.MoveCurrentTo(workspace);
        }
        #endregion // Workspaces
    }
}