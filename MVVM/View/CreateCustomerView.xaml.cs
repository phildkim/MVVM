using MVVM.ViewModel;
using System.Security.Permissions;
using System.Windows.Controls;
namespace MVVM.View
{
    [PrincipalPermission(SecurityAction.Demand)]
    public partial class CreateCustomerView : UserControl, IViewModel
    {
        public CreateCustomerView()
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