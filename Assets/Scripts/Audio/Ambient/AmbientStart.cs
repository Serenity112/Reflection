using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientStart",
                 "AmbientStart")]
    public class AmbientStart : Command
    {
        [SerializeField]
        private string ost;

        [SerializeField]
        private float duration;

        [SerializeField]
        private float targetVol = 1;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.AmbientStart(ost, duration, targetVol);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }
    }
}
