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
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineChange(AudioManager.AudioLine.Music, MusicOld, MusicNew, Time, Delay, Volume));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
