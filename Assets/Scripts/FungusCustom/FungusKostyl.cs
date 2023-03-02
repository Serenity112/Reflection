using UnityEngine;
namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusKostyl",
                 "FungusKostyl")]
    public class FungusKostyl : Command
    {
        public GameObject saveDialogCanvas;

        public override void OnEnter()
        {
            saveDialogCanvas.GetComponent<DialogMod>().SetDialogClickedFlag();
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(200, 250, 210, 255);
        }

    }
}