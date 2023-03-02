using UnityEngine;
namespace Fungus
{
    [CommandInfo("Flow",
                 "FungusSetSpritePos",
                 "FungusSetSpritePos")]
    public class FungusSetSpritePos : Command
    {
        public string Sprite;
        public Vector3 TargetVector;

        public override void OnEnter()
        {
            GameObject SpriteManager = this.transform.parent.gameObject.transform.Find("SpriteManager").gameObject;
            int num = SpriteManager.GetComponent<SpriteController>().GetActivityByName(Sprite);

            GameObject Current = SpriteManager.GetComponent<SpriteController>().GetSprite(num);
            GameObject scene = this.transform.parent.gameObject;

            Current.transform.localPosition = TargetVector;

            // scene.GetComponent<Player>().SaveSpriteData(Sprite, TargetVector);
            Player.IncreaseIndex(1);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(130, 100, 180, 255);
        }

    }
}