using System;
using System.Windows;

namespace FlyffUAutoFSPro._Script.UI
{
    public static class ToolTip
    {
        public static readonly DependencyProperty ToolTipText = DependencyProperty.RegisterAttached("ToolTipText",
            typeof(string), typeof(ToolTip), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ToolTipHeader = DependencyProperty.RegisterAttached("ToolTipHeader",
            typeof(string), typeof(ToolTip), new FrameworkPropertyMetadata(null));

        public static string GetToolTipText(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (string)element.GetValue(ToolTipText);
        }
        public static void SetToolTipText(UIElement element, string value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(ToolTipText, value);
        }

        public static string GetToolTipHeader(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (string)element.GetValue(ToolTipHeader);
        }
        public static void SetToolTipHeader(UIElement element, string value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(ToolTipHeader, value);
        }
    }
}
