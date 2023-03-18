using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicVolDefault",
                 "MusicVolDefault")]
    public class MusicVolDefault : Command
    {
        [SerializeField]
        private float duration;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.MusicVolDefault(duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
