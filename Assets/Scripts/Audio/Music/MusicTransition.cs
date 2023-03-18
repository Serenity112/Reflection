using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicTransition",
                 "MusicTransition")]
    public class MusicTransition : Command
    {
        [SerializeField]
        private string ost;

        [SerializeField]
        private float duration;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.MusicTransition(ost, duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
