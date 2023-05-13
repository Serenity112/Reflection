using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicTransition",
                 "MusicTransition")]
    public class MusicTransition : Command
    {
        [SerializeField]
        private string MusicName;

        [SerializeField]
        private float Duration = 1.5f;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.MusicTransition(MusicName, Duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
