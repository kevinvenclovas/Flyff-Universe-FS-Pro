using CefSharp;
using CefSharp.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlyffUAutoFSPro._Script
{
    public  class CustomChromiumWebBrowser : ChromiumWebBrowser
    {
        public delegate void UserInteractWithBrowser();
        public UserInteractWithBrowser UserInteractWithBrowserDelegate { get; set; }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            var browser = GetBrowser();

            if (browser != null)
            {
                base.OnPreviewKeyDown(e);
                if (UserInteractWithBrowserDelegate != null)
                    UserInteractWithBrowserDelegate.Invoke();
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            var browser = GetBrowser();

            if (browser != null)
            {
                base.OnPreviewKeyUp(e);
                if (UserInteractWithBrowserDelegate != null)
                    UserInteractWithBrowserDelegate.Invoke();
            }
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            var browser = GetBrowser();

            if (browser != null)
            {
                base.OnPreviewTextInput(e);
                if (UserInteractWithBrowserDelegate != null)
                    UserInteractWithBrowserDelegate.Invoke();
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var browser = GetBrowser();

            if (browser != null)
            {
                base.OnMouseDown(e);
                if (UserInteractWithBrowserDelegate != null)
                    UserInteractWithBrowserDelegate.Invoke();
            }
        }

        protected override void OnBrowserWasHidden(bool hidden)
        {
            base.OnBrowserWasHidden(false);
        }

        public async Task SendActionButtonToBrowser(List<int> actionKeys)
        {
            await SendActionButtonToBrowser(GlobalValues.AvailableKeys.Where(x => actionKeys.Contains((int)x.Key)).Select(x => x.Value).ToList());
        }

        public async Task SendActionButtonToBrowser(List<ActionKey> actionKeys)
        {
            actionKeys = actionKeys.OrderBy(x => x.CefEventFlags == CefEventFlags.None).ToList();

            List<KeyEvent> keysToSend = new List<KeyEvent>();

            foreach (var key in actionKeys)
            {
                KeyEvent keyEvent = new KeyEvent();
                keyEvent.FocusOnEditableField = true;
                keyEvent.IsSystemKey = false;

                if(key.CefEventFlags != CefEventFlags.None) 
                {
                    keyEvent.Modifiers = key.CefEventFlags;
                    keyEvent.WindowsKeyCode = (int)KeyInterop.VirtualKeyFromKey(key.KeybordKeys.First());
                }
                else
                {
                    keyEvent.WindowsKeyCode = (int)KeyInterop.VirtualKeyFromKey(key.KeybordKeys.First());
                }
                
                keysToSend.Add(keyEvent);
            }

            for (int i = 0; i < keysToSend.Count(); i++)
            {
                KeyEvent keyEvent = keysToSend [i];
                keyEvent.Type = KeyEventType.KeyDown;
                GetBrowser().GetHost().SendKeyEvent(keyEvent);
                await Task.Delay(RandomService.Instance.GetRandom(30, 50));
            }

            keysToSend.Reverse();

            for (int i = 0; i < keysToSend.Count(); i++)
            {
                KeyEvent keyEvent = keysToSend[i];
                keyEvent.Type = KeyEventType.KeyUp;
                GetBrowser().GetHost().SendKeyEvent(keyEvent);
                await Task.Delay(RandomService.Instance.GetRandom(30, 50));
            }

        }

        public async Task ClickOnScreen(int x, int y, MouseButtonType button)
        {
            x = (int)(x / Utils.GetWindowsScale());
            y = (int)(y / Utils.GetWindowsScale());
            GetBrowser().GetHost().SendMouseMoveEvent(x, y, true, CefEventFlags.None);
            GetBrowser().GetHost().SendMouseClickEvent(x, y, button, false, 1, CefEventFlags.None);
            await Task.Delay(RandomService.Instance.GetRandom(100, 150));
            GetBrowser().GetHost().SendMouseClickEvent(x, y, button, true, 1, CefEventFlags.None);
            await Task.Delay(RandomService.Instance.GetRandom(100, 150));
        }
    }
}
