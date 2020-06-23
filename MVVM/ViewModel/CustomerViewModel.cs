using MVVM.Properties;
namespace MVVM.ViewModel
{
    public class CustomerViewModel : WorkspaceViewModel, IViewModel
    {
        public CustomerViewModel()
        {
            base.DisplayName = Resources.AuthenticationViewModel_Command_CreateNewCustomer;
        }
    }
}