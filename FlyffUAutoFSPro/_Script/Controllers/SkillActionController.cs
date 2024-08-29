using FlyffUAutoFSPro._Script.Models.Actions;
using FlyffUAutoFSPro._Script.Types;
using FlyffUAutoFSPro._Script.UI;
using System.Collections.Generic;

namespace FlyffUAutoFSPro._Script.Controllers
{
    public class SkillActionController : ActionController
    {
        public SkillType SkillType { get; set; }
        public Action Skill { get; set; }
    }
}
