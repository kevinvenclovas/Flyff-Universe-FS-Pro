using FlyffUAutoFSPro._Script;
using FlyffUAutoFSPro._Script.Bot;
using FlyffUAutoFSPro._Script.Bot.FS;
using FlyffUAutoFSPro._Script.Controllers;
using FlyffUAutoFSPro._Script.Types;
using FlyffUAutoFSPro._Script.UI;
using FlyffUAutoFSPro.AppSettings;
using FlyffUAutoFSPro.AppViews;
using FlyffUAutoFSPro.AppWindows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;


namespace FlyffUAutoFSPro
{

    public partial class FSBotWindow : Window
    {
        private BotController botController;
        private FSBotController fsController;

        public FSBotWindow(FSBotController _fsController, BotController _botController, CustomChromiumWebBrowser browser)
        {

            log4net.Config.XmlConfigurator.Configure();

            fsController = _fsController;
            botController = _botController;

            this.Title = GlobalValues.ToolsName;
            InitializeComponent();

            browser.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.GameViewBrowser.Children.Add(browser);

            DispatcherTimer timerRenderUI = new DispatcherTimer();
            timerRenderUI.Interval = TimeSpan.FromMilliseconds(50);
            timerRenderUI.Tick += (sender, e) => {
                Task.Run(async () =>
                {
                    ReDrawUI();
                }).ConfigureAwait(true);
            };
            timerRenderUI.Start();

        }

