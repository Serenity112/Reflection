using UnityEngine;
namespace Fungus
{
    /// <summary>
    /// Stops executing the named Block.
    /// </summary>
    [CommandInfo("Flow",
                 "FungusIncreaseIndex",
                 "Stops executing the named Block")]
    public class FungusIncreaseIndex : Command
    {
        public int index;
       
        public override void OnEnter()
        {
            Player.IncreaseIndex(index);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

    }
}