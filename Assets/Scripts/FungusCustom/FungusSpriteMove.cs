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
        private float time;

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

            if (time == 0f)
            {
                time = 0.1f;
            }
                
            int spriteNum = SpriteController.instance.GetActivityByName(characterName);

            SpriteController.instance.SaveSpriteData(spriteNum, position);

            SpriteMove.instance.SetMovementSprites(spriteNum, position, time, vExtend, DialogMod.skipping);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(96, 100, 200, 255);
        }

    }
}