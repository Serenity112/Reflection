using UnityEngine;
using static SergeyRoom;

namespace Fungus
{
    [CommandInfo("Ref",
                   "FTrainWindow",
                   "FTrainWindow")]
    public class FTrainWindow : Command
    {
        [SerializeField]
        private TrainNum Train = TrainNum.Far;

        [SerializeField]
        private float Delay = 20f;

        public override void OnEnter()
        {
            

            StartCoroutine(((SergeyRoom)SpecialEventManager.instance.CurrentEventObject).LaunchTrain(Train, Delay));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(192, 192, 192, 255);
        }
    }
}
