using MVVM.Model;
using MVVM.Properties;
using MVVM.Service.Authentication;
using MVVM.Service.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace MVVM.ViewModel
{
    public interface IViewModel { }
    public class MainWindowBaseViewModel : WorkspaceViewModel, IViewModel
    {
        private readonly CustomerRepository _customerRepository;
        ObservableCollection<WorkspaceViewModel> _workspaces;
        ReadOnlyCollection<CommandViewModel> _commands;
        private readonly IAuthenticationService _authenticationService;
        public MainWindowBaseViewModel(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _customerRepository = new CustomerRepository("Service/Repository/customers.xml");
            base.DisplayName = Resources.MainWindowViewModel_DisplayName;
        }

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
                new CommandViewModel("LOGIN", new RelayCommand(login)),
                new CommandViewModel(Resources.AuthenticationViewModel_Command_CreateNewCustomer, new RelayCommand(CreateNewCustomer)),
                new CommandViewModel(Resources.AuthenticationViewModel_Command_AllCustomers, new RelayCommand(param => this.ShowAllCustomers())),
            };
        }
        void login(object parameter)
        {
            AuthenticationViewModel workspace = new AuthenticationViewModel(_authenticationService);
            this.Workspaces.Add(workspace);
            this.SetActiveWorkspace(workspace);
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
