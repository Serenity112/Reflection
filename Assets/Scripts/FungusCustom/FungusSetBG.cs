using UnityEngine;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusSetBG",
                 "FungusSetBG")]
    public class FungusSetBG : Command
    {
        [SerializeField]
        private string BackgroundName;

        [SerializeField]
        private float AppearSpeed = 6f;

        [SerializeField]
        private bgSwapType FadeType;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            UserData.instance.CurrentBG = BackgroundName;

            yield return StartCoroutine(BackgroundManager.instance.ISwap(BackgroundName, FadeType, AppearSpeed));

            Continue();
        }
        public override Color GetButtonColor()
        {
            return new Color32(184, 94, 39, 255);
        }
    }
}
