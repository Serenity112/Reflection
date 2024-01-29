using System.Collections;
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
        private float Volume = 1;

        [SerializeField]
        private float Time = 2f;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineStart(AudioManager.AudioLine.Ambient, AmbientName, Time, Volume));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }
    }
}
