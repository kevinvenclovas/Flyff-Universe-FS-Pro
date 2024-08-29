using CefSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace FlyffUAutoFSPro._Script.CustomAttributes
{
    public class ActionButtonCefEventFlagAttribute : Attribute
    {
        public Keys Key { get; set; }
        public CefEventFlags CefEventFlag { get; set; }

        public ActionButtonCefEventFlagAttribute(Keys _key, CefEventFlags _cefEventFlag = CefEventFlags.None)
        {
            Key = _key;
            CefEventFlag = _cefEventFlag;
        }

    }

   
}
