using System.Collections.Generic;

namespace EveSwitcher
{
    public class Config
    {
        public bool RequireActiveEveClient { get; set; }
        public string LoginScreenHotkey { get; set; }
        public Dictionary<string, string[]> Hotkeys { get; set; }
    }
}
