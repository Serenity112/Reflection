using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "AmbientEnd",
                 "AmbientEnd")]
    public class AmbientEnd : Command
    {
        [SerializeField]
        private string AmbientName;

        [SerializeField]
        private float Time = 2f;

        public override void OnEnter()
        {
            

            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return AudioManager.instance.StartCoroutine(AudioManager.instance.AudioLineEnd(AudioManager.AudioLine.Ambient, AmbientName, Time));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(111, 53, 112, 255);
        }
    }
}
