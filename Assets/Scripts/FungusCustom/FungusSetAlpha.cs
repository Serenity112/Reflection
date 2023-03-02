using UnityEngine;
using UnityEngine.UI;
namespace Fungus
{
    [CommandInfo("Flow",
                 "FungusSetAlpha",
                 "FungusSetAlpha")]
    public class FungusSetAlpha : Command
    {
        public GameObject Sprite1;
        public GameObject Sprite2;
        public GameObject Sprite3;
        public GameObject Sprite4;

        GameObject scene;
        SpriteController spritecontroller;

        GameObject currSprite;

        public int Character;
        public float color;

        void Start()
        {
            scene = this.transform.parent.gameObject;
            spritecontroller = scene.transform.Find("SpriteManager").gameObject.GetComponent<SpriteController>();
        }

        public override void OnEnter()
        {
            GameObject scene = GameObject.Find("Scene");
            spritecontroller.SaveSpriteData(Character, color);
            Player.IncreaseIndex(1);

            switch (Character)
            {
                case 1:
                    currSprite = Sprite1;
                    break;
                case 2:
                    currSprite = Sprite2;
                    break;
                case 3:
                    currSprite = Sprite3;
                    break;
                case 4:
                    currSprite = Sprite4;
                    break;
            }

            currSprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, color);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(0, 163, 54, 255);
        }

    }
}