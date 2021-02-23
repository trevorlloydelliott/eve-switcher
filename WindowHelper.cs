﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace EveSwitcher
{
    public class WindowHelper
    {
        private const string EVETitlePrefix = "EVE - ";

        public string GetActiveCharacter()
        {
            var hwnd = ProcessNativeMethods.GetForegroundWindow();
            ProcessNativeMethods.GetWindowThreadProcessId(hwnd, out uint processId);
            var process = Process.GetProcessById((int)processId);

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
            var processes = Process.GetProcessesByName("exefile").Where(x => x.MainWindowTitle == "EVE");
            var hwnds = processes.Select(x => x.MainWindowHandle).ToArray();
            return hwnds;
        }

        public void SetActiveLoginScreen(IntPtr hwnd)
        {
            ProcessNativeMethods.SetForegroundWindow(hwnd);
        }

        private Process GetProcessForCharacter(string character)
        {
            return Process.GetProcessesByName("exefile").FirstOrDefault(x => x.MainWindowTitle == $"{EVETitlePrefix}{character}");
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