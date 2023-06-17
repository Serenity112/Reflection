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

        [SerializeField]
        private string NewBackgroundName;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            switch (State)
            {
                case StationScrollState.Start:
                    SpecialEventManager.instance.AddEvent(SpecialEvent.StationScroll);
                    yield return StartCoroutine(StationScroll.instance.IAppearBg(Speed));
                    break;
                case StationScrollState.Scroll:
                    yield return StartCoroutine(StationScroll.instance.IScrollBg(Speed, DialogMod.skipping));
                    break;
                case StationScrollState.End:
                    yield return StartCoroutine(StationScroll.instance.IEndScroll(NewBackgroundName, Speed));
                    SpecialEventManager.instance.DeleteEvent(SpecialEvent.StationScroll);
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
