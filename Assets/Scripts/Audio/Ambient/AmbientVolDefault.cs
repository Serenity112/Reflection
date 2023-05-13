using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientVolDefault",
                 "AmbientVolDefault")]
    public class AmbientVolDefault : Command
    {
        [SerializeField]
        private float Duration = 1.5f;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.AmbientVolDefault(Duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
