using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicStart",
                 "MusicStart")]
    public class MusicStart : Command
    {
        [SerializeField]
        private string MusicName;

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
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineStart(AudioManager.AudioLine.Music, MusicName, Time, Volume));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
