﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace EveSwitcher
{
    public static class KeyboardMessage
    {
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint INPUT_KEYBOARD = 0x01;

        private static Dictionary<ModifierKeys, VirtualKeyCode> _modifierKeyCodes = new Dictionary<ModifierKeys, VirtualKeyCode>
        {
            { ModifierKeys.Control, VirtualKeyCode.CONTROL },
            { ModifierKeys.Shift, VirtualKeyCode.SHIFT },
            { ModifierKeys.Alt, VirtualKeyCode.MENU }
        };

        public static void Send(Key key)
        {
            Send(new KeyGesture(key));
        }

        public static void Send(KeyGesture gesture)
        {
            int keyCode = KeyInterop.VirtualKeyFromKey(gesture.Key);
            if (!Enum.IsDefined(typeof(VirtualKeyCode), keyCode))
            {
                throw new ArgumentException("Invalid virtual key code");
            }

            var vk = (VirtualKeyCode)keyCode;
            var keys = new List<VirtualKeyCode>();

            foreach (var modifier in _modifierKeyCodes.Keys)
            {
                if ((gesture.Modifiers & modifier) == modifier)
                {
                    keys.Add(_modifierKeyCodes[modifier]);
                }
            }

            keys.Add(vk);

            DispatchInput(keys.Select(x => CreateInput(x, 0)).ToArray());
            DispatchInput(keys.Select(x => CreateInput(x, KEYEVENTF_KEYUP)).ToArray());
        }

        private static INPUT CreateInput(VirtualKeyCode keyCode, uint flags)
        {
            return new INPUT
            {
                Type = INPUT_KEYBOARD,
                Data =
                {
                    Keyboard =
                    {
                        KeyCode = (ushort)keyCode,
                        Scan = 0,
                        Flags = IsExtendedKey(keyCode) ? flags | KEYEVENTF_EXTENDEDKEY : flags,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };
        }

        private static void DispatchInput(INPUT[] inputs)
        {
            uint successful = NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
            if (successful != inputs.Length)
            {
                throw new Exception(
                    "Some simulated input commands were not sent successfully. " +
                    "The most common reason for this happening are the security features of Windows including User Interface Privacy Isolation (UIPI). " +
                    "Your application can only send commands to applications of the same or lower elevation. Similarly certain commands are restricted to Accessibility/UIAutomation applications. " +
                    "Refer to the project home page and the code samples for more information.");
            }
        }

        private static bool IsExtendedKey(VirtualKeyCode keyCode)
        {
            return keyCode == VirtualKeyCode.MENU ||
                   keyCode == VirtualKeyCode.LMENU ||
                   keyCode == VirtualKeyCode.RMENU ||
                   keyCode == VirtualKeyCode.CONTROL ||
                   keyCode == VirtualKeyCode.RCONTROL ||
                   keyCode == VirtualKeyCode.INSERT ||
                   keyCode == VirtualKeyCode.DELETE ||
                   keyCode == VirtualKeyCode.HOME ||
                   keyCode == VirtualKeyCode.END ||
                   keyCode == VirtualKeyCode.PRIOR ||
                   keyCode == VirtualKeyCode.NEXT ||
                   keyCode == VirtualKeyCode.RIGHT ||
                   keyCode == VirtualKeyCode.UP ||
                   keyCode == VirtualKeyCode.LEFT ||
                   keyCode == VirtualKeyCode.DOWN ||
                   keyCode == VirtualKeyCode.NUMLOCK ||
                   keyCode == VirtualKeyCode.CANCEL ||
                   keyCode == VirtualKeyCode.SNAPSHOT ||
                   keyCode == VirtualKeyCode.DIVIDE;
        }

#pragma warning disable 0649

        private enum VirtualKeyCode //: UInt16
        {
            /// <summary>
            /// Left mouse button
            /// </summary>
            LBUTTON = 0x01,

            /// <summary>
            /// Right mouse button
            /// </summary>
            RBUTTON = 0x02,

            /// <summary>
            /// Control-break processing
            /// </summary>
            CANCEL = 0x03,

            /// <summary>
            /// Middle mouse button (three-button mouse) - NOT contiguous with LBUTTON and RBUTTON
            /// </summary>
            MBUTTON = 0x04,

            /// <summary>
            /// Windows 2000/XP: X1 mouse button - NOT contiguous with LBUTTON and RBUTTON
            /// </summary>
            XBUTTON1 = 0x05,

            /// <summary>
            /// Windows 2000/XP: X2 mouse button - NOT contiguous with LBUTTON and RBUTTON
            /// </summary>
            XBUTTON2 = 0x06,

            // 0x07 : Undefined

            /// <summary>
            /// BACKSPACE key
            /// </summary>
            BACK = 0x08,

            /// <summary>
            /// TAB key
            /// </summary>
            TAB = 0x09,

            // 0x0A - 0x0B : Reserved

            /// <summary>
            /// CLEAR key
            /// </summary>
            CLEAR = 0x0C,

            /// <summary>
            /// ENTER key
            /// </summary>
            RETURN = 0x0D,

            // 0x0E - 0x0F : Undefined

            /// <summary>
            /// SHIFT key
            /// </summary>
            SHIFT = 0x10,

            /// <summary>
            /// CTRL key
            /// </summary>
            CONTROL = 0x11,

            /// <summary>
            /// ALT key
            /// </summary>
            MENU = 0x12,

            /// <summary>
            /// PAUSE key
            /// </summary>
            PAUSE = 0x13,

            /// <summary>
            /// CAPS LOCK key
            /// </summary>
            CAPITAL = 0x14,

            /// <summary>
            /// Input Method Editor (IME) Kana mode
            /// </summary>
            KANA = 0x15,

            /// <summary>
            /// IME Hanguel mode (maintained for compatibility; use HANGUL)
            /// </summary>
            HANGEUL = 0x15,

            /// <summary>
            /// IME Hangul mode
            /// </summary>
            HANGUL = 0x15,

            // 0x16 : Undefined

            /// <summary>
            /// IME Junja mode
            /// </summary>
            JUNJA = 0x17,

            /// <summary>
            /// IME final mode
            /// </summary>
            FINAL = 0x18,

            /// <summary>
            /// IME Hanja mode
            /// </summary>
            HANJA = 0x19,

            /// <summary>
            /// IME Kanji mode
            /// </summary>
            KANJI = 0x19,

            // 0x1A : Undefined

            /// <summary>
            /// ESC key
            /// </summary>
            ESCAPE = 0x1B,

            /// <summary>
            /// IME convert
            /// </summary>
            CONVERT = 0x1C,

            /// <summary>
            /// IME nonconvert
            /// </summary>
            NONCONVERT = 0x1D,

            /// <summary>
            /// IME accept
            /// </summary>
            ACCEPT = 0x1E,

            /// <summary>
            /// IME mode change request
            /// </summary>
            MODECHANGE = 0x1F,

            /// <summary>
            /// SPACEBAR
            /// </summary>
            SPACE = 0x20,

            /// <summary>
            /// PAGE UP key
            /// </summary>
            PRIOR = 0x21,

            /// <summary>
            /// PAGE DOWN key
            /// </summary>
            NEXT = 0x22,

            /// <summary>
            /// END key
            /// </summary>
            END = 0x23,

            /// <summary>
            /// HOME key
            /// </summary>
            HOME = 0x24,

            /// <summary>
            /// LEFT ARROW key
            /// </summary>
            LEFT = 0x25,

            /// <summary>
            /// UP ARROW key
            /// </summary>
            UP = 0x26,

            /// <summary>
            /// RIGHT ARROW key
            /// </summary>
            RIGHT = 0x27,

            /// <summary>
            /// DOWN ARROW key
            /// </summary>
            DOWN = 0x28,

            /// <summary>
            /// SELECT key
            /// </summary>
            SELECT = 0x29,

            /// <summary>
            /// PRINT key
            /// </summary>
            PRINT = 0x2A,

            /// <summary>
            /// EXECUTE key
            /// </summary>
            EXECUTE = 0x2B,

            /// <summary>
            /// PRINT SCREEN key
            /// </summary>
            SNAPSHOT = 0x2C,

            /// <summary>
            /// INS key
            /// </summary>
            INSERT = 0x2D,

            /// <summary>
            /// DEL key
            /// </summary>
            DELETE = 0x2E,

            /// <summary>
            /// HELP key
            /// </summary>
            HELP = 0x2F,

            /// <summary>
            /// 0 key
            /// </summary>
            VK_0 = 0x30,

            /// <summary>
            /// 1 key
            /// </summary>
            VK_1 = 0x31,

            /// <summary>
            /// 2 key
            /// </summary>
            VK_2 = 0x32,

            /// <summary>
            /// 3 key
            /// </summary>
            VK_3 = 0x33,

            /// <summary>
            /// 4 key
            /// </summary>
            VK_4 = 0x34,

            /// <summary>
            /// 5 key
            /// </summary>
            VK_5 = 0x35,

            /// <summary>
            /// 6 key
            /// </summary>
            VK_6 = 0x36,

            /// <summary>
            /// 7 key
            /// </summary>
            VK_7 = 0x37,

            /// <summary>
            /// 8 key
            /// </summary>
            VK_8 = 0x38,

            /// <summary>
            /// 9 key
            /// </summary>
            VK_9 = 0x39,

            //
            // 0x3A - 0x40 : Udefined
            //

            /// <summary>
            /// A key
            /// </summary>
            VK_A = 0x41,

            /// <summary>
            /// B key
            /// </summary>
            VK_B = 0x42,

            /// <summary>
            /// C key
            /// </summary>
            VK_C = 0x43,

            /// <summary>
            /// D key
            /// </summary>
            VK_D = 0x44,

            /// <summary>
            /// E key
            /// </summary>
            VK_E = 0x45,

            /// <summary>
            /// F key
            /// </summary>
            VK_F = 0x46,

            /// <summary>
            /// G key
            /// </summary>
            VK_G = 0x47,

            /// <summary>
            /// H key
            /// </summary>
            VK_H = 0x48,

            /// <summary>
            /// I key
            /// </summary>
            VK_I = 0x49,

            /// <summary>
            /// J key
            /// </summary>
            VK_J = 0x4A,

            /// <summary>
            /// K key
            /// </summary>
            VK_K = 0x4B,

            /// <summary>
            /// L key
            /// </summary>
            VK_L = 0x4C,

            /// <summary>
            /// M key
            /// </summary>
            VK_M = 0x4D,

            /// <summary>
            /// N key
            /// </summary>
            VK_N = 0x4E,

            /// <summary>
            /// O key
            /// </summary>
            VK_O = 0x4F,

            /// <summary>
            /// P key
            /// </summary>
            VK_P = 0x50,

            /// <summary>
            /// Q key
            /// </summary>
            VK_Q = 0x51,

            /// <summary>
            /// R key
            /// </summary>
            VK_R = 0x52,

            /// <summary>
            /// S key
            /// </summary>
            VK_S = 0x53,

            /// <summary>
            /// T key
            /// </summary>
            VK_T = 0x54,

            /// <summary>
            /// U key
            /// </summary>
            VK_U = 0x55,

            /// <summary>
            /// V key
            /// </summary>
            VK_V = 0x56,

            /// <summary>
            /// W key
            /// </summary>
            VK_W = 0x57,

            /// <summary>
            /// X key
            /// </summary>
            VK_X = 0x58,

            /// <summary>
            /// Y key
            /// </summary>
            VK_Y = 0x59,

            /// <summary>
            /// Z key
            /// </summary>
            VK_Z = 0x5A,

            /// <summary>
            /// Left Windows key (Microsoft Natural keyboard)
            /// </summary>
            LWIN = 0x5B,

            /// <summary>
            /// Right Windows key (Natural keyboard)
            /// </summary>
            RWIN = 0x5C,

            /// <summary>
            /// Applications key (Natural keyboard)
            /// </summary>
            APPS = 0x5D,

            // 0x5E : reserved

            /// <summary>
            /// Computer Sleep key
            /// </summary>
            SLEEP = 0x5F,

            /// <summary>
            /// Numeric keypad 0 key
            /// </summary>
            NUMPAD0 = 0x60,

            /// <summary>
            /// Numeric keypad 1 key
            /// </summary>
            NUMPAD1 = 0x61,

            /// <summary>
            /// Numeric keypad 2 key
            /// </summary>
            NUMPAD2 = 0x62,

            /// <summary>
            /// Numeric keypad 3 key
            /// </summary>
            NUMPAD3 = 0x63,

            /// <summary>
            /// Numeric keypad 4 key
            /// </summary>
            NUMPAD4 = 0x64,

            /// <summary>
            /// Numeric keypad 5 key
            /// </summary>
            NUMPAD5 = 0x65,

            /// <summary>
            /// Numeric keypad 6 key
            /// </summary>
            NUMPAD6 = 0x66,

            /// <summary>
            /// Numeric keypad 7 key
            /// </summary>
            NUMPAD7 = 0x67,

            /// <summary>
            /// Numeric keypad 8 key
            /// </summary>
            NUMPAD8 = 0x68,

            /// <summary>
            /// Numeric keypad 9 key
            /// </summary>
            NUMPAD9 = 0x69,

            /// <summary>
            /// Multiply key
            /// </summary>
            MULTIPLY = 0x6A,

            /// <summary>
            /// Add key
            /// </summary>
            ADD = 0x6B,

            /// <summary>
            /// Separator key
            /// </summary>
            SEPARATOR = 0x6C,

            /// <summary>
            /// Subtract key
            /// </summary>
            SUBTRACT = 0x6D,

            /// <summary>
            /// Decimal key
            /// </summary>
            DECIMAL = 0x6E,

            /// <summary>
            /// Divide key
            /// </summary>
            DIVIDE = 0x6F,

            /// <summary>
            /// F1 key
            /// </summary>
            F1 = 0x70,

            /// <summary>
            /// F2 key
            /// </summary>
            F2 = 0x71,

            /// <summary>
            /// F3 key
            /// </summary>
            F3 = 0x72,

            /// <summary>
            /// F4 key
            /// </summary>
            F4 = 0x73,

            /// <summary>
            /// F5 key
            /// </summary>
            F5 = 0x74,

            /// <summary>
            /// F6 key
            /// </summary>
            F6 = 0x75,

            /// <summary>
            /// F7 key
            /// </summary>
            F7 = 0x76,

            /// <summary>
            /// F8 key
            /// </summary>
            F8 = 0x77,

            /// <summary>
            /// F9 key
            /// </summary>
            F9 = 0x78,

            /// <summary>
            /// F10 key
            /// </summary>
            F10 = 0x79,

            /// <summary>
            /// F11 key
            /// </summary>
            F11 = 0x7A,

            /// <summary>
            /// F12 key
            /// </summary>
            F12 = 0x7B,

            /// <summary>
            /// F13 key
            /// </summary>
            F13 = 0x7C,

            /// <summary>
            /// F14 key
            /// </summary>
            F14 = 0x7D,

            /// <summary>
            /// F15 key
            /// </summary>
            F15 = 0x7E,

            /// <summary>
            /// F16 key
            /// </summary>
            F16 = 0x7F,

            /// <summary>
            /// F17 key
            /// </summary>
            F17 = 0x80,

            /// <summary>
            /// F18 key
            /// </summary>
            F18 = 0x81,

            /// <summary>
            /// F19 key
            /// </summary>
            F19 = 0x82,

            /// <summary>
            /// F20 key
            /// </summary>
            F20 = 0x83,

            /// <summary>
            /// F21 key
            /// </summary>
            F21 = 0x84,

            /// <summary>
            /// F22 key
            /// </summary>
            F22 = 0x85,

            /// <summary>
            /// F23 key
            /// </summary>
            F23 = 0x86,

            /// <summary>
            /// F24 key
            /// </summary>
            F24 = 0x87,

            //
            // 0x88 - 0x8F : Unassigned
            //

            /// <summary>
            /// NUM LOCK key
            /// </summary>
            NUMLOCK = 0x90,

            /// <summary>
            /// SCROLL LOCK key
            /// </summary>
            SCROLL = 0x91,

            // 0x92 - 0x96 : OEM Specific

            // 0x97 - 0x9F : Unassigned

            //
            // L* & R* - left and right Alt, Ctrl and Shift virtual keys.
            // Used only as parameters to GetAsyncKeyState() and GetKeyState().
            // No other API or message will distinguish left and right keys in this way.
            //

            /// <summary>
            /// Left SHIFT key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
            /// </summary>
            LSHIFT = 0xA0,

            /// <summary>
            /// Right SHIFT key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
            /// </summary>
            RSHIFT = 0xA1,

            /// <summary>
            /// Left CONTROL key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
            /// </summary>
            LCONTROL = 0xA2,

            /// <summary>
            /// Right CONTROL key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
            /// </summary>
            RCONTROL = 0xA3,

            /// <summary>
            /// Left MENU key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
            /// </summary>
            LMENU = 0xA4,

            /// <summary>
            /// Right MENU key - Used only as parameters to GetAsyncKeyState() and GetKeyState()
            /// </summary>
            RMENU = 0xA5,

            /// <summary>
            /// Windows 2000/XP: Browser Back key
            /// </summary>
            BROWSER_BACK = 0xA6,

            /// <summary>
            /// Windows 2000/XP: Browser Forward key
            /// </summary>
            BROWSER_FORWARD = 0xA7,

            /// <summary>
            /// Windows 2000/XP: Browser Refresh key
            /// </summary>
            BROWSER_REFRESH = 0xA8,

            /// <summary>
            /// Windows 2000/XP: Browser Stop key
            /// </summary>
            BROWSER_STOP = 0xA9,

            /// <summary>
            /// Windows 2000/XP: Browser Search key
            /// </summary>
            BROWSER_SEARCH = 0xAA,

            /// <summary>
            /// Windows 2000/XP: Browser Favorites key
            /// </summary>
            BROWSER_FAVORITES = 0xAB,

            /// <summary>
            /// Windows 2000/XP: Browser Start and Home key
            /// </summary>
            BROWSER_HOME = 0xAC,

            /// <summary>
            /// Windows 2000/XP: Volume Mute key
            /// </summary>
            VOLUME_MUTE = 0xAD,

            /// <summary>
            /// Windows 2000/XP: Volume Down key
            /// </summary>
            VOLUME_DOWN = 0xAE,

            /// <summary>
            /// Windows 2000/XP: Volume Up key
            /// </summary>
            VOLUME_UP = 0xAF,

            /// <summary>
            /// Windows 2000/XP: Next Track key
            /// </summary>
            MEDIA_NEXT_TRACK = 0xB0,

            /// <summary>
            /// Windows 2000/XP: Previous Track key
            /// </summary>
            MEDIA_PREV_TRACK = 0xB1,

            /// <summary>
            /// Windows 2000/XP: Stop Media key
            /// </summary>
            MEDIA_STOP = 0xB2,

            /// <summary>
            /// Windows 2000/XP: Play/Pause Media key
            /// </summary>
            MEDIA_PLAY_PAUSE = 0xB3,

            /// <summary>
            /// Windows 2000/XP: Start Mail key
            /// </summary>
            LAUNCH_MAIL = 0xB4,

            /// <summary>
            /// Windows 2000/XP: Select Media key
            /// </summary>
            LAUNCH_MEDIA_SELECT = 0xB5,

            /// <summary>
            /// Windows 2000/XP: Start Application 1 key
            /// </summary>
            LAUNCH_APP1 = 0xB6,

            /// <summary>
            /// Windows 2000/XP: Start Application 2 key
            /// </summary>
            LAUNCH_APP2 = 0xB7,

            //
            // 0xB8 - 0xB9 : Reserved
            //

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the ';:' key 
            /// </summary>
            OEM_1 = 0xBA,

            /// <summary>
            /// Windows 2000/XP: For any country/region, the '+' key
            /// </summary>
            OEM_PLUS = 0xBB,

            /// <summary>
            /// Windows 2000/XP: For any country/region, the ',' key
            /// </summary>
            OEM_COMMA = 0xBC,

            /// <summary>
            /// Windows 2000/XP: For any country/region, the '-' key
            /// </summary>
            OEM_MINUS = 0xBD,

            /// <summary>
            /// Windows 2000/XP: For any country/region, the '.' key
            /// </summary>
            OEM_PERIOD = 0xBE,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '/?' key 
            /// </summary>
            OEM_2 = 0xBF,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '`~' key 
            /// </summary>
            OEM_3 = 0xC0,

            //
            // 0xC1 - 0xD7 : Reserved
            //

            //
            // 0xD8 - 0xDA : Unassigned
            //

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '[{' key
            /// </summary>
            OEM_4 = 0xDB,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the '\|' key
            /// </summary>
            OEM_5 = 0xDC,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the ']}' key
            /// </summary>
            OEM_6 = 0xDD,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard. Windows 2000/XP: For the US standard keyboard, the 'single-quote/double-quote' key
            /// </summary>
            OEM_7 = 0xDE,

            /// <summary>
            /// Used for miscellaneous characters; it can vary by keyboard.
            /// </summary>
            OEM_8 = 0xDF,

            //
            // 0xE0 : Reserved
            //

            //
            // 0xE1 : OEM Specific
            //

            /// <summary>
            /// Windows 2000/XP: Either the angle bracket key or the backslash key on the RT 102-key keyboard
            /// </summary>
            OEM_102 = 0xE2,

            //
            // (0xE3-E4) : OEM specific
            //

            /// <summary>
            /// Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key
            /// </summary>
            PROCESSKEY = 0xE5,

            //
            // 0xE6 : OEM specific
            //

            /// <summary>
            /// Windows 2000/XP: Used to pass Unicode characters as if they were keystrokes. The PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
            /// </summary>
            PACKET = 0xE7,

            //
            // 0xE8 : Unassigned
            //

            //
            // 0xE9-F5 : OEM specific
            //

            /// <summary>
            /// Attn key
            /// </summary>
            ATTN = 0xF6,

            /// <summary>
            /// CrSel key
            /// </summary>
            CRSEL = 0xF7,

            /// <summary>
            /// ExSel key
            /// </summary>
            EXSEL = 0xF8,

            /// <summary>
            /// Erase EOF key
            /// </summary>
            EREOF = 0xF9,

            /// <summary>
            /// Play key
            /// </summary>
            PLAY = 0xFA,

            /// <summary>
            /// Zoom key
            /// </summary>
            ZOOM = 0xFB,

            /// <summary>
            /// Reserved
            /// </summary>
            NONAME = 0xFC,

            /// <summary>
            /// PA1 key
            /// </summary>
            PA1 = 0xFD,

            /// <summary>
            /// Clear key
            /// </summary>
            OEM_CLEAR = 0xFE,
        }

        private struct HARDWAREINPUT
        {
            /// <summary>
            /// Value specifying the message generated by the input hardware. 
            /// </summary>
            public uint Msg;

            /// <summary>
            /// Specifies the low-order word of the lParam parameter for uMsg. 
            /// </summary>
            public ushort ParamL;

            /// <summary>
            /// Specifies the high-order word of the lParam parameter for uMsg. 
            /// </summary>
            public ushort ParamH;
        }

        private struct KEYBDINPUT
        {
            /// <summary>
            /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. The Winuser.h header file provides macro definitions (VK_*) for each value. If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0. 
            /// </summary>
            public UInt16 KeyCode;

            /// <summary>
            /// Specifies a hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application. 
            /// </summary>
            public UInt16 Scan;

            /// <summary>
            /// Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
            /// KEYEVENTF_EXTENDEDKEY - If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).
            /// KEYEVENTF_KEYUP - If specified, the key is being released. If not specified, the key is being pressed.
            /// KEYEVENTF_SCANCODE - If specified, wScan identifies the key and wVk is ignored. 
            /// KEYEVENTF_UNICODE - Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section. 
            /// </summary>
            public UInt32 Flags;

            /// <summary>
            /// Time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp. 
            /// </summary>
            public UInt32 Time;

            /// <summary>
            /// Specifies an additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information. 
            /// </summary>
            public IntPtr ExtraInfo;
        }

        private struct MOUSEINPUT
        {
            /// <summary>
            /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the x coordinate of the mouse; relative data is specified as the number of pixels moved. 
            /// </summary>
            public int X;

            /// <summary>
            /// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member. Absolute data is specified as the y coordinate of the mouse; relative data is specified as the number of pixels moved. 
            /// </summary>
            public int Y;

            /// <summary>
            /// If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is 120. 
            /// Windows Vista: If dwFlags contains MOUSEEVENTF_HWHEEL, then dwData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated to the right; a negative value indicates that the wheel was rotated to the left. One wheel click is defined as WHEEL_DELTA, which is 120.
            /// Windows 2000/XP: IfdwFlags does not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData should be zero. 
            /// If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were pressed or released. This value may be any combination of the following flags. 
            /// </summary>
            public uint MouseData;

            /// <summary>
            /// A set of bit flags that specify various aspects of mouse motion and button clicks. The bits in this member can be any reasonable combination of the following values. 
            /// The bit flags that specify mouse button status are set to indicate changes in status, not ongoing conditions. For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set when the left button is first pressed, but not for subsequent motions. Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first released. 
            /// You cannot specify both the MOUSEEVENTF_WHEEL flag and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field. 
            /// </summary>
            public uint Flags;

            /// <summary>
            /// Time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own time stamp. 
            /// </summary>
            public uint Time;

            /// <summary>
            /// Specifies an additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain this extra information. 
            /// </summary>
            public IntPtr ExtraInfo;
        }

        /// <summary>
        /// The combined/overlayed structure that includes Mouse, Keyboard and Hardware Input message data (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct MOUSEKEYBDHARDWAREINPUT
        {
            /// <summary>
            /// The <see cref="MOUSEINPUT"/> definition.
            /// </summary>
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;

            /// <summary>
            /// The <see cref="KEYBDINPUT"/> definition.
            /// </summary>
            [FieldOffset(0)]
            public KEYBDINPUT Keyboard;

            /// <summary>
            /// The <see cref="HARDWAREINPUT"/> definition.
            /// </summary>
            [FieldOffset(0)]
            public HARDWAREINPUT Hardware;
        }

        private struct INPUT
        {
            /// <summary>
            /// Specifies the type of the input event. This member can be one of the following values. 
            /// </summary>
            public uint Type;

            /// <summary>
            /// The data structure that contains information about the simulated Mouse, Keyboard or Hardware event.
            /// </summary>
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);
        }

#pragma warning restore 0649
    }
}
