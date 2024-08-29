using FlyffUAutoFSPro._Script;
using FlyffUAutoFSPro._Script.Bot.FS;
using FlyffUAutoFSPro._Script.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlyffUAutoFSPro.AppViews
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CustomGlobalHotkeyItemView : UserControl
    {
        Window ownerWindow;
        FSBotController fsBotController;
        CustomGlobalHotkeyItem item;
        Panel parent;
        CustomGlobalHotkey hotkey;

        public CustomGlobalHotkeyItemView()
        {
            InitializeComponent();
        }

        public void Initialize(Window _ownerWindow, Panel _parent, CustomGlobalHotkey _hotkey, CustomGlobalHotkeyItem _item, FSBotController _fsBotController)
        {
            fsBotController = _fsBotController;
            ownerWindow = _ownerWindow;
            item = _item;
            parent = _parent;
            hotkey = _hotkey;

            GlobalHotkeyItemActionKeys.Click += (sender, e) =>
            {
                if (MessageBox.Show(Properties.Resources.deleteglobalhotkeyitem, Properties.Resources.delete, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    parent.Children.Remove(this);
                    _hotkey.Actions.Remove(_item);
                    _fsBotController.SaveSettings();
                }
            };

            var availableTinyTimeValues = GlobalValues.AvailableTinyTimeValues.Select(x => new KeyValuePair<float, string>(x, x.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture) + "s")).ToList();
            GlobalHotkeyItemActionDuration.SetKeyValueToCombobox(availableTinyTimeValues, GlobalValues.AvailableTinyTimeValues.FindIndex(a => a == item.Duration));
            GlobalHotkeyItemActionDuration.SelectionChanged += (sender, e) =>
            {
                item.Duration = (float)GlobalHotkeyItemActionDuration.SelectedValue;
                _fsBotController.SaveSettings();
            };

            parent.Children.Add(this);
            DrawView();
        }

        public void DrawView()
        {
            GlobalHotkeyItemActionKeys.Content = item.Keys.ActionKeyIdsToActionkeys().ActionKeysToString();
          
            if (item.Keys.Count > 0)
            {
                GlobalHotkeyItemActionKeys.ToolTip = Properties.Resources.clicktoresethotkey;
            }
            else
            {
                GlobalHotkeyItemActionKeys.ToolTip = Properties.Resources.clicktosethotkey;
            }

        }
    }
}