        private bool canRender = true;
        public async void ReDrawUI()
        {
            try
            {
                if (!canRender) return;

                canRender = false;
                Action dispatcherAction = new Action(() =>
                {

                    #if DEBUG
                        DevTools.Visibility = Visibility.Visible;
                    #else
                        DevTools.Visibility = Visibility.Collapsed;
                    #endif

                    #region Visibility

                    SelfHealProtectionValueView.Visibility = fsController.Player.SealfHealProtectionActive ? Visibility.Visible : Visibility.Collapsed;
                    EmojiAutoRebuffView.Visibility = fsController.Player.EmojiAutoRebuffDetectionActive ? Visibility.Visible : Visibility.Collapsed;
                    AutoRefollowTimeView.Visibility = fsController.Player.AutoRefollowActive ? Visibility.Visible : Visibility.Collapsed;
                    StopBotOnInteractionTimeView.Visibility = fsController.Player.StopBotOnInteractionActive ? Visibility.Visible : Visibility.Collapsed;

                    GTAutoRebuffView.Visibility = fsController.Player.GeburahTiphrethAutoRebuffActive ? Visibility.Visible : Visibility.Collapsed;
                    GTAutoRebuffValueView.Visibility = fsController.Player.GeburahTiphrethAutoRebuffActive ? Visibility.Visible : Visibility.Collapsed;

                    HealVariantSelectionView_Heal.Visibility = fsController.Player.HealVariant == HealVariant.HEAL ? Visibility.Visible : Visibility.Collapsed;
                    HealVariantSelectionView_HealRain.Visibility = fsController.Player.HealVariant == HealVariant.HEALRAIN ? Visibility.Visible : Visibility.Collapsed;

                    UseMPPotionView.Visibility = fsController.Player.MpPotionsKeys.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                    UseHPPotionView.Visibility = fsController.Player.HpPotionsKeys.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                    CaptchaDetectionView.Visibility = fsController.Player.CaptchaDetectionActive ? Visibility.Visible : Visibility.Collapsed;
                    MainDistanceView.Visibility = fsController.Player.CheckMainPlayerDistanceActive ? Visibility.Visible : Visibility.Collapsed;
                    RebuffAfterResurrect.Visibility = fsController.Player.AutoResurrectActive ? Visibility.Visible : Visibility.Collapsed;


                    // ToDo
                    donateView.Visibility = Visibility.Visible;
                 
#endregion


                    #region Checkboxen

                    SelfHealProtectionCheckbox.IsChecked = fsController.Player.SealfHealProtectionActive;
                    GTAutoRebuffCheckbox.IsChecked = fsController.Player.GeburahTiphrethAutoRebuffActive;
                    EmojiAutoRebuffDetectionCheckbox.IsChecked = fsController.Player.EmojiAutoRebuffDetectionActive;
                    StopBotOnInteractionCheckbox.IsChecked = fsController.Player.StopBotOnInteractionActive;
                    AutoRefollowCheckbox.IsChecked = fsController.Player.AutoRefollowActive;
                    CaptchaDetectionCheckbox.IsChecked = fsController.Player.CaptchaDetectionActive;
                    CheckDistanceCheckbox.IsChecked = fsController.Player.CheckMainPlayerDistanceActive;
                    AutoResurrectActiveCheckbox.IsChecked = fsController.Player.AutoResurrectActive;
                    RebuffAfterResurrectActiveCheckbox.IsChecked = fsController.Player.RebuffAfterResurrectActive;


                    #endregion


                    #region Values

                    GTAutoRebuffTimerProgressBar.Maximum = fsController.Player.GeburahTiphrethAutoRebuffTime;


#endregion

                    SelfHPProgressBar.Value = fsController.Player.HP;
                    SelfMPProgressBar.Value = fsController.Player.MP;
                    GTAutoRebuffTimerProgressBar.Value = fsController.NextGTRebuffTime;

                    if (fsController.Player.CheckMainPlayerDistanceActive)
                    {
                        switch (fsController.CurrentDistanceType)
                        {
                            case DistanceType.NOTSELECTED:
                                MainDistanceStatus.Background = null;
                                MainDistanceStatus.Content = "---";
                                break;
                            case DistanceType.NOTONSCREEN:
                                MainDistanceStatus.Background = new SolidColorBrush(GlobalValues.ColorRed);
                                MainDistanceStatus.Content = Properties.Resources.maindistancenotonscreen.ToUpper();
                                break;
                            case DistanceType.ONCALCULATION:
                                MainDistanceStatus.Background = new SolidColorBrush(GlobalValues.ColorOrange);
                                MainDistanceStatus.Content = Properties.Resources.maindistanceoncalibration.ToUpper();
                                break;
                            default:
                                if (fsController.IsNearPlayerAndCanBuff())
                                {
                                    MainDistanceStatus.Background = new SolidColorBrush(GlobalValues.ColorGreen);
                                    MainDistanceStatus.Content = Properties.Resources.ok.ToUpper();
                                    //fsController.CurrentDistanceBetweenMain.ToString("0.00") + "//" + fsController.MinDistanceBetweenMain.ToString("0.00");
                                }
                                else
                                {
                                    MainDistanceStatus.Background = new SolidColorBrush(GlobalValues.ColorRed);
                                    MainDistanceStatus.Content = Properties.Resources.maindistancetofaraway.ToUpper();
                                    break;
                                }
                                break;
                        }

                    }
                    
                    if (fsController.SelectedPlayer.IsAvailable)
                    {
                        try
                        {
                            var hp = fsController.SelectedPlayer.HP;
                            if (hp <= fsController.Player.CriticalHealingValue)
                            {
                                SelectedPlayerHPProgressBar.Foreground = new SolidColorBrush(GlobalValues.ColorRed);
                            }
                            else if (hp <= fsController.Player.NormalHealingValue)
                            {
                                SelectedPlayerHPProgressBar.Foreground = new SolidColorBrush(GlobalValues.ColorOrange);
                            }
                            else
                            {
                                SelectedPlayerHPProgressBar.Foreground = new SolidColorBrush(GlobalValues.ColorGreen);
                            }
                            SelectedPlayerHPProgressBar.Value = hp;
                        }
                        catch (NullReferenceException)
                        {
                            SelectedPlayerHPProgressBar.Value = 0;
                        }
                    }
                    else
                    {
                        SelectedPlayerHPProgressBar.Value = 0;
                    }

                    if (fsController.IsRunning(true))
                    {
                        if (fsController.UserInteractionBlockTimer > DateTime.Now)
                        {
                            var seconds = (fsController.UserInteractionBlockTimer - DateTime.Now).TotalSeconds;
                            StartStopButton.Content = Properties.Resources.start + $" ({(int)Math.Round(seconds, MidpointRounding.AwayFromZero)}s)";
                            StartStopButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 80, 255, 74));
                        }
                        else
                        {
                            StartStopButton.Content = Properties.Resources.stop;
                            StartStopButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 74, 74));
                        }
                    }
                    else
                    {
                        StartStopButton.Content = Properties.Resources.start;
                        StartStopButton.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 80, 255, 74));
                    }

                    if (fsController.Player.HealVariant == HealVariant.HEAL)
                    {
                        HealActionButton.Content = fsController.Player.HealKeys.ActionKeysToString();
                    }
                    else if (fsController.Player.HealVariant == HealVariant.HEALRAIN)
                    {
                        HealRainActionButton.Content = fsController.Player.HealKeys.ActionKeysToString();
                    }

                });

                await this.Dispatcher.InvokeAsync(dispatcherAction, DispatcherPriority.Normal);

            }
            catch (Exception ex)
            {
                LogService.LogError(ex);
            }
            finally
            {
                canRender = true;
            }

        }

        public void InitializeUI()
        {
            try
            {
                VersionsLabel.Content = GlobalValues.VersionName;

                var availableSmallTimeValues = GlobalValues.AvailableSmallTimeValues.Select(x => new KeyValuePair<int, string>(x, x + "s")).ToList();
                var availableTinyTimeValues = GlobalValues.AvailableTinyTimeValues.Select(x => new KeyValuePair<float, string>(x, x.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture) + "s")).ToList();
                var hpValuesKeyValue = GlobalValues.AvailableHPValues.Select(x => new KeyValuePair<int, string>(x, x + "%")).ToList();
                var healVariantValue = Enum.GetValues(typeof(HealVariant)).Cast<HealVariant>().ToList().Select(x => new KeyValuePair<HealVariant, string>(x, Properties.Resources.ResourceManager.GetString(x.GetDescription()))).ToList();
                var gtAutoRebuffKeyValues = fsController.AvailableGTAutoRebuffTimes.Select(x => new KeyValuePair<int, string>(x, x + "s")).ToList();
                var captchaSoundValues = GlobalValues.CaptchaDetectedSounds.Select(x => new KeyValuePair<int, string>(x.ID, x.Name)).ToList();

                #region Skills

                foreach (var skill in fsController.ActionControllers)
                {
                    skill.View.Button.Content = skill.PlayerValue.Value.ActionKeysToString();

                    if (skill.PlayerValue.Value.Count > 0)
                    {
                        skill.View.Button.ToolTip = Properties.Resources.clicktoresethotkey;
                    }
                    else
                    {
                        skill.View.Button.ToolTip = Properties.Resources.clicktosethotkey;
                    }

                    skill.View.Button.Click += (sender, e) =>
                    {
                        if (skill.PlayerValue.Value.Count > 0)
                        {
                            var pressedKeys = new List<ActionKey>();
                            fsController.UpdatePlayerSkillKeys(skill, pressedKeys);
                            skill.View.Button.Content = pressedKeys.ActionKeysToString();
                        }
                        else
                        {
                            CheckKeyPressedWindow window = new CheckKeyPressedWindow(1, GlobalValues.MaxKeyCombinationCount, 1);
                            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                            window.Owner = this;
                            if (window.ShowDialog() == true)
                            {
                                var pressedKeys = window.PressedActionKeys;
                                if (fsController.UpdatePlayerSkillKeys(skill, pressedKeys))
                                {
                                    skill.View.Button.Content = pressedKeys.ActionKeysToString();
                                }
                            }
                        }

                        if (skill.PlayerValue.Value.Count > 0)
                        {
                            skill.View.Button.ToolTip = Properties.Resources.clicktoresethotkey;
                        }
                        else
                        {
                            skill.View.Button.ToolTip = Properties.Resources.clicktosethotkey;
                        }

                        if (skill.View.ActionButtonsDelegate != null)
                            skill.View.ActionButtonsDelegate(skill);
                    };
                }

                #endregion

                #region Heal/Healrain

                HealVariantButtonSelection.SetKeyValueToCombobox(healVariantValue, (int)fsController.Player.HealVariant);

                HealVariantButtonSelection.SelectionChanged += (sender, e) =>
                {
                    fsController.ChangeHealVariant((HealVariant)HealVariantButtonSelection.SelectedValue);
                };

                #endregion

                #region Potions

                UseMPPotionValueSelection.SetKeyValueToCombobox(hpValuesKeyValue, GlobalValues.AvailableHPValues.FindIndex(a => a == fsController.Player.MpPotionsValue));
                UseMPPotionValueSelection.SelectionChanged += (sender, e) =>
                {
                    fsController.SetMpPostionUseValue((int)UseMPPotionValueSelection.SelectedValue);
                };

                UseHPPotionValueSelection.SetKeyValueToCombobox(hpValuesKeyValue, GlobalValues.AvailableHPValues.FindIndex(a => a == fsController.Player.HpPotionsValue));
                UseHPPotionValueSelection.SelectionChanged += (sender, e) =>
                {
                    fsController.SetHpPostionUseValue((int)UseHPPotionValueSelection.SelectedValue);
                };

                #endregion

                #region Player Healing

                bool canChangenormalHealingSelection = true;
                NormalHealingSelection.SetKeyValueToCombobox(hpValuesKeyValue, GlobalValues.AvailableHPValues.FindIndex(a => a == fsController.Player.NormalHealingValue));
                NormalHealingSelection.SelectionChanged += (sender, e) =>
                {
                    if (canChangenormalHealingSelection)
                    {
                        e.Handled = true;
                        canChangenormalHealingSelection = false;
                        if (!fsController.ChangeNormalHealingValue((int)((ComboBox)sender).SelectedValue))
                        {
                            if (e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
                            {

                                ((ComboBox)sender).SelectedItem = e.RemovedItems[0];
                            }
                        }
                        canChangenormalHealingSelection = true;
                    }
                };

                bool canChangeCriticalHealingSelection = true;
                CriticalHealingSelection.SetKeyValueToCombobox(hpValuesKeyValue, GlobalValues.AvailableHPValues.FindIndex(a => a == fsController.Player.CriticalHealingValue));
                CriticalHealingSelection.SelectionChanged += (sender, e) =>
                {
                    if (canChangeCriticalHealingSelection)
                    {
                        e.Handled = true;
                        canChangeCriticalHealingSelection = false;
                        if (!fsController.ChangeCriticalHealingValue((int)((ComboBox)sender).SelectedValue))
                        {
                            if (e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
                            {
                                ((ComboBox)sender).SelectedItem = e.RemovedItems[0];
                            }
                        }
                        canChangeCriticalHealingSelection = true;
                    }
                };

                #endregion

                #region Self Heal 

                bool canChangeSelfHealProtectionCheckbox = true;
                SelfHealProtectionCheckbox.Checked += (sender, e) =>
                {
                    if (canChangeSelfHealProtectionCheckbox)
                    {
                        e.Handled = true;
                        canChangeSelfHealProtectionCheckbox = false;
                        if (!fsController.ChangeSelfHealActiveState(((CheckBox)sender).IsChecked ?? false))
                        {
                            ((CheckBox)sender).IsChecked = false;
                        }
                        canChangeSelfHealProtectionCheckbox = true;
                    }
                };

                SelfHealProtectionCheckbox.Unchecked += (sender, e) =>
                {
                    fsController.ChangeSelfHealActiveState(((CheckBox)sender).IsChecked ?? false);
                };

                bool canChangeSelfHealProtectionValueSelection = true;
                SelfHealProtectionValueSelection.SetKeyValueToCombobox(hpValuesKeyValue, GlobalValues.AvailableHPValues.FindIndex(a => a == fsController.Player.SealfHealProtectionValue));
                SelfHealProtectionValueSelection.SelectionChanged += (sender, e) =>
                {
                    if (canChangeSelfHealProtectionValueSelection)
                    {
                        e.Handled = true;
                        canChangeSelfHealProtectionValueSelection = false;
                        if (!fsController.ChangeSelfHealProtectionValue((int)((ComboBox)sender).SelectedValue))
                        {
                            if (e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
                            {
                                ((ComboBox)sender).SelectedItem = e.RemovedItems[0];
                            }
                        }
                        canChangeSelfHealProtectionValueSelection = true;
                    }
                };

                #endregion

                #region GT Autorebuff

                GTAutoRebuffValueSelection.SetKeyValueToCombobox(gtAutoRebuffKeyValues, fsController.AvailableGTAutoRebuffTimes.FindIndex(a => a == fsController.Player.GeburahTiphrethAutoRebuffTime));
                GTAutoRebuffValueSelection.SelectionChanged += (sender, e) =>
                {
                    fsController.ChangeGTAutoRebuffValue((int)GTAutoRebuffValueSelection.SelectedValue);
                };

                GTAutoRebuffCheckbox.Click += (sender, e) =>
                {
                    fsController.ChangeGTAutoRebuffState(((CheckBox)sender).IsChecked ?? false);
                };

                #endregion

                #region Emoji Auto Rebuff

                EmojiAutoRebuffDetectionCheckbox.Click += (sender, e) =>
                {
                    fsController.ChangeEmojiDetectionState(EmojiAutoRebuffDetectionCheckbox.IsChecked ?? false);
                };

                EmojiAutoRebuffDetectionSelection.SetMotionsToCombobox(GlobalValues.Motions, fsController.Player.EmojiAutoRebuffEmojiId);
                EmojiAutoRebuffDetectionSelection.SelectionChanged += (sender, e) =>
                {
                    fsController.ChangeEmojiDetectionValue(((ImageComboboxItem)EmojiAutoRebuffDetectionSelection.SelectedValue).ID);
                };

                #endregion

                #region Pause Bot

                StopBotOnInteractionSelection.SetKeyValueToCombobox(availableSmallTimeValues, GlobalValues.AvailableSmallTimeValues.FindIndex(a => a == fsController.Player.StopBotOnInteractionTime));
                StopBotOnInteractionSelection.SelectionChanged += (sender, e) =>
                {
                    fsController.ChangeStopBotOnInteractionValue((int)StopBotOnInteractionSelection.SelectedValue);
                };

                StopBotOnInteractionCheckbox.Click += (sender, e) =>
                {
                    fsController.ChangeStopBotOnInteractionState(StopBotOnInteractionCheckbox.IsChecked ?? false);
                };
                #endregion

                #region Buffs
                CriticalHealingDurationSelection.SetKeyValueToCombobox(availableSmallTimeValues, GlobalValues.AvailableSmallTimeValues.FindIndex(a => a == fsController.Player.CriticalHealingDuration));
                CriticalHealingDurationSelection.SelectionChanged += (sender, e) =>
                {
                    fsController.ChangeCriticalHealingDurationValue((int)CriticalHealingDurationSelection.SelectedValue);
                };

                WaitDurationBetweeRebuffalbeBuffsSelection.SetKeyValueToCombobox(availableTinyTimeValues, GlobalValues.AvailableTinyTimeValues.FindIndex(a => a == fsController.Player.WaitDurationBetweenRebuffableBuffs));
                WaitDurationBetweeRebuffalbeBuffsSelection.SelectionChanged += (sender, e) =>
                {
                    fsController.ChangeWaitDurationBetweeRebuffalbeBuffsValue((float)WaitDurationBetweeRebuffalbeBuffsSelection.SelectedValue);
                };

                #endregion

                #region Refollow

                RefollowSelection.SetKeyValueToCombobox(availableSmallTimeValues, GlobalValues.AvailableSmallTimeValues.FindIndex(a => a == fsController.Player.AutoRefollowTime));
                RefollowSelection.SelectionChanged += (sender, e) =>
                {
                    fsController.AutoRefollowValue((int)RefollowSelection.SelectedValue);
                };


                AutoRefollowCheckbox.Click += (sender, e) =>
                {
                    fsController.AutoRefollowActiveState(AutoRefollowCheckbox.IsChecked ?? false);
                };

                #endregion

                #region Captcha

                CaptchaDetectionSoundSelection.SetKeyValueToCombobox(captchaSoundValues, GlobalValues.CaptchaDetectedSounds.FindIndex(a => a.ID == fsController.Player.CaptchaDetectionSoundValue));
                CaptchaDetectionSoundSelection.SelectionChanged += (sender, e) =>
                {
                    fsController.ChangeCaptchaDetectionSoundValue((int)CaptchaDetectionSoundSelection.SelectedValue);
                };

                CaptchaDetectionCheckbox.Click += (sender, e) =>
                {
                    fsController.ChangeCaptchaDetectionActiveState(CaptchaDetectionCheckbox.IsChecked ?? false);
                };


                #endregion

                #region Distance
                CheckDistanceCheckbox.Click += (sender, e) =>
                {
                    fsController.ChangeCheckMainPlayerDistanceActiveState(CheckDistanceCheckbox.IsChecked ?? false);
                };

                #endregion

                #region AutoRess
                AutoResurrectActiveCheckbox.Click += (sender, e) =>
                {
                    fsController.ChangeAutoResurrectActiveState(AutoResurrectActiveCheckbox.IsChecked ?? false);
                };

                RebuffAfterResurrectActiveCheckbox.Click += (sender, e) =>
                {
                    fsController.ChangeRebuffAfterResurrectActiveState(RebuffAfterResurrectActiveCheckbox.IsChecked ?? false);
                };
                #endregion

                #region Global Hotkeys

                foreach (var skill in fsController.GlobalActionControllers)
                {
                    skill.View.Button.Content = skill.PlayerValue.Value.ActionKeysToString();

                    if (skill.PlayerValue.Value.Count > 0)
                    {
                        skill.View.Button.ToolTip = Properties.Resources.clicktoresethotkey;
                    }
                    else
                    {
                        skill.View.Button.ToolTip = Properties.Resources.clicktosethotkey;
                    }

                    skill.View.Button.Click += (sender, e) =>
                    {
                        if (skill.PlayerValue.Value.Count > 0)
                        {
                            var pressedKeys = new List<ActionKey>();
                            fsController.UpdatePlayerGlobalHotKeys(skill, pressedKeys);
                            skill.View.Button.Content = pressedKeys.ActionKeysToString();
                        }
                        else
                        {
                            CheckKeyPressedWindow window = new CheckKeyPressedWindow(2, GlobalValues.MaxKeyCombinationCount, 1, 1);
                            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                            window.Owner = this;
                            if (window.ShowDialog() == true)
                            {
                                var pressedKeys = window.PressedActionKeys;
                                if (fsController.UpdatePlayerGlobalHotKeys(skill, pressedKeys))
                                {
                                    skill.View.Button.Content = pressedKeys.ActionKeysToString();
                                }
                            }
                        }

                        if (skill.PlayerValue.Value.Count > 0)
                        {
                            skill.View.Button.ToolTip = Properties.Resources.clicktoresethotkey;
                        }
                        else
                        {
                            skill.View.Button.ToolTip = Properties.Resources.clicktosethotkey;
                        }

                        if (skill.View.ActionButtonsDelegate != null)
                            skill.View.ActionButtonsDelegate(skill);
                    };
                }

                foreach (var skill in fsController.CustomGlobalActionControllers)
                {
                    AddCustomGlobalHotkeyViewToScroller(skill);
                }

                #endregion
            }
            catch (Exception ex)
            {
                LogService.LogError(ex);
            }
            finally
            {
                canRender = true;
            }
        }

        public void AddSnackbar(string text)
        {
            Action dispatcherAction = new Action(() =>
            {
                Snackbar bar = new Snackbar();
                bar.Initialize(SnackBarView, 3, text);
            });
            this.Dispatcher.Invoke(dispatcherAction, DispatcherPriority.Normal);
        }

        public void AddCustomGlobalHotkeyViewToScroller(CustomGlobalSkillsActionController controller)
        {
            CustomGlobalHotkeyView view = new CustomGlobalHotkeyView();
            controller.View = view;
            view.Initialize(this, CustomHotkeysScrollerContent, controller, fsController);
           
        }

        private void ShowMessageBox(object sender, RoutedEventArgs e)
        {
            var message = _Script.UI.ToolTip.GetToolTipText(sender as UIElement);
            var header = _Script.UI.ToolTip.GetToolTipHeader(sender as UIElement);
            fsController.ShowMessage(header, message);
        }

        public void ShowLoadingOverview(string message)
        {
            try
            {
                LoadingOverview.Dispatcher.Invoke(() =>
                {
                    LoadingOverviewText.Text = message;
                    LoadingOverview.Visibility = Visibility.Visible;
                });
            }
            catch(Exception ex)
            {
                LogService.LogError(ex);
            }
        }

        public void HideLoadingOverview()
        {
            try
            {
                LoadingOverview.Dispatcher.Invoke(() =>
                {
                    LoadingOverview.Visibility = Visibility.Collapsed;
                });
            }
            catch (Exception ex)
            {
                
            }
        }

        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            fsController.StartStopBot(!fsController.IsRunning(true));
        }

        private async void RebuffButtonClick(object sender, RoutedEventArgs e)
        {
            await fsController.ReBuffPlayer(true);
        }

        private async void RebuffGTButtonClick(object sender, RoutedEventArgs e)
        {
            await fsController.ReBuffGeburahTiphreth(true);
        }
        
        private void JoinDiscordButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(GlobalValues.DiscordUrl) { UseShellExecute = true });
        }
        
        private void Donate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(GlobalValues.DonateUrl) { UseShellExecute = true });
        }
       
        private void AddGlobalHotkeyClick(object sender, RoutedEventArgs e)
        {
            CheckKeyPressedWindow window = new CheckKeyPressedWindow(1, GlobalValues.MaxKeyCombinationCount, 1);
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                if (window.PressedActionKeys.Count() >0)
                {
                    var controller = fsController.AddNewCustomGlobalHotkey(new _Script.Models.CustomGlobalHotkey()
                    {
                        TriggerKeys = window.PressedActionKeys.Select(x => (int)x.KeyType).ToList(),
                    });
                    if(controller != null)
                        AddCustomGlobalHotkeyViewToScroller(controller);
                }
            }
        }

        private void ResetSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.reset_tooltip, Properties.Resources.reset, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Settings.Default.Reset();
                Settings.Default.Save();

                var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
                Process.Start(currentExecutablePath);
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void TakeScreenshot_Click(object sender, RoutedEventArgs e)
        {
            fsController.SaveScreenshot();
        }


        #region DEV

        private void HideNames_Click(object sender, RoutedEventArgs e)
        {

            Path temp = (Path)GameViewBrowser.FindName("HIDENAME");
            if(temp != null) GameViewBrowser.Children.Remove(temp);
            Path temp1 = (Path)GameViewBrowser.FindName("HIDEUSERNAME");
            if (temp1 != null)  GameViewBrowser.Children.Remove(temp1);
            Path temp2 = (Path)GameViewBrowser.FindName("HIDENAMEEXP");
            if (temp2 != null) GameViewBrowser.Children.Remove(temp2);


            var rectMyName = new Path
            {
                Data = new RectangleGeometry(new Rect(10, 5, 150, 20)),
                Fill = Brushes.Black,
                Name = "HIDENAME"
            };
            GameViewBrowser.Children.Add(rectMyName);

            var rectMyNameExp = new Path
            {
                Data = new RectangleGeometry(new Rect(80, 80, 130, 20)),
                Fill = Brushes.Black,
                Name = "HIDENAMEEXP"
            };
            GameViewBrowser.Children.Add(rectMyNameExp);

            var rectUserMyName = new Path
            {
                Data = new RectangleGeometry(new Rect(0, 5, 3000, 20)),
                Fill = Brushes.Black,
                Name = "HIDEUSERNAME"
            };
            GameViewBrowser.Children.Add(rectUserMyName);
        }

        #endregion

       
    }
}
