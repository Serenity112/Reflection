using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicTransition",
                 "MusicTransition")]
    public class MusicTransition : Command
    {
        [SerializeField]
        private string MusicOld;

        [SerializeField]
        private string MusicNew;

        [SerializeField]
        private float Time = 2f;

        [SerializeField]
        private float Volume = 1f;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineTransition(AudioManager.AudioLine.Music, MusicOld, MusicNew, Time, Volume));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
