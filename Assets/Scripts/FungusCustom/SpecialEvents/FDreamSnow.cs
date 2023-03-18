using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                   "FDreamSnow",
                   "FDreamSnow")]
    public class FDreamSnow : Command
    {
        [SerializeField]
        private DreamSnowState state;

        [SerializeField]
        private float speed;

        [SerializeField]
        private int new_bg = -1;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            if (speed == 0f)
            {
                speed = 6f;
            }

            switch (state)
            {
                case DreamSnowState.Start:
                    yield return StartCoroutine(DreamSnow.instance.IStartDreamSnow(speed));
                    break;
                case DreamSnowState.Launch:
                    yield return StartCoroutine(DreamSnow.instance.IRocketLaunch(speed));
                    break;
                case DreamSnowState.End:
                    yield return StartCoroutine(DreamSnow.instance.IEndDreamSnow(new_bg, speed));                 
                    break;
            }

            Continue();
        }
        public override Color GetButtonColor()
        {
            return new Color32(192, 192, 192, 255);
        }
    }
}
