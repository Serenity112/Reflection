using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicVolChange",
                 "MusicVolChange")]
    public class MusicVolChange : Command
    {
        [SerializeField]
        private float duration;

        [SerializeField]
        private float factor;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.MusicVolChange(duration, factor);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
