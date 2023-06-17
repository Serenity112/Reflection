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
        private DreamSnowState State;

        [SerializeField]
        private float Speed = 6f;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            switch (State)
            {
                case DreamSnowState.Start:
                    SpecialEventManager.instance.AddEvent(SpecialEvent.DreamSnow);
                    yield return StartCoroutine(DreamSnow.instance.IStartDreamSnow(Speed));
                    break;
                case DreamSnowState.Launch:
                    yield return StartCoroutine(DreamSnow.instance.IRocketLaunch(Speed));
                    break;
                case DreamSnowState.End:
                    yield return StartCoroutine(DreamSnow.instance.IEndDreamSnow("AssemblyHall", Speed));
                    SpecialEventManager.instance.DeleteEvent(SpecialEvent.DreamSnow);
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
