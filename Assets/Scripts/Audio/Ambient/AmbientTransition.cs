using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientTransition",
                 "AmbientTransition")]
    public class AmbientTransition : Command
    {
        [SerializeField]
        private string AmbientOld;

        [SerializeField]
        private string AmbientNew;

        [SerializeField]
        private float Volume = 1f;

        [SerializeField]
        private float Time = 2f;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineTransition(AudioManager.AudioLine.Ambient, AmbientOld, AmbientNew, Time, Volume));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }
    }
}
