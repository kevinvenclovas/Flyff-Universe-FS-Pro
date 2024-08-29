using FlyffUAutoFSPro._Script.UI;
using System.Collections.Generic;

namespace FlyffUAutoFSPro._Script.Controllers
{
    public abstract class ActionController
    {
        public ActionView View { get; set; }
        public VarRef<List<int>> PlayerValue;
    }
}
