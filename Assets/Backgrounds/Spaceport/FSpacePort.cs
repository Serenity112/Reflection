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
            UserData.instance.CurrentCommandIndex += 1;

            ((SpacePort)SpecialEventManager.instance.currentEvent).RocketLaunch();

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(192, 192, 192, 255);
        }
    }
}
