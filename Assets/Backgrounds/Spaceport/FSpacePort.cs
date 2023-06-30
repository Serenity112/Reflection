using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                   "FSpacePort",
                   "FSpacePort")]
    public class FSpacePort : Command
    {
        [SerializeField]
        private SpacePortState State;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            switch (State)
            {
                case SpacePortState.Launch:
                    ((SpacePort)SpecialEventManager.instance.currentEvent).RocketLaunch();
                    break;
            }

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(192, 192, 192, 255);
        }
    }
}
