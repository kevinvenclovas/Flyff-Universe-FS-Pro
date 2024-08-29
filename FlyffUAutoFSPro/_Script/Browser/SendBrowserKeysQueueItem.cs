using CefSharp;
using System.Collections.Generic;

namespace FlyffUAutoFSPro._Script.Browser
{
    public class SendBrowserKeysQueueItem
    {
        public List<int> Keys { get; set; }
        public bool Processed { get; set; }
    }
}
