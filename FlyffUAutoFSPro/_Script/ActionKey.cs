using CefSharp;
using FlyffUAutoFSPro._Script.Types;
using System.Collections.Generic;
using System.Windows.Input;

namespace FlyffUAutoFSPro._Script
{
    public class ActionKey
    {
        public ActionKeyType KeyType { get; set; }
        public List<Key> KeybordKeys { get; set; } = new List<Key>();
        public string Name { get; set; }
        public CefEventFlags CefEventFlags { get; set; } = CefEventFlags.None;
        public NonInvasiveKeyboardHookLibrary.ModifierKeys GlobalHotKeyModifier { get; set; } = NonInvasiveKeyboardHookLibrary.ModifierKeys.WindowsKey;
    }
}
