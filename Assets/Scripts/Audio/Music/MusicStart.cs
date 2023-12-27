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
        private float Time = 2f;

        [SerializeField]
        private float Volume = 1;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

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
