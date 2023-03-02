using UnityEngine;
using UnityEngine.UI;
namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusName",
                 "FungusName")]
    public class FungusName : Command
    {
        public GameObject saydialog;

        public bool enable;
        public int i;

        public override void OnEnter()
        {
            if(enable)
            {
                //saydialog.GetComponent<NameChanger>().SetName(i);
            }
            else
            {
               // saydialog.GetComponent<NameChanger>().DelName();
            }
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(96, 150, 200, 255);
        }

    }
}