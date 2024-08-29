using Newtonsoft.Json;

namespace FlyffUAutoFSPro._Script.Bot.Main
{
    public class MainPlayer : Player
    {
        private bool _isAvailable;
        [JsonIgnore]
        public bool IsAvailable
        {
            get
            {
                return _isAvailable && !IsDead;
            }
            set
            {
                _isAvailable = value;
            }
        }

    }
}
