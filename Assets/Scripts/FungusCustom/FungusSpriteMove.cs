using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow",
                 "FungusSpriteMove",
                 "FungusSpriteMove")]
    public class FungusSpriteMove : Command
    {
        [SerializeField]
        private string CharacterName;

        [SerializeField]
        private Vector3 Position;

        [SerializeField]
        private float MovementTime = 0.1f;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            Typewriter.Instance.DenySkip(); // Запрет отключтся в конце выполнения SetMovementSprites

            SpriteMove.instance.SetSpriteMovement(CharacterName, Position, MovementTime, Typewriter.Instance.isSkipping);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(96, 100, 200, 255);
        }
    }
}
