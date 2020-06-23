using System;
using System.Windows;
using System.Windows.Input;
namespace MVVM.ViewModel
{
    public abstract class WorkspaceViewModel : BaseViewModel
    {
        #region Fields
        RelayCommand _closeCommand;
        #endregion // Fields

        #region Constructor
        protected WorkspaceViewModel() { }
        #endregion // Constructor

        #region CloseCommand
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(param => this.OnRequestClose());
                return _closeCommand;
            }
        }
        #endregion // CloseCommand

        #region RequestClose [event]
        public event EventHandler RequestClose;
        void OnRequestClose()
        {
            if (MessageBox.Show("Close?", "CLOSE", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                RequestClose?.Invoke(this, EventArgs.Empty);
            return;
        }
        #endregion // RequestClose [event]
    }
}