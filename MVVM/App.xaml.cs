using MVVM.Service.Authentication;
using MVVM.Service.Identity;
using MVVM.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
namespace MVVM
{
    public partial class App : Application
    {
        static App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            LoginPrincipal loginPrincipal = new LoginPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(loginPrincipal);
            base.OnStartup(e);
            MainWindowBaseViewModel viewModel = new MainWindowBaseViewModel(new AuthenticationService());
            EventHandler handler = null;
            IView loginWindow = new MainWindow(viewModel);
            handler = delegate
            {
                viewModel.RequestClose -= handler;
                loginWindow.Close();
            };
            viewModel.RequestClose += handler;
            loginWindow.Show();
        }
    }
}