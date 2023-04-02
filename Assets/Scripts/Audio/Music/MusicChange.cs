using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicChange",
                 "MusicChange")]
    public class MusicChange : Command
    {
        [SerializeField]
        private string ost;

        [SerializeField]
        private float duration = 3f;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.MusicChange(ost, duration);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
