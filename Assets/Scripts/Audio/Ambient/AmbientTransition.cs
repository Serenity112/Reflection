using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientTransition",
                 "AmbientTransition")]
    public class AmbientTransition : Command
    {
        [SerializeField]
        private string AmbientName;

        [SerializeField]
        private float Duration = 1.5f;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.AmbientTransition(AmbientName, Duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }
    }
}