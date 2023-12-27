using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicChange",
                 "MusicChange")]
    public class MusicChange : Command
    {
        [SerializeField]
        private string MusicOld;

        [SerializeField]
        private string MusicNew;

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
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineChange(AudioManager.AudioLine.Music, MusicOld, MusicNew, FadeInTime, DelayTime, Volume));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
