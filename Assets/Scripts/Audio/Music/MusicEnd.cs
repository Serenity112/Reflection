using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicEnd",
                 "MusicEnd")]
    public class MusicEnd : Command
    {
        [SerializeField]
        private string MusicName;

        [SerializeField]
        private float Duration = 1.5f;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.MusicEnd(Duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
