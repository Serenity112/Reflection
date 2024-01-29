using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientVolChange",
                 "AmbientVolChange")]
    public class AmbientVolChange : Command
    {
        [SerializeField]
        private string AmbientName;

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
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineVolChange(AudioManager.AudioLine.Ambient, AmbientName, Time, Volume));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
