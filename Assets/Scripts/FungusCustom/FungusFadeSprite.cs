using UnityEngine;
using UnityEngine.UI;
namespace Fungus
{
    [CommandInfo("Flow",
                 "FungusFadeSprite",
                 "FungusFadeSprite")]
    public class FungusFadeSprite : Command
    {
        public GameObject CharacterFader;

        public int character;
        public bool fadeIn;
        public float speed;
        public GameObject scene;

        public override void OnEnter()
        {
           // scene.GetComponent<Player>().IncreaseIndex(1);
           // CharacterFader.GetComponent<SpriteFade>().SetFading(character, fadeIn, speed);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(200, 130, 40, 255);
        }

    }
}