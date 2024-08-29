using FlyffUAutoFSPro._Script.Controllers;
using FlyffUAutoFSPro._Script.Models;
using FlyffUAutoFSPro._Script.Types;
using FlyffUAutoFSPro.AppSettings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace FlyffUAutoFSPro._Script.Bot.FS
{
    public class FSPlayer : Player
    {
        [JsonIgnore]
        private FSStateType _currentState;
        [JsonIgnore]
        public FSStateType CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
            }
        }


        #region Mein Status

        private List<int> _hpPotionsKeys = new List<int>();
        public List<int> HpPotionsKeys 
        { 
            get
            {
                return _hpPotionsKeys;
            }
            set
            {
                _hpPotionsKeys = value;
            }
        }

        private int _hpPotionsValue = 80;
        public int HpPotionsValue
        {
            get
            {
                return _hpPotionsValue;
            }
            set
            {
                _hpPotionsValue = value;
            }
        }

        private List<int> _mpPotionsKeys = new List<int>();
        public List<int> MpPotionsKeys
        {
            get
            {
                return _mpPotionsKeys;
            }
            set
            {
                _mpPotionsKeys = value;
            }
        }

        private int _mpPotionsValue = 80;
        public int MpPotionsValue
        {
            get
            {
                return _mpPotionsValue;
            }
            set
            {
                _mpPotionsValue = value;
            }
        }


        private bool _sealfHealProtectionActive;
        public bool SealfHealProtectionActive
        {
            get
            {
                return _sealfHealProtectionActive;
            }
            set
            {
                _sealfHealProtectionActive = value;
            }
        }

        private int _sealfHealProtectionValue = 10;
        public int SealfHealProtectionValue
        {
            get
            {
                return _sealfHealProtectionValue;
            }
            set
            {
                _sealfHealProtectionValue = value;
            }
        }

        #endregion

        #region Selected Player

        private int _normalHealingValue = 90;
        public int NormalHealingValue
        {
            get
            {
                return _normalHealingValue;
            }
            set
            {
                _normalHealingValue = value;
            }
        }

        private int _criticalHealingValue = 50;
        public int CriticalHealingValue
        {
            get
            {
                return _criticalHealingValue;
            }
            set
            {
                _criticalHealingValue = value;
            }
        }

        private int _criticalHealingDuration = 2;
        public int CriticalHealingDuration
        {
            get
            {
                return _criticalHealingDuration;
            }
            set
            {
                _criticalHealingDuration = value;
            }
        }

        #endregion

        #region Emoji

        private bool _emojiAutoRebuffDetectionActive;
        public bool EmojiAutoRebuffDetectionActive
        {
            get
            {
                return _emojiAutoRebuffDetectionActive;
            }
            set
            {
                _emojiAutoRebuffDetectionActive = value;
            }
        }

        private int _emojiAutoRebuffEmojiId = 0;
        public int EmojiAutoRebuffEmojiId
        {
            get
            {
                return _emojiAutoRebuffEmojiId;
            }
            set
            {
                _emojiAutoRebuffEmojiId = value;
            }
        }

        #endregion

        #region Skills

        private HealVariant _healVariant = HealVariant.HEAL;
        public HealVariant HealVariant
        {
            get
            {
                return _healVariant;
            }
            set
            {
                _healVariant = value;
            }
        }

        private List<int> _healKeys = new List<int>();
        public List<int> HealKeys
        {
            get
            {
                return _healKeys;
            }
            set
            {
                _healKeys = value;
            }
        }

        private List<int> _patienceKeys = new List<int>();
        public List<int> PatienceKeys
        {
            get
            {
                return _patienceKeys;
            }
            set
            {
                _patienceKeys = value;
            }
        }

        private List<int> _quickStepKeys = new List<int>();
        public List<int> QuickStepKeys
        {
            get
            {
                return _quickStepKeys;
            }
            set
            {
                _quickStepKeys = value;
            }
        }

        private List<int> _mentalSignKeys = new List<int>();
        public List<int> MentalSignKeys
        {
            get
            {
                return _mentalSignKeys;
            }
            set
            {
                _mentalSignKeys = value;
            }
        }

        private List<int> _hasteKeys = new List<int>();
        public List<int> HasteKeys
        {
            get
            {
                return _hasteKeys;
            }
            set
            {
                _hasteKeys = value;
            }
        }

        private List<int> _heapUpKeys = new List<int>();
        public List<int> HeapUpKeys
        {
            get
            {
                return _heapUpKeys;
            }
            set
            {
                _heapUpKeys = value;
            }
        }

        private List<int> _catsReflexionKeys = new List<int>();
        public List<int> CatsReflexionKeys
        {
            get
            {
                return _catsReflexionKeys;
            }
            set
            {
                _catsReflexionKeys = value;
            }
        }

        private List<int> _beefUpKeys = new List<int>();
        public List<int> BeefUpKeys
        {
            get
            {
                return _beefUpKeys;
            }
            set
            {
                _beefUpKeys = value;
            }
        }

        private List<int> _cannenBallKeys = new List<int>();
        public List<int> CannenbalKeys
        {
            get
            {
                return _cannenBallKeys;
            }
            set
            {
                _cannenBallKeys = value;
            }
        }

        private List<int> _accuracyKeys = new List<int>();
        public List<int> AccuracyKeys
        {
            get
            {
                return _accuracyKeys;
            }
            set
            {
                _accuracyKeys = value;
            }
        }

        private List<int> _protectKeys = new List<int>();
        public List<int> ProtectKeys
        {
            get
            {
                return _protectKeys;
            }
            set
            {
                _protectKeys = value;
            }
        }

        private List<int> _holyGuardKeys = new List<int>();
        public List<int> HolyGuardKeys
        {
            get
            {
                return _holyGuardKeys;
            }
            set
            {
                _holyGuardKeys = value;
            }
        }

        private List<int> _spiritureFortuneKeys = new List<int>();
        public List<int> SpiritureFortuneKeys
        {
            get
            {
                return _spiritureFortuneKeys;
            }
            set
            {
                _spiritureFortuneKeys = value;
            }
        }

        private List<int> _geburahTiphrethKeys = new List<int>();
        public List<int> GeburahTiphrethKeys
        {
            get
            {
                return _geburahTiphrethKeys;
            }
            set
            {
                _geburahTiphrethKeys = value;
            }
        }

        private bool _geburahTiphrethAutoRebuffActive;
        public bool GeburahTiphrethAutoRebuffActive
        {
            get
            {
                return _geburahTiphrethAutoRebuffActive;
            }
            set
            {
                _geburahTiphrethAutoRebuffActive = value;
            }
        }

        private int _geburahTiphrethAutoRebuffTime = 30;
        public int GeburahTiphrethAutoRebuffTime
        {
            get
            {
                return _geburahTiphrethAutoRebuffTime;
            }
            set
            {
                _geburahTiphrethAutoRebuffTime = value;
            }
        }


        private List<int> _resurrectionHotkeys = new List<int>();
        public List<int> ResurrectionHotkeys
        {
            get
            {
                return _resurrectionHotkeys;
            }
            set
            {
                _resurrectionHotkeys = value;
            }
        }

        #endregion

        #region Global Skills

        private List<int> _globalSkillRebuffHotkeys = new List<int>();
        public List<int> GlobalSkillRebuffHotkeys
        {
            get
            {
                return _globalSkillRebuffHotkeys;
            }
            set
            {
                _globalSkillRebuffHotkeys = value;
            }
        }

        private List<int> _globalSkillGeburahTiphrethHotkeys = new List<int>();
        public List<int> GlobalSkillGeburahTiphrethHotkeys
        {
            get
            {
                return _globalSkillGeburahTiphrethHotkeys;
            }
            set
            {
                _globalSkillGeburahTiphrethHotkeys = value;
            }
        }

        private List<int> _globalSkillHolyGuardHotkeys = new List<int>();
        public List<int> GlobalSkillHolyGuardHotkeys
        {
            get
            {
                return _globalSkillHolyGuardHotkeys;
            }
            set
            {
                _globalSkillHolyGuardHotkeys = value;
            }
        }


        private List<CustomGlobalHotkey> _customGlobalHotkeys = new List<CustomGlobalHotkey>();
        public List<CustomGlobalHotkey> CustomGlobalHotkeys
        {
            get
            {
                return _customGlobalHotkeys;
            }
            set
            {
                _customGlobalHotkeys = value;
            }
        }



        #endregion

        #region Hotkeys

        private List<int> _triggerStatusHotkeys = new List<int>() { (int) ActionKeyType.T };
        public List<int> TriggerStatusHotkeys
        {
            get
            {
                return _triggerStatusHotkeys;
            }
            set
            {
                _triggerStatusHotkeys = value;
            }
        }

        private List<int> _triggerFollowHotkeys = new List<int>() { (int) ActionKeyType.Z };
        public List<int> TriggerFollowHotkeys
        {
            get
            {
                return _triggerFollowHotkeys;
            }
            set
            {
                _triggerFollowHotkeys = value;
            }
        }

        private List<int> _triggerDeselectHotkeys = new List<int>() { (int) ActionKeyType.Escape  };
        public List<int> TriggerClearTargetHotkeys
        {
            get
            {
                return _triggerDeselectHotkeys;
            }
            set
            {
                _triggerDeselectHotkeys = value;
            }
        }


       

        

        private bool _autoRefollowActive;
        public bool AutoRefollowActive
        {
            get
            {
                return _autoRefollowActive;
            }
            set
            {
                _autoRefollowActive = value;
            }
        }

        private int _autoRefollowTime = 1;
        public int AutoRefollowTime
        {
            get
            {
                return _autoRefollowTime;
            }
            set
            {
                _autoRefollowTime = value;
            }
        }

        private bool _stopBotOnInteractionActive;
        public bool StopBotOnInteractionActive
        {
            get
            {
                return _stopBotOnInteractionActive;
            }
            set
            {
                _stopBotOnInteractionActive = value;
            }
        }

        private int _stopBotOnInteractionTime = 1;
        public int StopBotOnInteractionTime
        {
            get
            {
                return _stopBotOnInteractionTime;
            }
            set
            {
                _stopBotOnInteractionTime = value;
            }
        }

        #endregion

        #region Other

        private bool _captchaDetectionActive = true;
        public bool CaptchaDetectionActive
        {
            get
            {
                return _captchaDetectionActive;
            }
            set
            {
                _captchaDetectionActive = value;
            }
        }

        private int _captchaDetectionSoundValue = 0;
        public int CaptchaDetectionSoundValue
        {
            get
            {
                return _captchaDetectionSoundValue;
            }
            set
            {
                _captchaDetectionSoundValue = value;
            }
        }


        private bool _checkMainPlayerDistanceActive = true;
        public bool CheckMainPlayerDistanceActive
        {
            get
            {
                return _checkMainPlayerDistanceActive;
            }
            set
            {
                _checkMainPlayerDistanceActive = value;
            }
        }

        private bool _autoRessActive = true;
        public bool AutoResurrectActive
        {
            get
            {
                return _autoRessActive;
            }
            set
            {
                _autoRessActive = value;
            }
        }

        private bool _rebuffAfterAutoRessActive = true;
        public bool RebuffAfterResurrectActive
        {
            get
            {
                return _rebuffAfterAutoRessActive;
            }
            set
            {
                _rebuffAfterAutoRessActive = value;
            }
        }

        #endregion


        private float _waitDurationBetweenRebuffableBuffs = 2f;
        public float WaitDurationBetweenRebuffableBuffs
        {
            get
            {
                return _waitDurationBetweenRebuffableBuffs;
            }
            set
            {
                _waitDurationBetweenRebuffableBuffs = value;
            }
        }


        public void Save()
        {
            Settings.Default.SettingsFS = JsonConvert.SerializeObject(this);
            Settings.Default.Version = GlobalValues.Version;
            Settings.Default.Save();
        }

        public static FSPlayer Load()
        {
            try
            {
                if (String.IsNullOrEmpty(Settings.Default.SettingsFS)) return new FSPlayer();

                    return JsonConvert.DeserializeObject<FSPlayer>(Settings.Default.SettingsFS);
            }
            catch
            {
                return new FSPlayer();
            }
        }
    }
}
