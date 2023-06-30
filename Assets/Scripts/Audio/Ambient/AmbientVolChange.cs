using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientVolChange",
                 "AmbientVolChange")]
    public class AmbientVolChange : Command
    {
        // ”казываетс€ если играет больше чем 1 эмбиент и надо уточнить какой
        [SerializeField]
        private string AmbientName;

        [SerializeField]
        private float Duration = 1.5f;

        [SerializeField]
        private float Volume = 1;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            AudioManager.instance.AmbientVolChange(AmbientName, Duration, Volume);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
