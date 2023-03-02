
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow",
                 "FungusStartLoad",
                 "Stops executing the named Block")]
    public class FungusStartLoad : Command
    {
        GameObject scene;
        public override void OnEnter()
        {
            scene = GameObject.Find("Scene");
            Player.StartingLoad();
            Player.IncreaseIndex(1);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

    }
}