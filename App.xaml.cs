using Hardcodet.Wpf.TaskbarNotification;
using System.Windows;

namespace EveSwitcher
{
    public partial class App : Application
    {
        private TaskbarIcon _taskbarIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            _taskbarIcon = FindResource("TaskbarIcon") as TaskbarIcon;

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _taskbarIcon.Dispose();

            base.OnExit(e);
        }
    }
}
