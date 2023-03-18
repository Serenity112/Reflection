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
        private string backgroundName;

        [SerializeField]
        private float speed;

        [SerializeField]
        private bgSwapType fadeType;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            UserData.instance.CurrentBG = backgroundName;

            if(speed == 0f)
            {
                speed = 6f;
            }

            yield return StartCoroutine(BackgroundManager.instance.ISwap(backgroundName, fadeType, speed));

            Continue();
        }
        public override Color GetButtonColor()
        {
            return new Color32(184, 94, 39, 255);
        }
    }
}