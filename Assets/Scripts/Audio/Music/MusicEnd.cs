using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicEnd",
                 "MusicEnd")]
    public class MusicEnd : Command
    {
        [SerializeField]
        private float duration;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.MusicEnd(duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
