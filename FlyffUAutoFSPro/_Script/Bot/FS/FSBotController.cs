using CefSharp;
using FlyffUAutoFSPro._Script.Bot.Main;
using FlyffUAutoFSPro._Script.Browser;
using FlyffUAutoFSPro._Script.Controllers;
using FlyffUAutoFSPro._Script.Models;
using FlyffUAutoFSPro._Script.Models.Actions;
using FlyffUAutoFSPro._Script.Models.Actions.Hotkeys;
using FlyffUAutoFSPro._Script.Types;
using FlyffUAutoFSPro._Script.UI;
using NonInvasiveKeyboardHookLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Action = System.Action;

namespace FlyffUAutoFSPro._Script.Bot.FS
{
    public class FSBotController 
    { 
        private BotController botController;
        public FSPlayer Player;
        public MainPlayer SelectedPlayer;

        public List<SkillActionController> ActionControllers = new List<SkillActionController>();
        public List<GlobalSkillsActionController> GlobalActionControllers = new List<GlobalSkillsActionController>();
        public List<CustomGlobalSkillsActionController> CustomGlobalActionControllers = new List<CustomGlobalSkillsActionController>();

        public List<CronJob> CronJobs = new List<CronJob>();

        private FSBotWindow Window;
       
        private bool _isRunning = false;
        private int _runningTime = 0;

        private KeyboardHookManager _globalHotkeysManager = new KeyboardHookManager();

        public List<int> AvailableGTAutoRebuffTimes = new List<int>()
        {
            30,40,50,60
        };

        #region Browser

        public CustomChromiumWebBrowser Browser;
   
        private bool _isBrowserReady = false;
        List<SendBrowserKeysQueueItem> BrowserKeysQueueItems = new List<SendBrowserKeysQueueItem>();

        #endregion

        #region Current Frame

        private Bitmap _currentFrame = null;
        private readonly object _currentFrameLock = new object();
        private bool _newFrameExist = false;

        private Bitmap GetCurrentFrame()
        {
            if (_currentFrame != null)
            {
                lock (_currentFrameLock)
                {
                    return (Bitmap)_currentFrame.Clone();
                }
            }
            return null;
        }

        #endregion

        #region Blocktimers 
        public int UseMpPotionBlockDuration = 500;
        public DateTime UseMpPotionBlockTimer = DateTime.Now;

        public int UseHpPotionBlockDuration = 500;
        public DateTime UseHpPotionBlockTimer = DateTime.Now;

        public DateTime UseHealBlockTimer = DateTime.Now;
        public DateTime UserInteractionBlockTimer = DateTime.Now;

        public int SelectedPlayerDeadTimer = 0;
        public int NextGTRebuffTime = 0;
        // Ist GT geskippt worden weil Spieler keine 100% HP hat 
        private bool _rebuffGTSkipped = false;


        public Vector2? MainPosition;
        public float CurrentDistanceBetweenMain;
        public float MinDistanceBetweenMain = float.MaxValue;
        public List<double> LastDistanceBetweenMain = new List<double>();
        public DistanceType CurrentDistanceType;
        #endregion

        private bool _onPlayerFullHeal;
        private bool _cricitalHealingActive = false;

        public FSBotController(BotController _botController)
        {
            log4net.Config.XmlConfigurator.Configure();
            Player = FSPlayer.Load();
            Player.CurrentState = FSStateType.NONE;
            SelectedPlayer = new MainPlayer();

            botController = _botController;

            var browser = new CustomChromiumWebBrowser();
            this.Browser = browser;
            this.Browser.FrameLoadStart += BrowserFrameLoadStart;
            this.Browser.UserInteractWithBrowserDelegate += UserInteractWithBrowser;

            
            this.Window = new FSBotWindow(this, botController, this.Browser);

            IniGlobalSkillController();
            IniSkillControllers();
            IniCustomGlobalSkillController();

            Window.InitializeUI();
            Window.Show();
            this.Browser.Load("https://universe.flyff.com/play");
            IniCronJobs();
            _globalHotkeysManager.Start();


        }

        #region CronJobs

        private void IniCronJobs()
        {
            CronJobs.Add(new CronJob(50, CheckSelfStatus));
            CronJobs.Add(new CronJob(10, SendKeysToBrowser));
            CronJobs.Add(new CronJob(1000, CheckBuffBarVisible));
            CronJobs.Add(new CronJob(250, CheckRebuffEmoji));
            CronJobs.Add(new CronJob(50, ProgressNewFrame));
            CronJobs.Add(new CronJob(3000, CheckCaptchaVisible));
            CronJobs.Add(new CronJob(10, CheckDistance));
        }

