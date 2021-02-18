using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Input;

namespace EveSwitcher
{
    class AppViewModel
    {
        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                return _exitCommand ??= new RelayCommand(() =>
                {
                    Application.Current.Shutdown();
                });
            }
        }
    }
}
