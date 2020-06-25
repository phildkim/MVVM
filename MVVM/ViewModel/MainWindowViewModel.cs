using MVVM.Model;
using MVVM.Properties;
using MVVM.Service.Authentication;
using MVVM.Service.Identity;
using MVVM.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Security;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace MVVM.ViewModel
{
    public interface IViewModel { }
    public class MainWindowViewModel : WorkspaceViewModel, IViewModel
    {

        private ICommand _loginCommand;
        private ICommand _logoutCommand;
        private object _currentView;
        private object _viewLogin;
        private object _viewLogout;


        #region Constructor
        public MainWindowViewModel(IAuthenticationService authenticationService)
        {
            _viewLogin = new LoginAuthenticationView();
            _viewLogout = new MainMenuView();

            _currentView = _viewLogout;
        }
        #endregion // Constructor
        public object LogoutCommand
        {
            get
            {
                return _viewLogin ?? (_viewLogin = new RelayCommand(
                   x =>
                   {
                       Login();
                   }));
            }
        }
        public object LoginCommand
        {
            get
            {
                return _viewLogout ?? (_viewLogout = new RelayCommand(
                   x =>
                   {
                       Logout();
                   }));
            }
        }
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }
        private void Login()
        {
            CurrentView = _viewLogin;
        }
        private void Logout()
        {
            CurrentView = _viewLogout;
        }

    }
}