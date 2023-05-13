using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientVolChange",
                 "AmbientVolChange")]
    public class AmbientVolChange : Command
    {
        [SerializeField]
        private float Duration = 1.5f;

        [SerializeField]
        private float Volume;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.AmbientVolChange(Duration, Volume);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
