using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace EveSwitcher
{
    public class WindowHelper
    {
        private const string EVETitlePrefix = "EVE - ";
        private readonly ProcessProvider _processProvider;

        public WindowHelper()
        {
            _processProvider = new ProcessProvider();
            _processProvider.Start();
        }

        public string GetActiveCharacter()
        {
            var process = GetActiveWindowProcess();

            if (process == null)
                return null;

            var title = process.MainWindowTitle;

            if (!title.StartsWith(EVETitlePrefix))
                return null;

            title = title[EVETitlePrefix.Length..];

            return title;
        }

        public void SetActiveCharacter(string character)
        {
            var process = GetProcessForCharacter(character);
            var hwnd = process?.MainWindowHandle;

            if (hwnd == null)
            {
                Debug.WriteLine($"Failed to find window for {character}.");
                return;
            }

            ProcessNativeMethods.SetForegroundWindow(hwnd.Value);
        }

        public string[] GetActiveCharacters(string[] characters)
        {
            return characters.Where(x => GetProcessForCharacter(x) != null).ToArray();
        }

        public IntPtr[] GetActiveLoginScreens()
        {
            var processes = _processProvider.GetProcesses().Where(x => x.MainWindowTitle == "EVE");
            var hwnds = processes.Select(x => x.MainWindowHandle).ToArray();
            return hwnds;
        }

        public void SetActiveLoginScreen(IntPtr hwnd)
        {
            ProcessNativeMethods.SetForegroundWindow(hwnd);
        }

        public bool IsEveClientActive()
        {
            var process = GetActiveWindowProcess();
            return process?.ProcessName == "exefile";
        }

        private Process GetActiveWindowProcess()
        {
            var hwnd = ProcessNativeMethods.GetForegroundWindow();
            ProcessNativeMethods.GetWindowThreadProcessId(hwnd, out uint processId);
            return _processProvider.GetProcessById((int)processId);
        }

        private Process GetProcessForCharacter(string character)
        {
            return _processProvider.GetProcesses().FirstOrDefault(x => x.MainWindowTitle == $"{EVETitlePrefix}{character}");
        }

        static class ProcessNativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }
    }
}
