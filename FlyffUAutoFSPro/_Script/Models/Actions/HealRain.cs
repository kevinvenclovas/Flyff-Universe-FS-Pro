using System.Collections.Generic;

namespace FlyffUAutoFSPro._Script.Models.Actions
{
    public class HealRain : Action
    {
        public override bool Rebuffable { get; set; } = false;
        public override int DefaultCastingTimesInMilliseconds { get; set; } = 500;
    }
}
