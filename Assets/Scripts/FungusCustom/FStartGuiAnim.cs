using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                   "FStartGuiAnim",
                   "FStartGuiAnim")]
    public class FStartGuiAnim : Command
    {
        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            

            //yield return new WaitForSeconds(2.5f);

            yield return StartCoroutine(PanelsManager.instance.EnableGuiOnStart(false));

            yield return new WaitForSeconds(0.5f);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(255, 102, 102, 255);
        }
    }
}
