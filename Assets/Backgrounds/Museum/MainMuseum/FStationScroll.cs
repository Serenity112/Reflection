using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                   "FStationScroll",
                   "FStationScroll")]
    public class FStationScroll : Command
    {
        [SerializeField]
        private StationScrollState State;

        [SerializeField]
        private float Speed = 3f;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            switch (State)
            {
                case StationScrollState.Scroll:
                    yield return StartCoroutine(((StationScroll)SpecialEventManager.instance.currentEvent).IScrollBg(Speed, Typewriter.Instance.isSkipping));
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
