using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                   "FStationScroll",
                   "FStationScroll")]
    public class FStationScroll : Command
    {
        private float Speed = 3f;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            

            yield return StartCoroutine(((StationScroll)SpecialEventManager.instance.CurrentEventObject).IScrollBg(Speed));

            Continue();
        }
        public override Color GetButtonColor()
        {
            return new Color32(192, 192, 192, 255);
        }
    }
}
