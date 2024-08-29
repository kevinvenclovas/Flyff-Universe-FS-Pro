using FlyffUAutoFSPro._Script.Controllers;
using System.Windows.Controls;

namespace FlyffUAutoFSPro._Script.UI
{
    public class ActionView
    {
        public delegate void ActionButtonsChangeDelegate(ActionController controller);
        public Button Button { get; set; }
        public ActionButtonsChangeDelegate ActionButtonsDelegate { get; set; }
    }
}
