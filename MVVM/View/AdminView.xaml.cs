using MVVM.ViewModel;
using System.Security.Permissions;
using System.Windows;
namespace MVVM.View
{
    [PrincipalPermission(SecurityAction.Demand, Role = "Administrators")]
    public partial class AdminView : Window, IView
    {
        public AdminView()
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