using FlyffUAutoFSPro._Script;
using FlyffUAutoFSPro._Script.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Input;

namespace FlyffUAutoFSPro
{
    public static class GlobalValues
    {
        public static int Version = 27;

        // Eindeutiger Key für die API
        public static string BotKey = "FLYFFUAUTOFSPRO";

        // Versionsname
        public static string VersionName { get; set; } = "Version 1.0." + Version;

        // Botname
        public static string ToolsName = "Flyff Universe Auto FS Pro";

        // Testzeit für den Bot

        public static string FolderName = "FlyffUAutoFSPro";

        public static string FileLocation = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + FolderName;

        // Nicht 100 da einige Clients die HP nicht genau auf 100 ermitteln können
        public static int MaxPercentValue = 98;

        public static Dictionary<ActionKeyType,ActionKey> AvailableKeys = new Dictionary<ActionKeyType, ActionKey>
        {
            {
                ActionKeyType.None,
                new ActionKey()
                {
                    KeyType = ActionKeyType.None,
                    Name = "",
                }
            },
            {
                ActionKeyType.A,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.A,
                    KeybordKeys = new List<Key>(){ Key.A },
                    Name = "A",
                }
            },
            {
                ActionKeyType.B,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.B,
                    KeybordKeys = new List<Key>(){ Key.B },
                    Name = "B",
                }
            },
            {
                ActionKeyType.C,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.C,
                    KeybordKeys = new List<Key>(){ Key.C },
                    Name = "C",
                }
            },
            {
                ActionKeyType.D,
                new ActionKey()
                {
                   KeyType = _Script.Types.ActionKeyType.D,
                   KeybordKeys = new List<Key>(){ Key.D },
                   Name = "D",
                }
            },
            {
                ActionKeyType.E,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.E,
                    KeybordKeys = new List<Key>(){ Key.E },
                    Name = "E",
                }
            },
            {
                ActionKeyType.F,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F,
                    KeybordKeys = new List<Key>(){ Key.F },
                    Name = "F",
                }
            },
            {
                ActionKeyType.G,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.G,
                    KeybordKeys = new List<Key>(){ Key.G },
                    Name = "G",
                }
            },
            {
                ActionKeyType.H,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.H,
                    KeybordKeys = new List<Key>(){ Key.H },
                    Name = "H",
                }
            },
            {
                ActionKeyType.I,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.I,
                    KeybordKeys = new List<Key>(){ Key.I },
                    Name = "I",
                }
            },
            {
                ActionKeyType.J,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.J,
                    KeybordKeys = new List<Key>(){ Key.J },
                    Name = "J",
                }
            },
            {
                ActionKeyType.K,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.K,
                    KeybordKeys = new List<Key>(){ Key.K },
                    Name = "K",
                }
            },
            {
                ActionKeyType.L,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.L,
                    KeybordKeys = new List<Key>(){ Key.L },
                    Name = "L",
                }
            },
            {
                ActionKeyType.M,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.M,
                    KeybordKeys = new List<Key>(){ Key.M },
                    Name = "M",
                }
            },
            {
                ActionKeyType.N,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.N,
                    KeybordKeys = new List<Key>(){ Key.N },
                    Name = "N",
                }
            },
            {
                ActionKeyType.O,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.O,
                    KeybordKeys = new List<Key>(){ Key.O },
                    Name = "O",
                }
            },
            {
                ActionKeyType.P,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.P,
                    KeybordKeys = new List<Key>(){ Key.P },
                    Name = "P",
                }
            },
            {
                ActionKeyType.Q,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.Q,
                    KeybordKeys = new List<Key>(){ Key.Q },
                    Name = "Q",
                }
            },
            {
                ActionKeyType.R,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.R,
                    KeybordKeys = new List<Key>(){ Key.R },
                    Name = "R",
                }
            },
            {
                ActionKeyType.S,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.S,
                    KeybordKeys = new List<Key>(){ Key.S },
                    Name = "S",
                }
            },
            {
                ActionKeyType.T,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.T,
                    KeybordKeys = new List<Key>(){ Key.T },
                    Name = "T",
                }
            },
            {
                ActionKeyType.U,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.U,
                    KeybordKeys = new List<Key>(){ Key.U },
                    Name = "U",
                }
            },
            {
                ActionKeyType.V,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.V,
                    KeybordKeys = new List<Key>(){ Key.V },
                    Name = "V",
                }
            },
            {
                ActionKeyType.W,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.W,
                    KeybordKeys = new List<Key>(){ Key.W },
                    Name = "W",
                }
            },
            {
                ActionKeyType.X,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.X,
                    KeybordKeys = new List<Key>(){ Key.X },
                    Name = "X",
                }
            },
            {
                ActionKeyType.Y,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.Y,
                    KeybordKeys = new List<Key>(){ Key.Y },
                    Name = "Y",
                }
            },
            {
                ActionKeyType.Z,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.Z,
                    KeybordKeys = new List<Key>(){ Key.Z },
                    Name = "Z",
                }
            },
            {
                ActionKeyType.NUM1,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.NUM1,
                    KeybordKeys = new List<Key>(){ Key.NumPad1 , Key.D1},
                    Name = "1",
                }
            },
            {
                ActionKeyType.NUM2,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.NUM2,
                    KeybordKeys = new List<Key>(){ Key.NumPad2 , Key.D2},
                    Name = "2",
                }
            },
            {
                ActionKeyType.NUM3,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.NUM3,
                    KeybordKeys = new List<Key>(){ Key.NumPad3 , Key.D3},
                    Name = "3",
                }
            },
            {
                ActionKeyType.NUM4,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.NUM4,
                    KeybordKeys = new List<Key>(){ Key.NumPad4 , Key.D4},
                    Name = "4",
                }
            },
            {
                ActionKeyType.NUM5,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.NUM5,
                    KeybordKeys = new List<Key>(){ Key.NumPad5 , Key.D5},
                    Name = "5",
                }
            },
            {
                ActionKeyType.NUM6,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.NUM6,
                    KeybordKeys = new List<Key>(){ Key.NumPad6 , Key.D6},
                    Name = "6",
                }
            },
            {
                ActionKeyType.NUM7,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.NUM7,
                    KeybordKeys = new List<Key>(){ Key.NumPad7 , Key.D7},
                    Name = "7",
                }
            },
            {
                ActionKeyType.NUM8,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.NUM8,
                    KeybordKeys = new List<Key>(){ Key.NumPad8 , Key.D8},
                    Name = "8",
                }
            },
            {
                ActionKeyType.NUM9,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.NUM9,
                    KeybordKeys = new List<Key>(){ Key.NumPad9 , Key.D9},
                    Name = "9",
                }
            },
            {
                ActionKeyType.NUM0,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.NUM0,
                    KeybordKeys = new List<Key>(){ Key.NumPad0 , Key.D0},
                    Name = "0",
                }
            },
            {
                ActionKeyType.F1,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F1,
                    KeybordKeys = new List<Key>(){ Key.F1 },
                    Name = "F1",
                }
            },
            {
                ActionKeyType.F2,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F2,
                    KeybordKeys = new List<Key>(){ Key.F2 },
                    Name = "F2",
                }
            },
            {
                ActionKeyType.F3,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F3,
                    KeybordKeys = new List<Key>(){ Key.F3 },
                    Name = "F3",
                }
            },
            {
                ActionKeyType.F4,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F4,
                    KeybordKeys = new List<Key>(){ Key.F4 },
                    Name = "F4",
                }
            },
            {
                ActionKeyType.F5,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F5,
                    KeybordKeys = new List<Key>(){ Key.F5 },
                    Name = "F5",
                }
            },
            {
                ActionKeyType.F6,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F6,
                    KeybordKeys = new List<Key>(){ Key.F6 },
                    Name = "F6",
                }
            },
            {
                ActionKeyType.F7,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F7,
                    KeybordKeys = new List<Key>(){ Key.F7 },
                    Name = "F7",
                }
            },
            {
                ActionKeyType.F8,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F8,
                    KeybordKeys = new List<Key>(){ Key.F8 },
                    Name = "F8",
                }
            },
            {
                ActionKeyType.F9,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F9,
                    KeybordKeys = new List<Key>(){ Key.F9 },
                    Name = "F9",
                }
            },
            {
                ActionKeyType.F10,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F10,
                    KeybordKeys = new List<Key>(){ Key.F10 },
                    Name = "F10",
                }
            },
            {
                ActionKeyType.F11,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F11,
                    KeybordKeys = new List<Key>(){ Key.F11 },
                    Name = "F11",
                }
            },
            {
                ActionKeyType.F12,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.F12,
                    KeybordKeys = new List<Key>(){ Key.F12 },
                    Name = "F12",
                }
            },
            {
                ActionKeyType.Alt,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.Alt,
                    KeybordKeys = new List<Key>(){ Key.LeftAlt, Key.RightAlt },
                    Name = "Alt",
                    CefEventFlags = CefSharp.CefEventFlags.AltDown,
                    GlobalHotKeyModifier = _Script.NonInvasiveKeyboardHookLibrary.ModifierKeys.Alt
                }
            },
            {
                ActionKeyType.Shift,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.Shift,
                    KeybordKeys = new List<Key>(){ Key.LeftShift, Key.RightShift },
                    Name = "Shift",
                    CefEventFlags = CefSharp.CefEventFlags.ShiftDown,
                    GlobalHotKeyModifier = _Script.NonInvasiveKeyboardHookLibrary.ModifierKeys.Shift
                }
            },
            {
                ActionKeyType.Ctrl,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.Ctrl,
                    KeybordKeys = new List<Key>(){ Key.LeftCtrl, Key.RightCtrl },
                    Name = "Ctrl",
                    CefEventFlags = CefSharp.CefEventFlags.ControlDown,
                    GlobalHotKeyModifier = _Script.NonInvasiveKeyboardHookLibrary.ModifierKeys.Control
                }
            },
            {
                ActionKeyType.Escape,
                new ActionKey()
                {
                    KeyType = _Script.Types.ActionKeyType.Escape,
                    KeybordKeys = new List<Key>(){ Key.Escape },
                    Name = "ESC",
                }
            }
        };
        
        //Wieviele tasten können gleichzeitig gedrückt werden
        public const int MaxKeyCombinationCount = 3;
        public const int MaxModifierKeyCombinationCount = 2;

        public static string DiscordUrl = "https://discord.gg/Y5kj9kZyBx";

        public static string DonateUrl = "https://www.paypal.com/donate/?hosted_button_id=RY34FS8JZB4QW";

        public static List<int> AvailableHPValues = new List<int>()
        {
            10,20,30,40,50,60,70,80,90,95,99
        };

        public static List<int> AvailableSmallTimeValues = new List<int>()
        {
            1,2,3,4,5,6,7,8,9,10
        };

        public static List<float> AvailableTinyTimeValues = new List<float>()
        {
            0.5f,
            0.6f,
            0.7f,
            0.8f,
            0.9f,
            1.0f,
            1.1f,
            1.2f,
            1.3f,
            1.5f,
            1.6f,
            1.7f,
            1.8f,
            1.9f,
            2.0f,
            2.1f,
            2.2f,
            2.3f,
            2.4f,
            2.5f,
            3.0f,
            3.5f,
            4.0f,
            4.5f,
            5.0f,
            5.5f,
            6.0f,
            6.5f,
            7.0f,
            7.5f,
            8.0f,
            8.5f,
            9.0f,
            9.5f,
            10.0f
        };

        #region UI Colors

        public static System.Windows.Media.Color ColorGreen = System.Windows.Media.Color.FromArgb(255, 34, 191, 27);
        public static System.Windows.Media.Color ColorOrange = System.Windows.Media.Color.FromArgb(255, 255, 152, 19);
        public static System.Windows.Media.Color ColorRed = System.Windows.Media.Color.FromArgb(255, 204, 30, 70);
        public static System.Windows.Media.Color ColorBlue = System.Windows.Media.Color.FromArgb(255, 38, 134, 217);

