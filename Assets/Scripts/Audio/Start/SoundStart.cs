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

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.SoundStart(ost);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(96, 63, 97, 255);
        }
    }
}
