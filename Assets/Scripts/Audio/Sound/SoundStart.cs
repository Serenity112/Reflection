using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "SoundStart",
                 "SoundStart")]
    public class SoundStart : Command
    {
        [SerializeField]
        private string SoundName;

        [SerializeField]
        private float Volume = 1f;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.SoundStart(SoundName, Volume));

            Continue();
        }


        public override Color GetButtonColor()
        {
            return new Color32(96, 63, 97, 255);
        }
    }
}
