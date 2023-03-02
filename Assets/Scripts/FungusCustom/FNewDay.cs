using System.Collections;
using UnityEngine;
using UnityEngine.UI;
namespace Fungus
{
    [CommandInfo("Ref",
                 "FNewDay",
                 "FNewDay")]
    public class FNewDay : Command
    {
        [SerializeField]
        private int dayIndex;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());           
        }

        IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            yield return StartCoroutine(DaysManager.instance.ILoadDay(dayIndex));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(200, 130, 40, 255);
        }

    }
}