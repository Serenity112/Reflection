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
        private float FadeInTime = 2f;

        [SerializeField]
        private float DelayTime = 1f;

        [SerializeField]
        private float Volume = 1f;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineChange(AudioManager.AudioLine.Ambient, AmbientOld, AmbientNew, FadeInTime, DelayTime, Volume));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }
    }
}
