using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientVolDefault",
                 "AmbientVolDefault")]
    public class AmbientVolDefault : Command
    {
        [SerializeField]
        private float duration;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.AmbientVolDefault(duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
