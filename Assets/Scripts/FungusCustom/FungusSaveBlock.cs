using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusSaveBlock",
                 "FungusSaveBlock")]
    public class FungusSaveBlock : Command
    {
        [SerializeField] 
        private string blockName;

        public override void OnEnter()
        {
            UserData.instance.CurrentBlock = blockName;

            UserData.instance.CurrentCommandIndex = 0;

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }
    }
}
