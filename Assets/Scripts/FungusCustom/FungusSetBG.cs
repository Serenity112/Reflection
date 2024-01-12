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
        private float AppearSpeed = 2.5f;

        [SerializeField]
        private float Delay = 0.5f;

        [SerializeField]
        private BgSwapType FadeType;

        public override void OnEnter() => StartCoroutine(IOnEnter());

        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            yield return StartCoroutine(BackgroundManager.instance.ISwap(BackgroundName, FadeType, AppearSpeed, Delay));

            Continue();
        }
        public override Color GetButtonColor()
        {
            return new Color32(184, 94, 39, 255);
        }
    }
}
