using Hardcodet.Wpf.TaskbarNotification;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace EveSwitcher
{
    public partial class App : Application
    {
        private Config _config;
        private TaskbarIcon _taskbarIcon;
        private HotkeyHandler _hotkeyHandler;
        private readonly WindowHelper _windowHelper = new WindowHelper();
        private readonly Dictionary<string, string> _lastActiveCharacterForHotkey = new Dictionary<string, string>();
        private IntPtr? _lastActiveLoginScreen;

        protected override void OnStartup(StartupEventArgs e)
        {
            var configFileName = "config.json";

#if DEBUG
            if (File.Exists("config.debug.json"))
                configFileName = "config.debug.json";
#endif
            
            _config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFileName));

            _taskbarIcon = FindResource("TaskbarIcon") as TaskbarIcon;
            _hotkeyHandler = new HotkeyHandler();

            _hotkeyHandler.HotkeyPressed += HotkeyHandler_HotkeyPressed;

            _hotkeyHandler.RegisterHotkey(_config.LoginScreenHotkey);

            foreach (var hotkeyConfig in _config.Hotkeys)
            {
                _hotkeyHandler.RegisterHotkey(hotkeyConfig.Key);
            }

            base.OnStartup(e);
        }

        private void HotkeyHandler_HotkeyPressed(object sender, HotkeyEventArgs e)
        {
            var hotkey = e.Gesture.GetDisplayStringForCulture(CultureInfo.CurrentCulture);

            if (hotkey == _config.LoginScreenHotkey)
            {
                SelectNextLoginScreen();
                return;
            }

            var characters = _windowHelper.GetActiveCharacters(_config.Hotkeys[hotkey]);
            string activeCharacter = _windowHelper.GetActiveCharacter();
            string newActiveCharacter;

            if (characters.Contains(activeCharacter))
            {
                var index = Array.IndexOf(characters, activeCharacter) + 1;

                if (index >= characters.Length)
                    index = 0;

                newActiveCharacter = characters[index];
            }
            else
            {
                _lastActiveCharacterForHotkey.TryGetValue(hotkey, out string lastActiveCharacter);

                if (characters.Contains(lastActiveCharacter))
                {
                    newActiveCharacter = lastActiveCharacter;
                }
                else
                {
                    newActiveCharacter = characters.FirstOrDefault();
                }
            }

            Debug.WriteLine($"Hotkey: {hotkey}, ActiveCharacter: {activeCharacter}, NewActiveCharacter: {newActiveCharacter}");

            if (newActiveCharacter != null)
            {
                _windowHelper.SetActiveCharacter(newActiveCharacter);
                _lastActiveCharacterForHotkey[hotkey] = newActiveCharacter;
            }
        }

        private void SelectNextLoginScreen()
        {
            var activeLoginScreens = _windowHelper.GetActiveLoginScreens();

            if (!activeLoginScreens.Any())
                return;

            IntPtr newActiveLoginScreen;

            if (_lastActiveLoginScreen != null && activeLoginScreens.Contains(_lastActiveLoginScreen.Value))
            {
                var index = Array.IndexOf(activeLoginScreens, _lastActiveLoginScreen.Value) + 1;

                if (index >= activeLoginScreens.Length)
                    index = 0;

                newActiveLoginScreen = activeLoginScreens[index];
            }
            else
            {
                newActiveLoginScreen = activeLoginScreens.First();
            }

            _windowHelper.SetActiveLoginScreen(newActiveLoginScreen);
            _lastActiveLoginScreen = newActiveLoginScreen;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _hotkeyHandler.Dispose();
            _taskbarIcon.Dispose();

            base.OnExit(e);
        }
    }
}
