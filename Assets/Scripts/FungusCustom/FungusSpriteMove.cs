using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow",
                 "FungusSpriteMove",
                 "FungusSpriteMove")]
    public class FungusSpriteMove : Command
    {
        [SerializeField]
        private string characterName;

        [SerializeField]
        private Vector3 position;

        [SerializeField]
        private float time = 0.1f;

        [SerializeField]
        private float vExtend;
        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;
            // Скип идёт от сюда, т.к. нет класса типо SpriteMover. SpriteMove и SpriteFade вспомогательные классы, в которых писать пропуск не хочется.
            SpriteMove.instance.StopSpriteMoving();
            SpriteFade.instance.StopSpritesFading();
            SpriteController.instance.SkipSpriteActions();

            DialogMod.denyNextDialog = true; // Запрет отключтся в конце выполнения SetMovementSprites

            int spriteNum = SpriteController.instance.GetSpriteByName(characterName);

            SpriteController.instance.SaveSpriteData(spriteNum, position);

            GameObject sprite = SpriteController.instance.GetSprite(spriteNum);

            SpriteMove.instance.SetMovementSprites(sprite, position, time, vExtend, DialogMod.skipping);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(96, 100, 200, 255);
        }

    }
}