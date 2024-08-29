using System.Collections.Generic;

namespace FlyffUAutoFSPro._Script.Models.Actions
{
    public abstract class Action
    {
        public virtual bool Rebuffable { get; set; } = true;
        public virtual int DefaultCastingTimesInMilliseconds { get; set; } = 1000;
        public virtual Dictionary<int, int> CastingTimesInMilliseconds { get; set; } = new Dictionary<int, int>();
    }
}
