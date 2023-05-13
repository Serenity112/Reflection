using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientStart",
                 "AmbientStart")]
    public class AmbientStart : Command
    {
        [SerializeField]
        private string AmbientName;

        [SerializeField]
        private float Duration = 1.5f;

        [Range(0, 1)]
        [SerializeField]
        private float Volume = 1;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.AmbientStart(AmbientName, Duration, Volume);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }
    }
}
