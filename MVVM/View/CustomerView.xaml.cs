using MVVM.ViewModel;
using System.Security.Permissions;
using System.Windows;
namespace MVVM.View
{
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class CustomerView : Window, IView
    {
        public CustomerView()
        {
            InitializeComponent();
        }
        #region IView Members
        public IViewModel ViewModel
        {
            get { return DataContext as IViewModel; }
            set { DataContext = value; }
        }
        #endregion // IView Members
    }
}