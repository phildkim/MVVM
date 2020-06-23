using MVVM.ViewModel;
using System.Windows;
namespace MVVM
{
    public interface IView
    {
        IViewModel ViewModel { get; set; }
        void Show();
        void Close();
    }
    public partial class MainWindow : Window, IView
    {
        public MainWindow(AuthenticationViewModel viewModel)
        {
            ViewModel = viewModel;
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