using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicStart",
                 "MusicStart")]
    public class MusicStart : Command
    {
        [SerializeField]
        private string MusicName;

        [SerializeField]
        private float Duration = 1.5f;

        [Range(0, 1)]
        [SerializeField]
        private float Volume = 1;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.MusicStart(MusicName, Duration, Volume);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
