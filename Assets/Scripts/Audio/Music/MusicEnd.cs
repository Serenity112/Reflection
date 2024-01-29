using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "MusicEnd",
                 "MusicEnd")]
    public class MusicEnd : Command
    {
        [SerializeField]
        private string MusicName;

        [SerializeField]
        private float Time = 2f;

        public override void OnEnter()
        {
            

            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineEnd(AudioManager.AudioLine.Music, MusicName, Time));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
