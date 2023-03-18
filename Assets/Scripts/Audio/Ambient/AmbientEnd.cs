using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientEnd",
                 "AmbientEnd")]
    public class AmbientEnd : Command
    {
        [SerializeField]
        private float duration;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.AmbientEnd(duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }
    }
}
