using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientChange",
                 "AmbientChange")]
    public class AmbientChange : Command
    {
        [SerializeField]
        private string ost;

        [SerializeField]
        private float duration = 3f;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.AmbientChange(ost, duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }
    }
}
