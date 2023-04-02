using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "SoundStart",
                 "SoundStart")]
    public class SoundStart : Command
    {
        [SerializeField]
        private string ost;

        [SerializeField]
        private float duration = 3f;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.SoundStart(ost, duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(96, 63, 97, 255);
        }
    }
}