#endregion


        #region Motions

        public static List<Motion> Motions = new List<Motion>()
        {
            new Motion(0,Properties.Resources.motion_laugh,Properties.Resources.motion_laugh_search),
            new Motion(1,Properties.Resources.motion_sad,Properties.Resources.motion_sad_search),
            new Motion(2,Properties.Resources.motion_kiss,Properties.Resources.motion_kiss_search),
            new Motion(3,Properties.Resources.motion_surprise,Properties.Resources.motion_surprise_search),
            new Motion(4,Properties.Resources.motion_blush,Properties.Resources.motion_blush_search),
            new Motion(5,Properties.Resources.motion_anger,Properties.Resources.motion_anger_search),
            new Motion(6,Properties.Resources.motion_sigh,Properties.Resources.motion_sigh_search),
            new Motion(7,Properties.Resources.motion_wink, Properties.Resources.motion_wink_search),
            new Motion(8,Properties.Resources.motion_ache, Properties.Resources.motion_ache_search),
            new Motion(9,Properties.Resources.motion_hunger, Properties.Resources.motion_hunger_search),
            new Motion(10,Properties.Resources.motion_yummy, Properties.Resources.motion_yummy_search),
            new Motion(11,Properties.Resources.motion_sneer, Properties.Resources.motion_sneer_search),
            new Motion(12,Properties.Resources.motion_sparkle, Properties.Resources.motion_sparkle_search),
            new Motion(13,Properties.Resources.motion_ridicule, Properties.Resources.motion_ridicule_search),
            new Motion(14,Properties.Resources.motion_sleepy, Properties.Resources.motion_sleepy_search),
            new Motion(15,Properties.Resources.motion_rich, Properties.Resources.motion_rich_search),
            new Motion(16,Properties.Resources.motion_glare, Properties.Resources.motion_glare_search),
            new Motion(17,Properties.Resources.motion_sweat, Properties.Resources.motion_sweat_search),
            new Motion(18,Properties.Resources.motion_catface, Properties.Resources.motion_catface_search),
            new Motion(19,Properties.Resources.motion_tongue, Properties.Resources.motion_tongue_search),
            new Motion(20,Properties.Resources.motion_mad,Properties.Resources.motion_mad_search),
            new Motion(21,Properties.Resources.motion_aha,Properties.Resources.motion_aha_search),
            new Motion(22,Properties.Resources.motion_embarrassed,Properties.Resources.motion_embarrassed_search),
            new Motion(23,Properties.Resources.motion_help,Properties.Resources.motion_help_search),
            new Motion(24,Properties.Resources.motion_crazy,Properties.Resources.motion_crazy_search),
            new Motion(25,Properties.Resources.motion_oh,Properties.Resources.motion_oh_search),
            new Motion(26,Properties.Resources.motion_confused,Properties.Resources.motion_confused_search),
            new Motion(27,Properties.Resources.motion_ouch,Properties.Resources.motion_ouch_search),
            new Motion(28,Properties.Resources.motion_love,Properties.Resources.motion_love_search)
        };

        #endregion

        // Text voin hier https://ttsmp3.com/
        public static List<AudioFile> CaptchaDetectedSounds = new List<AudioFile>()
        {
            new AudioFile()
            {
                ID = 0,
                Name = "Sound 1",
                Stream = Properties.Resources.captcha_detected_s1
            },
            new AudioFile()
            {
                ID = 1,
                Name = "Sound 2",
                Stream = Properties.Resources.captcha_detected_s2
            },
        };

        public static List<Bitmap> SelectedArrowTopLeftGroup = new List<Bitmap>()
        {
            Properties.Resources.SelectArrow1TopLeft,
            Properties.Resources.SelectArrow2TopLeft,
            Properties.Resources.SelectArrow3TopLeft,
            Properties.Resources.SelectArrow4TopLeft,
            Properties.Resources.SelectArrow5TopLeft,
        };

        public static List<Bitmap> SelectedArrowTopRightGroup = new List<Bitmap>()
        {
            Properties.Resources.SelectArrow1TopRight,
            Properties.Resources.SelectArrow2TopRight,
            Properties.Resources.SelectArrow3TopRight,
            Properties.Resources.SelectArrow4TopRight,
            Properties.Resources.SelectArrow5TopRight,
        };

        public static List<Bitmap> SelectedArrowBottomLeftGroup = new List<Bitmap>()
        {
            Properties.Resources.SelectArrow1BottomLeft,
            Properties.Resources.SelectArrow2BottomLeft,
            Properties.Resources.SelectArrow3BottomLeft,
            Properties.Resources.SelectArrow4BottomLeft,
            Properties.Resources.SelectArrow5BottomLeft,
        };

        public static List<Bitmap> SelectedArrowBottomRightGroup = new List<Bitmap>()
        {
            Properties.Resources.SelectArrow1BottomRight,
            Properties.Resources.SelectArrow2BottomRight,
            Properties.Resources.SelectArrow3BottomRight,
            Properties.Resources.SelectArrow4BottomRight,
            Properties.Resources.SelectArrow5BottomRight,
        };
    }

    public static class GlobalValuesUtils
    {

        public static List<ActionKey> KeysToActionkeys(this List<Key> keys)
        {
            return GlobalValues.AvailableKeys.Select(x => x.Value).Where(x => keys.Any(t => x.KeybordKeys.Contains(t))).ToList();
        }

        public static bool IsModifierKey(this Key key)
        {
            return (GlobalValues.AvailableKeys.Select(x => x.Value).FirstOrDefault(x => x.KeybordKeys.Contains(key))?.GlobalHotKeyModifier ?? _Script.NonInvasiveKeyboardHookLibrary.ModifierKeys.WindowsKey) != _Script.NonInvasiveKeyboardHookLibrary.ModifierKeys.WindowsKey;
        }

        public static List<ActionKey> ActionKeyIdsToActionkeys(this List<int> keys)
        {
            return GlobalValues.AvailableKeys.Select(x => x.Value).Where(x => keys.Any(t => x.KeyType == (ActionKeyType)t)).ToList();
        }


        public static string ActionKeysToString(this List<int> keys)
        {
            return ActionKeysToString(ActionKeyIdsToActionkeys(keys));
        }

        public static string ActionKeysToString(this List<Key> keys)
        {
            return ActionKeysToString(KeysToActionkeys(keys));
        }

        public static string ActionKeysToString(this List<ActionKey> keys)
        {
            if (keys.Count == 0) return Properties.Resources.disabled;
            var ordered = keys.OrderByDescending(x => x.CefEventFlags != CefSharp.CefEventFlags.None).ThenBy(x => x.KeyType).ToList();
            return String.Join(" + ", ordered.Select(x => x.Name));
        }


    }
}
