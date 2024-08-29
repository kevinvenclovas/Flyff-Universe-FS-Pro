using Newtonsoft.Json;
using System;

namespace FlyffUAutoFSPro._Script
{
    public class Player
    {
        private int _hp;
        [JsonIgnore]
        public int HP
        {
            get
            {
                return _hp >= GlobalValues.MaxPercentValue ? 100 : _hp;
            }
            set
            {
                _hp = value;
            }
        }
        private int _mp;
        [JsonIgnore]
        public int MP
        {
            get
            {
                return _mp >= GlobalValues.MaxPercentValue ? 100 : _mp;
            }
            set
            {
                _mp = value;
            }
        }
        private int _fp;
        [JsonIgnore]
        public int FP
        {
            get
            {
                return _fp >= GlobalValues.MaxPercentValue ? 100 : _fp;
            }
            set
            {
                _fp = value;
            }
        }



        private DateTime? _playerDeadTime;
        [JsonIgnore]
        public DateTime? PlayerDeadTime
        {
            get
            {
                return _playerDeadTime;
            }
            set
            {
                _playerDeadTime = value;
            }
        }

        private bool _isDead;
        [JsonIgnore]
        public bool IsDead
        {
            get
            {
                return _isDead;
            }
            set
            {
                _isDead = value;

                if (value)
                {
                    _playerDeadTime = DateTime.Now;
                }
                else
                {
                    _playerDeadTime = null;
                }
            }
        }

        public DateTime? LastFullHPTimestamp { get; set; }
    }
}
