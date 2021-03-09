using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace EveSwitcher
{
    public delegate void HotkeyEventHandler(object sender, HotkeyEventArgs e);

    public class HotkeyEventArgs : EventArgs
    {
        public KeyGesture Gesture { get; }
        public bool Handled { get; set; }

        public HotkeyEventArgs(KeyGesture gesture)
        {
            Gesture = gesture;
        }
    }

    public class HotkeyHandler : IDisposable
    {
        private static int _lastId = 0;

        private bool _disposed;
        private readonly WindowHelper _windowHelper;
        private readonly bool _requireActiveEveClient;
        private readonly Window _window;
        private readonly IntPtr _handle;
        private readonly HwndSource _hwndSource;
        private readonly List<RegisteredHotKey> _registeredHotkeys = new List<RegisteredHotKey>();

        public event HotkeyEventHandler HotkeyPressed;

        public HotkeyHandler(bool requireActiveEveClient)
        {
            _requireActiveEveClient = requireActiveEveClient;

            _windowHelper = new WindowHelper();

            // Force window creation to start a message queue, but hide it.
            _window = new Window
            {
                Width = 0,
                Height = 0,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false,
                ShowActivated = false,
                Visibility = Visibility.Hidden,
                Title = "Eve Switcher"
            };
            _window.Show();
            _window.Hide();

            _handle = new WindowInteropHelper(_window).Handle;
            _hwndSource = HwndSource.FromHwnd(_handle);

            _hwndSource.AddHook(HwndHook);
        }

        public bool RegisterHotkey(string gesture)
        {
            var converter = new KeyGestureConverter();
            var keyGesture = (KeyGesture)converter.ConvertFromString(gesture);
            return RegisterHotkey(keyGesture);
        }

        public bool RegisterHotkey(KeyGesture gesture)
        {
            if (_registeredHotkeys.Any(x => x.Gesture == gesture))
            {
                throw new ArgumentException("Hotkey already registered.");
            }

            var vk = KeyInterop.VirtualKeyFromKey(gesture.Key);
            int id = GetNextId();
            uint fsModifiers = 0;

            if ((gesture.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                fsModifiers |= HotkeyNativeMethods.MOD_ALT;

            if ((gesture.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                fsModifiers |= HotkeyNativeMethods.MOD_CONTROL;

            if ((gesture.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                fsModifiers |= HotkeyNativeMethods.MOD_SHIFT;

            var result = HotkeyNativeMethods.RegisterHotKey(_handle, id, fsModifiers, (uint)vk);

            if (result)
            {
                _registeredHotkeys.Add(new RegisteredHotKey { Id = id, Gesture = gesture, Modifiers = fsModifiers, VirtualKey = (uint)vk });
            }

            return result;
        }

        public void UnregisterHotkey(KeyGesture gesture)
        {
            var hotkey = _registeredHotkeys.FirstOrDefault(x => x.Gesture == gesture);

            if (hotkey == null)
                throw new ArgumentException("Hotkey not registered.");

            HotkeyNativeMethods.UnregisterHotKey(_handle, hotkey.Id);

            _registeredHotkeys.Remove(hotkey);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                var hotkey = _registeredHotkeys.FirstOrDefault(x => x.Id == id);

                if (hotkey != null)
                {
                    if (_requireActiveEveClient && !_windowHelper.IsEveClientActive())
                    {
                        handled = false;
                    }
                    else
                    {
                        var keyEventArgs = new HotkeyEventArgs(hotkey.Gesture);
                        HotkeyPressed?.Invoke(this, keyEventArgs);
                        handled = keyEventArgs.Handled;
                    }

                    if (!handled)
                    {
                        HotkeyNativeMethods.UnregisterHotKey(_handle, hotkey.Id);
                        KeyboardMessage.Send(hotkey.Gesture);
                        HotkeyNativeMethods.RegisterHotKey(_handle, hotkey.Id, hotkey.Modifiers, hotkey.VirtualKey);
                    }
                }
            }

            return IntPtr.Zero;
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var hotkey in _registeredHotkeys.ToArray())
                {
                    UnregisterHotkey(hotkey.Gesture);
                }

                _hwndSource.RemoveHook(HwndHook);
                _hwndSource.Dispose();

                _window.Close();
            }

            _disposed = true;
        }

        private static int GetNextId()
        {
            return _lastId++;
        }

        private class RegisteredHotKey
        {
            public int Id { get; set; }
            public KeyGesture Gesture { get; set; }
            public uint Modifiers { get; set; }
            public uint VirtualKey { get; set; }
        }

        private static class HotkeyNativeMethods
        {
            public const uint WM_HOTKEY = 0x0312;

            public const uint MOD_ALT = 0x0001;
            public const uint MOD_CONTROL = 0x0002;
            public const uint MOD_NOREPEAT = 0x4000;
            public const uint MOD_SHIFT = 0x0004;
            public const uint MOD_WIN = 0x0008;

            public const uint ERROR_HOTKEY_ALREADY_REGISTERED = 1409;

            [DllImport("user32.dll")]
            public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

            [DllImport("user32.dll")]
            public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        }
    }
}
