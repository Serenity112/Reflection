using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                   "FSpacePort",
                   "FSpacePort")]
    public class FSpacePort : Command
    {
        public override void OnEnter()
        {
            

            ((SpacePort)SpecialEventManager.instance.CurrentEventObject).RocketLaunch();

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(192, 192, 192, 255);
        }
    }
}
