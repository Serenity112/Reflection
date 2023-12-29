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
        private float Volume;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

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
