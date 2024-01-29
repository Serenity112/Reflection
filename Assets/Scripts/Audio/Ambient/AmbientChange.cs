using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientChange",
                 "AmbientChange")]
    public class AmbientChange : Command
    {
        [SerializeField]
        private string AmbientOld;

        [SerializeField]
        private string AmbientNew;

        [SerializeField]
        private float Volume = 1f;

        [SerializeField]
        private float Time = 2f;

        [SerializeField]
        private float Delay = 1f;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineChange(AudioManager.AudioLine.Ambient, AmbientOld, AmbientNew, Time, Delay, Volume));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }
    }
}
