using FlyffUAutoFSPro._Script.Bot.FS;
using FlyffUAutoFSPro._Script.Controllers;
using FlyffUAutoFSPro._Script.Models;
using FlyffUAutoFSPro.AppWindows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FlyffUAutoFSPro.AppViews
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CustomGlobalHotkeyView : UserControl
    {
        Window ownerWindow;
        CustomGlobalSkillsActionController skillController;
        FSBotController fsBotController;
        Panel parent;

        public CustomGlobalHotkeyView()
        {
            InitializeComponent();
        }

        public void Initialize(Window _ownerWindow, Panel _parent, CustomGlobalSkillsActionController _skillController, FSBotController _fsBotController)
        {
            skillController = _skillController;
            fsBotController = _fsBotController;
            ownerWindow = _ownerWindow;
            parent = _parent;
           
            DeleteGlobalHotkey.Click += (sender, e) =>
            {
                if (MessageBox.Show(String.Format(Properties.Resources.deleteglobalhotkey, _skillController.Skill.TriggerKeys.ActionKeyIdsToActionkeys().ActionKeysToString()), Properties.Resources.delete, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    parent.Children.Remove(skillController.View);
                    fsBotController.RemoveCustomGlobalHotkey(skillController);
                }
            };


            AddAction.Click += (sender, e) =>
            {
                CheckKeyPressedWindow window = new CheckKeyPressedWindow(1, GlobalValues.MaxKeyCombinationCount);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Owner = ownerWindow;

                if (window.ShowDialog() == true)
                {
                    CustomGlobalHotkeyItem item = new CustomGlobalHotkeyItem();
                    item.Keys = window.PressedActionKeys.Select(x => (int)x.KeyType).ToList();
                    item.Duration = fsBotController.Player.WaitDurationBetweenRebuffableBuffs;

                    skillController.Skill.Actions.Add(item);

                    CustomGlobalHotkeyItemView itemView = new CustomGlobalHotkeyItemView();
                    itemView.Initialize(ownerWindow, GlobalHotkeysActionItems, _skillController.Skill, item, fsBotController);

                    fsBotController.SaveSettings();
                }
            };

            foreach (var action in skillController.Skill.Actions)
            {
                CustomGlobalHotkeyItemView itemView = new CustomGlobalHotkeyItemView();
                itemView.Initialize(ownerWindow, GlobalHotkeysActionItems, _skillController.Skill, action, fsBotController);
            }

            parent.Children.Add(this);

            DrawView();
        }



        public void DrawView()
        {
            GlobalHotkeyTriggerKeys.Content = skillController.Skill.TriggerKeys.ActionKeyIdsToActionkeys().ActionKeysToString();

            if (skillController.Skill.TriggerKeys.Count > 0)
            {
                GlobalHotkeyTriggerKeys.ToolTip = Properties.Resources.clicktoresethotkey;
            }
            else
            {
                GlobalHotkeyTriggerKeys.ToolTip = Properties.Resources.clicktosethotkey;
            }

        }
    }
}
