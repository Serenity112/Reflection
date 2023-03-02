using UnityEngine;
using UnityEngine.UI;
namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusChangeSprite",
                 "FungusChangeSprite")]
    public class FungusChangeSprite : Command
    {
        public GameObject SpriteController;


        public int panel;
        public string character;
        public int sprite;
        public int num;

        public GameObject scene;

        public override void OnEnter()
        {
            Player.IncreaseIndex(1);
           // SpriteController.GetComponent<SpriteController>().ChangeSprite(panel, character, sprite, num);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(200, 130, 40, 255);
        }

    }
}