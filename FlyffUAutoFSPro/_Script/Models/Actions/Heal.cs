using System.Collections.Generic;

namespace FlyffUAutoFSPro._Script.Models.Actions
{
    public class Heal : Action
    {
        public override bool Rebuffable { get; set; } = false;
        public override int DefaultCastingTimesInMilliseconds { get; set; } = 600;
        public override Dictionary<int, int> CastingTimesInMilliseconds { get; set; } =
            new Dictionary<int, int>()
            {
                { 1,600 },
                { 2,575 },
                { 3,550 },
                { 4,525 },
                { 5,500 },
                { 7,450 },
                { 8,425 },
                { 9,400 },
                { 10,375 },
                { 11,350 },
                { 12,325 },
                { 13,300 },
                { 14,275 },
                { 15,250 },
                { 16,250 },
                { 17,250 },
                { 18,250 },
                { 19,250 },
                { 20,150 },
            };

    }
}
