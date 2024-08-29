namespace FlyffUAutoFSPro._Script.Models.Actions
{
    public class Resurrection : Action
    {
        public override bool Rebuffable { get; set; } = false;
        public override int DefaultCastingTimesInMilliseconds { get; set; } = 500;
    }
}