        private async Task CheckSelfStatus()
        {
            using (var frame = await Browser.TakeScreenshotAsync())
            {

                Rectangle hpSearchResult = new Rectangle();
                Rectangle mpSearchResult = new Rectangle();
                //Rectangle fpSearchResult = new Rectangle();
                hpSearchResult = BitmapSearcher.SearchBitmap(new Bitmap(Properties.Resources.HPText), frame, 0.4);
                mpSearchResult = BitmapSearcher.SearchBitmap(new Bitmap(Properties.Resources.MPText), frame, 0.4);
                //fpSearchResult = BitmapSearcher.SearchBitmap(new Bitmap(Properties.Resources.FPText), frame, 0.4);

                if (hpSearchResult == Rectangle.Empty || mpSearchResult == Rectangle.Empty /*|| fpSearchResult == Rectangle.Empty*/)
                {
                    await TriggerStatusUI();
                    return;
                }

                int energyBarEndBuffer = 10;

                if (hpSearchResult != Rectangle.Empty && hpSearchResult.X != 0 && hpSearchResult.Y != 0)
                {
                    int middle = Convert.ToInt32(hpSearchResult.Y + (hpSearchResult.Height / 2d));

                    int xPosStart = hpSearchResult.X + hpSearchResult.Width + 15;
                    int currentxPos = xPosStart;

                    List<Color> pixelResults = new List<Color>();

                    bool endByWidthReached = false;

                    while (true)
                    {
                        currentxPos++;
                        Color pixelColor = frame.GetPixel(currentxPos, middle);
                        var isEnd = ColorHelper.IsEnergyLineStartEndColor(pixelColor);
                        if (isEnd)
                        {
                            break;
                        }

                        if (ColorHelper.IsPixelColorEnergyHP(pixelColor))
                        {
                            pixelResults.Add(Color.Red);
                        }
                        if (ColorHelper.IsPixelColorEnergyBlack(pixelColor))
                        {
                            pixelResults.Add(Color.Black);
                        }
                        if (ColorHelper.IsPixelColorEnergyWhite(pixelColor))
                        {
                            pixelResults.Add(Color.White);

                        }

                        if (currentxPos + 1 == frame.Width)
                        {
                            break;
                        }
                    }

                    if(pixelResults.Count > energyBarEndBuffer)
                        pixelResults.RemoveRange(pixelResults.Count - energyBarEndBuffer, energyBarEndBuffer);

                    if (pixelResults.Count(x => x == Color.White) > 1)
                    {
                        await Browser.ClickOnScreen(xPosStart, middle, MouseButtonType.Left);
                        return;
                    }

                    Player.HP = Convert.ToInt32((100d / pixelResults.Count()) * pixelResults.Count(x => x == Color.Red));

                    if (Player.HP == 100 && Player.LastFullHPTimestamp == null)
                    {
                        Player.LastFullHPTimestamp = DateTime.Now;
                    }
                    else if (Player.HP < 100)
                    {
                        Player.LastFullHPTimestamp = null;
                    }

                    if ((Player.SealfHealProtectionActive && Player.HP <= Player.SealfHealProtectionValue))
                    {
                        if (Player.HealVariant == HealVariant.HEAL)
                        {
                            if (SelectedPlayer.IsAvailable)
                            {
                                await TriggerClearPlayer();
                            }
                        }

                        _cricitalHealingActive = true;
                    }
                    else if (Player.HP < Player.HpPotionsValue && DateTime.Now > UseHpPotionBlockTimer && IsRunning())
                    {
                        UseHpPotionBlockTimer = DateTime.Now.AddMilliseconds(UseHpPotionBlockDuration);
                        await SendActionButtonToBrowser(Player.HpPotionsKeys);
                    }

                }

                if (mpSearchResult != Rectangle.Empty && mpSearchResult.X != 0 && mpSearchResult.Y != 0)
                {
                    int middle = Convert.ToInt32(mpSearchResult.Y + (mpSearchResult.Height / 2d));

                    int xPosStart = mpSearchResult.X + mpSearchResult.Width + 15;
                    int currentxPos = xPosStart;

                    List<Color> pixelResults = new List<Color>();

                    while (true)
                    {
                        currentxPos++;
                        //await AddDebugDot(currentxPos, middle, "hp-bar-check-start");
                        Color pixelColor = frame.GetPixel(currentxPos, middle);
                        var isEnd = ColorHelper.IsEnergyLineStartEndColor(pixelColor);
                        if (isEnd)
                        {
                            break;
                        }

                        if (ColorHelper.IsPixelColorEnergyMP(pixelColor))
                        {
                            pixelResults.Add(Color.Blue);
                        }
                        if (ColorHelper.IsPixelColorEnergyBlack(pixelColor))
                        {
                            pixelResults.Add(Color.Black);
                        }
                        if (ColorHelper.IsPixelColorEnergyWhite(pixelColor))
                        {
                            pixelResults.Add(Color.White);
                        }

                        if (currentxPos + 1 == frame.Width)
                        {
                            break;
                        }
                    }

                    if (pixelResults.Count > energyBarEndBuffer)
                        pixelResults.RemoveRange(pixelResults.Count - energyBarEndBuffer, energyBarEndBuffer);

                    if (pixelResults.Count(x => x == Color.White) > 1)
                    {
                        await Browser.ClickOnScreen(xPosStart, middle, MouseButtonType.Left);
                        return;
                    }

                    Player.MP = Convert.ToInt32((100d / pixelResults.Count()) * pixelResults.Count(x => x == Color.Blue));

                    if (Player.MP < Player.MpPotionsValue && DateTime.Now > UseMpPotionBlockTimer && IsRunning())
                    {
                        UseMpPotionBlockTimer = DateTime.Now.AddMilliseconds(UseMpPotionBlockDuration);
                        await SendActionButtonToBrowser(Player.MpPotionsKeys);
                    }

                }

            }
        }

        private async Task CheckBuffBarVisible()
        {
            using (var frame = await Browser.TakeScreenshotAsync())
            {

                var barswitcher = BitmapSearcher.SearchBitmap(new Bitmap(Properties.Resources.BarSwitcher), frame, 0.2);
                var barswitchersgray = BitmapSearcher.SearchMultipleBitmap(new Bitmap(Properties.Resources.BarSwitcherGray), frame, 0.2);

                foreach (var graySwitch in barswitchersgray)
                {
                    if (graySwitch.X - 5 < barswitcher.X && graySwitch.X + 5 > barswitcher.X)
                    {
                        var diff = Math.Abs(graySwitch.Y - barswitcher.Y);

                        var x = (int) (graySwitch.X + (graySwitch.Width/2f));
                        var y = (int) (barswitcher.Y - (diff/2f));

                        await Browser.ClickOnScreen(x, y, MouseButtonType.Left);
                    }
                }
            }
        }

        private async Task CheckRebuffEmoji()
        {
            if (!IsRunning()) return;

            using (var frame = await Browser.TakeScreenshotAsync())
            {

                if (!Player.EmojiAutoRebuffDetectionActive) return;

                Bitmap searchImage = GlobalValues.Motions.FirstOrDefault(x => x.Id == Player.EmojiAutoRebuffEmojiId)?.SearchImage ?? null;

                if (searchImage == null) return;

                Rectangle rebuffSmileyResult = new Rectangle();

                rebuffSmileyResult = BitmapSearcher.SearchBitmap(searchImage, frame, 0.2);

                if (rebuffSmileyResult != Rectangle.Empty)
                {
                    await ReBuffPlayer();
                }
            }
        }

