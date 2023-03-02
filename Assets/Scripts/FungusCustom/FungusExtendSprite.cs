using UnityEngine;
using UnityEngine.UI;
namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusExtendSprite",
                 "FungusExtendSprite")]
    public class FungusExtendSprite : Command
    {
        public string NewSpriteName;
        public int Pose;
        public int Emotion;
        public Vector3 position;
        public int scale;

        public bool extend = false;

        SpriteController spritecontroller;
        GameObject CharacterAnimator;
        GameObject CharacterFader;
        GameObject SpriteManager;
        GameObject scene;

        int NameToNum(string name)
        {
            switch (name)
            {
                case "Katya":
                    return 0;
                case "Nastya":
                    return 1;
                case "Evelina":
                    return 2;
                case "Tanya":
                    return 3;
                case "Pasha":
                    return 4;
                case "Others":
                    return 5;
                default:
                    return 5;
            }
        }

        public override void OnEnter()
        {
            scene = this.transform.parent.gameObject;
            spritecontroller = scene.transform.Find("SpriteManager").gameObject.GetComponent<SpriteController>();
            CharacterFader = scene.transform.Find("CharacterFader").gameObject;
            CharacterAnimator = scene.transform.Find("CharacterAnimator").gameObject;


            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(70, 110, 200, 255);
        }

    }
}