using System;
using System.Windows.Input;
namespace MVVM.ViewModel
{
    public class CommandViewModel : BaseViewModel
    {
        public CommandViewModel(string displayName, ICommand command)
        {
            base.DisplayName = displayName;
            this.Command = command ?? throw new ArgumentNullException("command");
        }
        public ICommand Command { get; private set; }
    }
}