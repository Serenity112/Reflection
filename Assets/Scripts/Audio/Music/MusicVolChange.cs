using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicVolChange",
                 "MusicVolChange")]
    public class MusicVolChange : Command
    {
        [SerializeField]
        private float Duration = 1.5f;

        [Range(0, 1)]
        [SerializeField]
        private float Volume;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.MusicVolChange(Duration, Volume);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