        private async Task ProgressNewFrame()
        {
            if (!IsRunning()) return;

            using (var frame = await Browser.TakeScreenshotAsync())
            {
                var deselectLocation = CheckUserSelected(frame);

                // Wenn SPieler selektiert
                if (deselectLocation != Rectangle.Empty)
                {
                    var health = CalculateSelectedPlayerHealth(frame, deselectLocation);
                    SelectedPlayer.HP = health;

                    if (SelectedPlayer.HP == 100 && SelectedPlayer.LastFullHPTimestamp == null)
                    {
                        _onPlayerFullHeal = false;
                        SelectedPlayer.LastFullHPTimestamp = DateTime.Now;
                    }


                    if (!SelectedPlayer.IsDead)
                    {
                        if (_cricitalHealingActive)
                        {
                            await UseHeal();
                            if (SelectedPlayer.LastFullHPTimestamp.HasValue && SelectedPlayer.LastFullHPTimestamp.Value.AddSeconds(Player.CriticalHealingDuration) < DateTime.Now)
                            {
                                _cricitalHealingActive = false;
                            }
                        }

                        if (SelectedPlayer.HP < 100)
                        {
                            SelectedPlayer.LastFullHPTimestamp = null;

                            if (SelectedPlayer.HP == 0)
                            {
                                if (SelectedPlayer.IsDead == false)
                                {
                                    SelectedPlayer.PlayerDeadTime = DateTime.Now;
                                    SelectedPlayer.IsDead = true;
                                    if (Player.AutoResurrectActive)
                                    {
                                        await TriggerResurrection();
                                    }
                                }
                            }
                            else
                            {
                                if (SelectedPlayer.HP <= Player.CriticalHealingValue)
                                {
                                    _cricitalHealingActive = true;
                                }
                                else if (SelectedPlayer.HP <= Player.NormalHealingValue || _onPlayerFullHeal)
                                {
                                    _onPlayerFullHeal = true;
                                    await UseHeal();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (SelectedPlayer.HP == 0)
                        {
                            if ((SelectedPlayer.PlayerDeadTime.HasValue && SelectedPlayerDeadTimer % 10 == 0 && SelectedPlayerDeadTimer > 0) || SelectedPlayerDeadTimer > 5)
                            {
                                SelectedPlayerDeadTimer = 0;
                                await TriggerResurrection();
                            }
                        }
                        else if (SelectedPlayer.LastFullHPTimestamp.HasValue && SelectedPlayer.LastFullHPTimestamp.Value.AddSeconds(1) < DateTime.Now)
                        {
                            SelectedPlayerDeadTimer = 0;
                            SelectedPlayer.IsDead = false;
                            SelectedPlayer.PlayerDeadTime = null;
                            if (Player.RebuffAfterResurrectActive)
                            {
                                await Task.Delay(2000);
                                await ReBuffPlayer();
                            }
                        }
                        else if (!SelectedPlayer.LastFullHPTimestamp.HasValue)
                        {
                            await UseHeal();
                        }
                    }

                }
                else
                {
                    if (_cricitalHealingActive)
                    {
                        await UseHeal();
                    }

                    if (Player.LastFullHPTimestamp.HasValue && Player.LastFullHPTimestamp.Value.AddSeconds(Player.CriticalHealingDuration) < DateTime.Now)
                    {
                        _cricitalHealingActive = false;
                    }

                    if (SelectedPlayer.IsAvailable)
                        ResetNextGTRebuffTime();
                }

            }
        }

        private async Task CheckCaptchaVisible()
        {
            using (var frame = await Browser.TakeScreenshotAsync())
            {
                var captcha1 = BitmapSearcher.SearchBitmap(new Bitmap(Properties.Resources.Captcha1), frame, 0.2);
                var captcha2 = BitmapSearcher.SearchBitmap(new Bitmap(Properties.Resources.Captcha2), frame, 0.2);

                if (captcha1 != Rectangle.Empty || captcha2 != Rectangle.Empty)
                {
                    StartStopBot(false);
                    PlayCaptchaDetectedSound();
                }
            }
        }

        private async Task SendKeysToBrowser()
        {
            List<SendBrowserKeysQueueItem> itemsToExecute = new List<SendBrowserKeysQueueItem>();

            lock (BrowserKeysQueueItems)
            {
                itemsToExecute.AddRange(BrowserKeysQueueItems);
            }

            foreach (var item in itemsToExecute)
            {
                await Browser.SendActionButtonToBrowser(item.Keys);
                item.Processed = true;
            }

            lock (BrowserKeysQueueItems)
            {
                BrowserKeysQueueItems.RemoveAll(x => x.Processed);
            }
        }
        #endregion

        public bool IsRunning(bool ignoreUserInteractionBlock = false)
        {
            if (ignoreUserInteractionBlock)
            {
                return _isRunning;
            }
            else
            {
                return _isRunning && UserInteractionBlockTimer < DateTime.Now;
            }
        }

        public void ShowMessage(string header, string message)
        {
            System.Windows.MessageBox.Show(Window, message, header, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void IncreaseRunningTime()
        {
            if (!IsRunning()) return;
            {
                _runningTime++;

                if (Player.GeburahTiphrethAutoRebuffActive)
                {

                    if (!_rebuffGTSkipped && SelectedPlayer.IsAvailable)
                    {
                        NextGTRebuffTime--;
                    }

                    if (NextGTRebuffTime <= 0 || _rebuffGTSkipped)
                    {
                        _ = ReBuffGeburahTiphreth();
                    }
                }
               
                if (Player.AutoRefollowActive && _runningTime > 0 && Player.AutoRefollowTime > 0 && (_runningTime % Player.AutoRefollowTime) == 0)
                {
                    _ = TriggerFollowPlayer();
                }

                if (SelectedPlayer.IsDead)
                {
                    SelectedPlayerDeadTimer++;
                }
            }
        }

        public void StartStopBot(bool newState)
        {
            if (newState == true)
            {
                _isRunning = true;
                UserInteractionBlockTimer = DateTime.Now;
                ResetNextGTRebuffTime();
                RegisterGlobalHotkeys();
            }
            else
            {
                _isRunning = false;
                UnRegisterGlobalHotkeys();
            }
            Player.CurrentState = FSStateType.NONE;
            Window.ReDrawUI();
        }

        public void ResetNextGTRebuffTime()
        {
            _rebuffGTSkipped = false;
            NextGTRebuffTime = Player.GeburahTiphrethAutoRebuffTime;
        }

        public bool IsNearPlayerAndCanBuff()
        {
            if (Player.CheckMainPlayerDistanceActive)
            {
                return (CurrentDistanceBetweenMain < (MinDistanceBetweenMain * 1.3f)) || CurrentDistanceBetweenMain < 30 || CurrentDistanceType == DistanceType.NOTONSCREEN;
            }
            else
            {
                return true;
            }

        }

        #region Player

        public bool UpdatePlayerSkillKeys(SkillActionController controller, List<ActionKey> keys )
        {
            
            if(keys.Count > 0)
            {
                foreach (var con in ActionControllers)
                {
                    if (con.SkillType != controller.SkillType)
                    {
                        var keysSame = keys.Select(x => (int)x.KeyType).All(con.PlayerValue.Value.Contains) && keys.Count == con.PlayerValue.Value.Count;
                        if (keysSame)
                        {
                            ShowMessage(Properties.Resources.hotkeyalreadyinuse, Properties.Resources.hotkeyalreadyinuse);
                            return false;
                        }
                    }
                }
            }
           
            controller.PlayerValue.Value = keys.Select(x => (int)x.KeyType).ToList();
            SaveSettings();
            return true;
        }

        public bool UpdatePlayerGlobalHotKeys(GlobalSkillsActionController controller, List<ActionKey> keys)
        {

            if (keys.Count > 0)
            {
                foreach (var con in GlobalActionControllers)
                {
                    if (con.SkillType != controller.SkillType)
                    {
                        var keysSame = keys.Select(x => (int)x.KeyType).All(con.PlayerValue.Value.Contains) && keys.Count == con.PlayerValue.Value.Count;
                        if (keysSame)
                        {
                            ShowMessage(Properties.Resources.hotkeyalreadyinuse, Properties.Resources.hotkeyalreadyinuse);
                            return false;
                        }
                    }
                }
            }

            controller.PlayerValue.Value = keys.Select(x => (int)x.KeyType).ToList();
            SaveSettings();
            RegisterGlobalHotkeys();
            return true;
        }

        public void ChangeHealVariant(HealVariant newVariant)
        {
            Player.HealVariant = newVariant;
            Player.HealKeys = new List<int>();
            SaveSettings();
        }

        public void SetHpPostionUseValue(int value)
        {
            Player.HpPotionsValue = value;

            if(Player.SealfHealProtectionValue >= Player.HpPotionsValue)
            {
                Player.SealfHealProtectionValue = GlobalValues.AvailableHPValues.First();
                Window.SelfHealProtectionValueSelection.SelectedValue = Player.SealfHealProtectionValue;
            }
            SaveSettings();
        }

        public void SetMpPostionUseValue(int value)
        {
            Player.MpPotionsValue = value;
            SaveSettings();
        }

        public bool ChangeNormalHealingValue(int value)
        {
            if(Player.CriticalHealingValue > value)
            {
               ShowMessage(Properties.Resources.info, Properties.Resources.normalhealing_not_smaller_than_critical);
               return false;
            }
            else
            {
                Player.NormalHealingValue = value;
                SaveSettings();
                return true;
            }
        }

        public bool ChangeCriticalHealingValue(int value)
        {
            if (Player.NormalHealingValue < value)
            {
                ShowMessage(Properties.Resources.info, Properties.Resources.criticalhealing_not_bigger_than_normal);
                return false;
            }
            else
            {
                Player.CriticalHealingValue = value;
                SaveSettings();
                return true;
            }
        }
        
        public bool ChangeSelfHealActiveState(bool newState)
        {
            if (newState)
            {
                if (Player.HealKeys.Count == 0)
                {
                    ShowMessage(Properties.Resources.info, Properties.Resources.mustsethealingbefore);
                    return false;
                }
            }

            Player.SealfHealProtectionActive = newState;

            if(newState == false)
            {
                Window.SelfHealProtectionValueSelection.SelectedIndex = 0;
            }

            SaveSettings();
            return true;
        }

        public bool ChangeAutoResurrectActiveState(bool newState)
        {
            if (newState)
            {
                if (Player.ResurrectionHotkeys.Count == 0)
                {
                    ShowMessage(Properties.Resources.autoress, Properties.Resources.mustsetresurrectionbefore);
                    return false;
                }

                if (Player.HealKeys.Count == 0)
                {
                    ShowMessage(Properties.Resources.info, Properties.Resources.mustsethealingbefore);
                    return false;
                }
            }

            Player.AutoResurrectActive = newState;

            SaveSettings();
            return true;
        }

        public bool ChangeRebuffAfterResurrectActiveState(bool newState)
        {
            Player.RebuffAfterResurrectActive = newState;
            SaveSettings();
            return true;
        }

        public bool ChangeSelfHealProtectionValue(int value)
        {
            if (Player.HpPotionsKeys.Count > 0 && value >= Player.HpPotionsValue)
            {
                ShowMessage(Properties.Resources.info, String.Format(Properties.Resources.selfhealprotection_value_smaller_then_hppotion_value, Player.HpPotionsValue));
                return false;
            }
            else
            {
                Player.SealfHealProtectionValue = value;
                SaveSettings();
                return true;
            }
        }

        public void ChangeGTAutoRebuffState(bool newState)
        {
            Player.GeburahTiphrethAutoRebuffActive = newState;
            SaveSettings();
            ResetNextGTRebuffTime();
        }

        public void ChangeGTAutoRebuffValue(int value)
        {
            Player.GeburahTiphrethAutoRebuffTime = value;
            SaveSettings();
            ResetNextGTRebuffTime();
        }

        public void ChangeEmojiDetectionState(bool newState)
        {
            Player.EmojiAutoRebuffDetectionActive = newState;
            SaveSettings();
        }

        public void ChangeEmojiDetectionValue(int value)
        {
            Player.EmojiAutoRebuffEmojiId = value;
            SaveSettings();
        }

        public void ChangeStopBotOnInteractionState(bool newState)
        {
            Player.StopBotOnInteractionActive = newState;
            SaveSettings();
        }

        public void ChangeStopBotOnInteractionValue(int value)
        {
            Player.StopBotOnInteractionTime = value;
            SaveSettings();
        }

        public void ChangeCriticalHealingDurationValue(int value)
        {
            Player.CriticalHealingDuration = value;
            SaveSettings();
        }

        public void ChangeWaitDurationBetweeRebuffalbeBuffsValue(float value)
        {
            Player.WaitDurationBetweenRebuffableBuffs = value;
            SaveSettings();
        }
    
        public void AutoRefollowActiveState(bool newState)
        {
            Player.AutoRefollowActive = newState;
            SaveSettings();
        }

        public void AutoRefollowValue(int value)
        {
            Player.AutoRefollowTime = value;
            SaveSettings();
        }

        public void RebuffAutoRessActiveState(bool newState)
        {
            Player.RebuffAfterResurrectActive = newState;
            SaveSettings();
        }
        
        public bool ChangeCaptchaDetectionActiveState(bool newState)
        {
            Player.CaptchaDetectionActive = newState;
            SaveSettings();
            return true;
        }

        public bool ChangeCaptchaDetectionSoundValue(int newSound)
        {
            Player.CaptchaDetectionSoundValue = newSound;
            SaveSettings();
            PlayCaptchaDetectedSound();
            return true;
        }

        public bool ChangeCheckMainPlayerDistanceActiveState(bool newState)
        {
            Player.CheckMainPlayerDistanceActive = newState;
            SaveSettings();
            return true;
        }

        public CustomGlobalSkillsActionController AddNewCustomGlobalHotkey(CustomGlobalHotkey hotkey)
        {
            var controller = new _Script.Controllers.CustomGlobalSkillsActionController()
            {
                Skill = hotkey,
            };


            foreach (var con in GlobalActionControllers)
            {
                var keysSame = hotkey.TriggerKeys.All(con.PlayerValue.Value.Contains) && hotkey.TriggerKeys.Count == con.PlayerValue.Value.Count;
                if (keysSame)
                {
                    ShowMessage(Properties.Resources.hotkeyalreadyinuse, Properties.Resources.hotkeyalreadyinuse);
                    return null;
                }
            }

            foreach (var con in CustomGlobalActionControllers)
            {
                var keysSame = hotkey.TriggerKeys.All(con.Skill.TriggerKeys.Contains) && hotkey.TriggerKeys.Count == con.Skill.TriggerKeys.Count;
                if (keysSame)
                {
                    ShowMessage(Properties.Resources.hotkeyalreadyinuse, Properties.Resources.hotkeyalreadyinuse);
                    return null;
                }
            }


            CustomGlobalActionControllers.Add(controller);
            Player.CustomGlobalHotkeys.Add(hotkey);
            SaveSettings();
            RegisterGlobalHotkeys();
            return controller;
        }

        public void RemoveCustomGlobalHotkey(CustomGlobalSkillsActionController controller)
        {
            CustomGlobalActionControllers.Remove(controller);
            Player.CustomGlobalHotkeys.Remove(controller.Skill);
            SaveSettings();
            RegisterGlobalHotkeys();
        }

        public void SaveSettings()
        {
            Player.Save();
        }

        #endregion

        #region Check Frame

        private Rectangle GetDeselectPlayerPosition(Bitmap image)
        {

            int ymin = 35;
            int ymax = 40;
            List<Rectangle> selectX = BitmapSearcher.SearchMultipleBitmap(new Bitmap(Properties.Resources.PlayerSelectX), image, 0.2);
            return selectX.Where(x => x.Y > ymin && x.Y < ymax && x.X > (Browser.ActualWidth / 2f)).OrderBy(x => x.X).FirstOrDefault();
        }

        private Rectangle CheckUserSelected(Bitmap image)
        {
            var loc = GetDeselectPlayerPosition(image);
            var available = loc != Rectangle.Empty;
            SelectedPlayer.IsAvailable = available;
            return loc;
        }

        Dictionary<int, Color> GoodHpPixelColors;
        private unsafe int CalculateSelectedPlayerHealth(Bitmap image, Rectangle deselectLocation)
        {
            if (GoodHpPixelColors == null)
            {
                GoodHpPixelColors = new Dictionary<int, System.Drawing.Color>();
                var b = new Bitmap(Properties.Resources.GoodHPPixelColors);

                for (int x = 0; x < b.Width; x++)
                {
                    for (int y = 0; y < b.Height; y++)
                    {
                        var pixel = b.GetPixel(x, y);

                        if (!GoodHpPixelColors.ContainsKey(pixel.ToArgb()) && pixel.ToArgb() != System.Drawing.Color.White.ToArgb())
                        {
                            GoodHpPixelColors.Add(pixel.ToArgb(), pixel);
                        }
                    }
                }
            }

            int energyBarEndBuffer = 10;

            int middleY = Convert.ToInt32(deselectLocation.Y + (deselectLocation.Height / 2d));

            int xPosStart = deselectLocation.X;
            int currentxPos = xPosStart;

            List<Color> pixelResults = new List<Color>();

            bool firstPixelDedected = false;

            while (true)
            {
                currentxPos--;
                Color pixelColor = image.GetPixel(currentxPos, middleY);
                var isEnd = ColorHelper.IsEnergyLineStartEndColor(pixelColor);
                if (isEnd && firstPixelDedected)
                {
                    break;
                }
                else if(firstPixelDedected == false)
                {
                    firstPixelDedected = true;
                    currentxPos -= energyBarEndBuffer;
                }

                if (ColorHelper.IsPixelColorEnergyHP(pixelColor))
                {
                    pixelResults.Add(Color.Red);
                }
                if (ColorHelper.IsPixelColorEnergyBlack(pixelColor))
                {
                    pixelResults.Add(Color.Black);
                }
                if (ColorHelper.IsPixelColorEnergyWhite(pixelColor))
                {
                    pixelResults.Add(Color.White);

                }

                if (currentxPos -1 == 0)
                {
                    break;
                }
            }

            if (pixelResults.Count > energyBarEndBuffer)
                pixelResults.RemoveRange(pixelResults.Count - energyBarEndBuffer, energyBarEndBuffer);

            return Convert.ToInt32((100d / pixelResults.Count()) * pixelResults.Count(x => x == Color.Red)); ;


        }

        private async Task CheckDistance()
        {
            using (var frame = await Browser.TakeScreenshotAsync())
            {

                if (SelectedPlayer.IsAvailable)
                {

                    float browserMiddleX = (float)(Browser.ActualWidth / 2f);
                    float browserMiddleY = (float)(Browser.ActualHeight / 2f);

                    Rectangle topleft = Rectangle.Empty;
                    Rectangle bottomright = Rectangle.Empty;
                    Rectangle bottomleft = Rectangle.Empty;
                    Rectangle topright = Rectangle.Empty;


                    foreach (var part in GlobalValues.SelectedArrowBottomRightGroup)
                    {
                        bottomright = BitmapSearcher.SearchBitmap(frame, part, 0.1);
                        if (bottomright != Rectangle.Empty) break;
                    }

                    foreach (var part in GlobalValues.SelectedArrowBottomLeftGroup)
                    {
                        bottomleft = BitmapSearcher.SearchBitmap(frame, part, 0.1);
                        if (bottomleft != Rectangle.Empty) break;
                    }

                    foreach (var part in GlobalValues.SelectedArrowTopLeftGroup)
                    {
                        topleft = BitmapSearcher.SearchBitmap(frame, part, 0.1);
                        if (topleft != Rectangle.Empty) break;
                    }

                    foreach (var part in GlobalValues.SelectedArrowTopRightGroup)
                    {
                        topright = BitmapSearcher.SearchBitmap(frame, part, 0.1);
                        if (topright != Rectangle.Empty) break;
                    }


                    if (bottomright == Rectangle.Empty && bottomleft == Rectangle.Empty && topleft == Rectangle.Empty && topright == Rectangle.Empty)
                    {
                        CurrentDistanceType = DistanceType.NOTONSCREEN;
                        return;
                    }

                    var centerPoint = Utils.CalculateCenterSquare(bottomleft, bottomright, topright, topleft);

                    if (centerPoint != Vector2.Zero)
                    {
                        MainPosition = centerPoint;
                        CurrentDistanceBetweenMain = Vector2.Distance(centerPoint, new Vector2(browserMiddleX, browserMiddleY));

                        if (MinDistanceBetweenMain >= CurrentDistanceBetweenMain && UserInteractionBlockTimer.AddSeconds(2) < DateTime.Now)
                        {
                            MinDistanceBetweenMain = CurrentDistanceBetweenMain;
                        }

                        LastDistanceBetweenMain.Add(Math.Round((double)CurrentDistanceBetweenMain, 1));

                        if (LastDistanceBetweenMain.Count() >= 10)
                        {
                            if (LastDistanceBetweenMain.Distinct().Count() == 1)
                            {
                                MinDistanceBetweenMain = CurrentDistanceBetweenMain;
                            }

                            LastDistanceBetweenMain = new List<double>();
                        }
                        CurrentDistanceType = DistanceType.OK;
                    }
                    else
                    {
                        CurrentDistanceType = DistanceType.ONCALCULATION;
                        MinDistanceBetweenMain = float.MaxValue;
                        MainPosition = null;
                        CurrentDistanceBetweenMain = 0;
                    }

                }
                else
                {
                    CurrentDistanceType = DistanceType.NOTSELECTED;
                    MinDistanceBetweenMain = float.MaxValue;
                    MainPosition = null;
                    CurrentDistanceBetweenMain = 0;
                }
            }
        }

        #endregion

        #region BrowserInteraction

        public async Task SendActionButtonToBrowser(List<int> actionKeys)
        {
            actionKeys = actionKeys.Distinct().ToList();

            var queueItem = new SendBrowserKeysQueueItem()
            {
                Keys = actionKeys
            };

            lock (BrowserKeysQueueItems)
            {
                BrowserKeysQueueItems.Add(queueItem);
            }

            while (!queueItem.Processed)
            {
                await Task.Delay(10);
            }
        }

        public async Task TriggerFollowPlayer()
        {
            await SendActionButtonToBrowser(Player.TriggerFollowHotkeys);
        }

        public async Task ReBuffPlayer(bool calledManuell = false)
        {
            try
            {
                if (Player.CurrentState == FSStateType.NONE)
                {
                    Player.CurrentState = FSStateType.BUFFING;
                    var actions = ActionControllers.Where(x => x.Skill.Rebuffable).ToList();

                    foreach (var action in actions)
                    {
                        while (!IsNearPlayerAndCanBuff())
                        {
                            if (Player.SealfHealProtectionActive && Player.HP <= Player.SealfHealProtectionValue) break;
                            await Task.Delay(10);
                        }

                        if (action.PlayerValue.Value.Count > 0)
                        {
                            await SendActionButtonToBrowser(action.PlayerValue.Value);
                            await Task.Delay((int)(Player.WaitDurationBetweenRebuffableBuffs * 1000));
                            if (!calledManuell && UserInteractionBlockTimer > DateTime.Now) break;
                        }
                    }
                    await TriggerFollowPlayer();
                }
            }
            catch
            {

            }
            finally
            {
                Player.CurrentState = FSStateType.NONE;
            }
        }
       
        public async Task TriggerStatusUI()
        {
            await SendActionButtonToBrowser(Player.TriggerStatusHotkeys);
        }

        public async Task TriggerResurrection()
        {
            await SendActionButtonToBrowser(Player.ResurrectionHotkeys);
        }

        public async Task TriggerClearPlayer()
        {
            await SendActionButtonToBrowser(Player.TriggerClearTargetHotkeys);
        }

        public async Task UseHeal()
        {
            if ( 
                (DateTime.Now < UseHealBlockTimer || Player.CurrentState != FSStateType.NONE || (SelectedPlayer.IsAvailable && !IsNearPlayerAndCanBuff())) && !_cricitalHealingActive
                ) return;
            try
            {
                SkillActionController skillController = null;

                if (Player.HealVariant == HealVariant.HEAL)
                {
                    skillController = ActionControllers.First(x => x.SkillType == SkillType.HEAL);
                }
                else if (Player.HealVariant == HealVariant.HEALRAIN)
                {
                    skillController = ActionControllers.First(x => x.SkillType == SkillType.HEALRAIN);
                }

                await SendActionButtonToBrowser(skillController.PlayerValue.Value);
                
                if (skillController.Skill.CastingTimesInMilliseconds.ContainsKey(1)) // LEVEL 1 muss noch auf Level der SKills angepasst werden
                {
                    UseHealBlockTimer = DateTime.Now.AddMilliseconds(skillController.Skill.CastingTimesInMilliseconds[1]);
                }
                else
                {
                    UseHealBlockTimer = DateTime.Now.AddMilliseconds(skillController.Skill.DefaultCastingTimesInMilliseconds);
                }
            }
            catch
            {

            }
        }

       
        public async Task ReBuffGeburahTiphreth(bool calledManuell = false)
        {

            try
            {
                if (calledManuell ||
                       (Player.HP == 100 &&
                       SelectedPlayer.IsAvailable &&
                       SelectedPlayer.LastFullHPTimestamp.HasValue &&
                       SelectedPlayer.LastFullHPTimestamp.Value.AddSeconds(2) < DateTime.Now &&
                       Player.CurrentState == FSStateType.NONE &&
                        MainPosition != null))
                    {
                    Player.CurrentState = FSStateType.BUFFING;
                    while (!IsNearPlayerAndCanBuff())
                    {
                        await Task.Delay(10);
                    }

                    _rebuffGTSkipped = false;

                    var skillController = ActionControllers.First(x => x.SkillType == SkillType.GEBURAHTIPHRETH);

                    if (skillController.PlayerValue.Value.Count > 0)
                    {
                        await SendActionButtonToBrowser(skillController.PlayerValue.Value);
                        await Task.Delay((int)(Player.WaitDurationBetweenRebuffableBuffs * 1000));
                        if (Player.AutoRefollowActive)
                            await TriggerFollowPlayer();
                        ResetNextGTRebuffTime();
                    }
                }
                    else
                {
                    _rebuffGTSkipped = true;
                }
            }
            catch
            {

            }
            finally
            {
                Player.CurrentState = FSStateType.NONE;
            }

        }

        public async Task ReBuffHolyGuard(bool calledManuell = false)
        {

            try
            {
                if (Player.CurrentState == FSStateType.NONE)
                {
                    Player.CurrentState = FSStateType.BUFFING;
                    var skillController = ActionControllers.First(x => x.SkillType == SkillType.HOLYGUARD);

                    if (skillController.PlayerValue.Value.Count > 0)
                    {
                        while (!IsNearPlayerAndCanBuff())
                        {
                            await Task.Delay(10);
                        }

                        await SendActionButtonToBrowser(skillController.PlayerValue.Value);
                        await Task.Delay((int)(Player.WaitDurationBetweenRebuffableBuffs * 1000));
                        if (Player.AutoRefollowActive)
                            await TriggerFollowPlayer();
                    }
                }
            }
            catch
            {

            }
            finally
            {
                Player.CurrentState = FSStateType.NONE;
            }
        }


        private async void UserInteractWithBrowser()
        {
            if (Player.StopBotOnInteractionActive)
            {
                UserInteractionBlockTimer = DateTime.Now.AddSeconds(Player.StopBotOnInteractionTime);
            }

            MinDistanceBetweenMain = float.MaxValue;
        }

        private async void BrowserFrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            if (e.Frame.IsMain)
            {
                _isBrowserReady = true;
                double zoomLevel = Math.Log(1 / Utils.GetWindowsScale(), 1.2);
                Browser.SetZoomLevel(zoomLevel);
                this.Browser.FrameLoadStart -= BrowserFrameLoadStart;
                Browser.Reload();
            }
        }

        private async Task ExecuteCustomHotkey(CustomGlobalSkillsActionController controller)
        {

            try
            {
                if (Player.CurrentState == FSStateType.NONE)
                {
                    Window.AddSnackbar(String.Format(Properties.Resources.executehotkey, controller.Skill.TriggerKeys.ActionKeyIdsToActionkeys().ActionKeysToString()));
                    Player.CurrentState = FSStateType.BUFFING;
                    foreach (var actions in controller.Skill.Actions)
                    {
                        await SendActionButtonToBrowser(actions.Keys);
                        await Task.Delay((int)(actions.Duration * 1000));
                    }
                }
            }
            catch
            {

            }
            finally
            {
                Player.CurrentState = FSStateType.NONE;
            }

        }
#endregion

        #region Initialize

        private void IniGlobalSkillController()
        {
            GlobalActionControllers.Add(new GlobalSkillsActionController()
            {
                SkillType = _Script.Types.GlobalSkillTypes.REBUFF,
                View = new ActionView()
                {
                    Button = Window.GlobalSkillRebuffActionButton,
                },
                PlayerValue = new VarRef<List<int>>(() => Player.GlobalSkillRebuffHotkeys, val => { Player.GlobalSkillRebuffHotkeys = val; })
            });

            GlobalActionControllers.Add(new GlobalSkillsActionController()
            {
                SkillType = _Script.Types.GlobalSkillTypes.GEBURAHTIPHRETH,
                View = new ActionView()
                {
                    Button = Window.GlobalSkillGeburahTiphrethActionButton,
                    ActionButtonsDelegate = (ActionController controller) =>
                    {
                        if (Player.GeburahTiphrethKeys.Count == 0 && Player.GlobalSkillGeburahTiphrethHotkeys.Count > 0)
                        {
                            UpdatePlayerGlobalHotKeys(controller as GlobalSkillsActionController, new List<ActionKey>());
                            ShowMessage(Properties.Resources.info, Properties.Resources.mustsetgtbefore);
                        }
                    }
                },
                PlayerValue = new VarRef<List<int>>(() => Player.GlobalSkillGeburahTiphrethHotkeys, val => { Player.GlobalSkillGeburahTiphrethHotkeys = val; })
            });

            GlobalActionControllers.Add(new GlobalSkillsActionController()
            {
                SkillType = _Script.Types.GlobalSkillTypes.HOLYGUARD,
                View = new ActionView()
                {
                    Button = Window.GlobalSkillHolyGuardActionButton,
                    ActionButtonsDelegate = (ActionController controller) =>
                    {
                        if (Player.HolyGuardKeys.Count == 0 && Player.GlobalSkillHolyGuardHotkeys.Count > 0)
                        {
                            UpdatePlayerGlobalHotKeys(controller as GlobalSkillsActionController, new List<ActionKey>());
                            ShowMessage(Properties.Resources.info, Properties.Resources.mustsetholyguardbefore);
                        }
                    }
                },
                PlayerValue = new VarRef<List<int>>(() => Player.GlobalSkillHolyGuardHotkeys, val => { Player.GlobalSkillHolyGuardHotkeys = val; })
            });


        }
        private void IniCustomGlobalSkillController()
        {
            foreach (var skill in Player.CustomGlobalHotkeys)
            {
                CustomGlobalActionControllers.Add(new CustomGlobalSkillsActionController()
                {
                    Skill = skill
                });
            }
        }
        private void IniSkillControllers()
        {
            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.HEAL,
                Skill = new Heal(),
                View = new ActionView()
                {
                    Button = Window.HealActionButton,
                    ActionButtonsDelegate = (ActionController controller) =>
                    {
                        if (Player.HealKeys.Count == 0)
                        {
                            ChangeSelfHealActiveState(false);
                            ChangeAutoResurrectActiveState(false);
                        }
                    }
                },
                PlayerValue = new VarRef<List<int>>(() => Player.HealKeys, val => { Player.HealKeys = val; }) 
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.HEALRAIN,
                Skill = new HealRain(),
                View = new ActionView()
                {
                    Button = Window.HealRainActionButton,
                    ActionButtonsDelegate = (ActionController controller) =>
                    {
                        if (Player.HealKeys.Count == 0)
                        {
                            ChangeSelfHealActiveState(false);
                        }
                    }
                },
                PlayerValue = new VarRef<List<int>>(() => Player.HealKeys, val => { Player.HealKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.MENTALSIGN,
                Skill = new MentalSign(),
                View = new ActionView()
                {
                    Button = Window.MentalsignActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.MentalSignKeys, val => { Player.MentalSignKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.HASTE,
                Skill = new Haste(),
                View = new ActionView()
                {
                    Button = Window.HasteActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.HasteKeys, val => { Player.HasteKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.CATSREFLEX,
                Skill = new CatsReflex(),
                View = new ActionView()
                {
                    Button = Window.CatsReflexionActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.CatsReflexionKeys, val => { Player.CatsReflexionKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.CANNENBALL,
                Skill = new Cannenball(),
                View = new ActionView()
                {
                    Button = Window.CannenballActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.CannenbalKeys, val => { Player.CannenbalKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.HEAPUP,
                Skill = new HeapUp(),
                View = new ActionView()
                {
                    Button = Window.HeapActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.HeapUpKeys, val => { Player.HeapUpKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.BEEFUP,
                Skill = new BeefUp(),
                View = new ActionView()
                {
                    Button = Window.BeefActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.BeefUpKeys, val => { Player.BeefUpKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.ACCURACY,
                Skill = new Accuracy(),
                View = new ActionView()
                {
                    Button = Window.AccuracyActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.AccuracyKeys, val => { Player.AccuracyKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.QUICKSTEP,
                Skill = new QuickStep(),
                View = new ActionView()
                {
                    Button = Window.QuickstepActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.QuickStepKeys, val => { Player.QuickStepKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.PATIENCE,
                Skill = new Patience(),
                View = new ActionView()
                {
                    Button = Window.PatienceActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.PatienceKeys, val => { Player.PatienceKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.PROTECT,
                Skill = new Protect(),
                View = new ActionView()
                {
                    Button = Window.ProtectActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.ProtectKeys, val => { Player.ProtectKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.HOLYGUARD,
                Skill = new HolyGuard(),
                View = new ActionView()
                {
                    Button = Window.HolyguardActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.HolyGuardKeys, val => { Player.HolyGuardKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.SPIRITUREFORTUNE,
                Skill = new SpiritureFortune(),
                View = new ActionView()
                {
                    Button = Window.SpiritureFortuneActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.SpiritureFortuneKeys, val => { Player.SpiritureFortuneKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.GEBURAHTIPHRETH,
                Skill = new GeburahTiphreth(),
                View = new ActionView()
                {
                    Button = Window.GeburahTiphrethActionButton,
                },
                PlayerValue = new VarRef<List<int>>(() => Player.GeburahTiphrethKeys, val => { Player.GeburahTiphrethKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.HPPOTION,
                Skill = new HPPotion(),
                View = new ActionView()
                {
                    Button = Window.HpPotActionButton,
                    ActionButtonsDelegate = (ActionController controller) =>
                    {
                        if (Player.HpPotionsKeys.Count == 0)
                        {
                            ChangeSelfHealActiveState(false);
                        }
                    }
                },
                PlayerValue = new VarRef<List<int>>(() => Player.HpPotionsKeys, val => { Player.HpPotionsKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.MPOTION,
                Skill = new MPPotion(),
                View = new ActionView()
                {
                    Button = Window.MpPotActionButton
                },
                PlayerValue = new VarRef<List<int>>(() => Player.MpPotionsKeys, val => { Player.MpPotionsKeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.TRIGGERSTATUS,
                Skill = new TriggerStatus(),
                View = new ActionView()
                {
                    Button = Window.TriggerStatusActionButton,
                },
                PlayerValue = new VarRef<List<int>>(() => Player.TriggerStatusHotkeys, val => { Player.TriggerStatusHotkeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.TRIGGERFOLLOW,
                Skill = new TriggerFollow(),
                View = new ActionView()
                {
                    Button = Window.TriggerFollowActionButton,
                },
                PlayerValue = new VarRef<List<int>>(() => Player.TriggerFollowHotkeys, val => { Player.TriggerFollowHotkeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.TRIGGERCLEARTARGET,
                Skill = new TriggerClearTarget(),
                View = new ActionView()
                {
                    Button = Window.TriggerClearTargetActionButton,
                },
                PlayerValue = new VarRef<List<int>>(() => Player.TriggerClearTargetHotkeys, val => { Player.TriggerClearTargetHotkeys = val; })
            });

            ActionControllers.Add(new SkillActionController()
            {
                SkillType = _Script.Types.SkillType.RESURRECTION,
                Skill = new Resurrection(),
                View = new ActionView()
                {
                    Button = Window.ResurrectionActionButton,
                    ActionButtonsDelegate = (ActionController controller) =>
                    {
                        if (Player.ResurrectionHotkeys.Count == 0)
                        {
                            ChangeAutoResurrectActiveState(false);
                        }
                    }
                },
                PlayerValue = new VarRef<List<int>>(() => Player.ResurrectionHotkeys, val => { Player.ResurrectionHotkeys = val; })
            });

        }

        #endregion

        #region Captcha

        public async void PlayCaptchaDetectedSound()
        {
            if (Player.CaptchaDetectionActive)
            {
                var audio = GlobalValues.CaptchaDetectedSounds.FirstOrDefault(x => x.ID == Player.CaptchaDetectionSoundValue);

                if (audio != null)
                {
                    if (audio.Stream.CanSeek) audio.Stream.Seek(0, System.IO.SeekOrigin.Begin);
                    using (System.Media.SoundPlayer player = new System.Media.SoundPlayer(audio.Stream))
                    {
                        player.Play();
                    };
                }
               
            }
        }

        #endregion

        #region DEV

        public async void SaveScreenshot()
        {
            var b = await Browser.TakeScreenshotAsync();
            b.Save("C:\\Temp\\AutoFS\\Screenshot.png");
        }

        #endregion

        #region Global Hotkeys

       
        private bool CanExecuteGlobalHotkey()
        {
            return true;
        }

        public void UnRegisterGlobalHotkeys()
        {
            _globalHotkeysManager.UnregisterAll();
        }

        public void RegisterGlobalHotkeys()
        {
            if (_isRunning)
            {
                UnRegisterGlobalHotkeys();

                RegisterGlobalHotkeys(
                    Player.GlobalSkillRebuffHotkeys.ActionKeyIdsToActionkeys(),
                    () =>
                    {
                        if (CanExecuteGlobalHotkey())
                        {
                           
                            _ = ReBuffPlayer();
                        }
                    }
                );

                RegisterGlobalHotkeys(
                    Player.GlobalSkillGeburahTiphrethHotkeys.ActionKeyIdsToActionkeys(),
                    () =>
                    {
                        if (CanExecuteGlobalHotkey())
                        {
                            _ = ReBuffGeburahTiphreth(true);
                        }
                    }
                );

                RegisterGlobalHotkeys(
                    Player.GlobalSkillHolyGuardHotkeys.ActionKeyIdsToActionkeys(),
                    () =>
                    {
                        if (CanExecuteGlobalHotkey())
                        {
                            _ = ReBuffHolyGuard();
                        }
                    }
                );


                foreach (var custom in CustomGlobalActionControllers)
                {
                    RegisterGlobalHotkeys(custom.Skill.TriggerKeys.ActionKeyIdsToActionkeys(), () =>
                    {
                        if (CanExecuteGlobalHotkey())
                        {
                            
                            _ = ExecuteCustomHotkey(custom);
                        }
                    });
                }
            }
        }
        
        private void RegisterGlobalHotkeys(List<ActionKey> actionKeys, Action action)
        {
            if (actionKeys.Count > 0)
            {
                var modifiers = actionKeys.Where(x => x.GlobalHotKeyModifier != NonInvasiveKeyboardHookLibrary.ModifierKeys.WindowsKey).Select(x => x.GlobalHotKeyModifier).ToArray();
                var key = actionKeys.FirstOrDefault(x => x.GlobalHotKeyModifier == NonInvasiveKeyboardHookLibrary.ModifierKeys.WindowsKey);

                _globalHotkeysManager.RegisterHotkey(modifiers, KeyInterop.VirtualKeyFromKey(key.KeybordKeys.First()), action, false, key.KeybordKeys.Select(x => KeyInterop.VirtualKeyFromKey(x)).ToList());
            }
        }


#endregion

        public void Dispose()
        {
            foreach (var job in CronJobs)
            {
                job.Dispose();
            }
        }
    }
}
