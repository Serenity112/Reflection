using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicStart",
                 "MusicStart")]
    public class MusicStart : Command
    {
        [SerializeField]
        private string ost;

        [SerializeField]
        private float duration = 3f;

        [SerializeField]
        private float targetVol = 1;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.MusicStart(ost, duration, targetVol);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
