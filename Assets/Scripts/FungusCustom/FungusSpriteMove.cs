using System;
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

            StaticVariables.SPRITE_MOVING = true;

            var parsed = Enum.TryParse<global::Character>(CharacterName, true, out global::Character character);
            var character_input = parsed ? character : global::Character.None;

            SpriteMove.instance.SetSpriteMovement(character_input, Position, MovementTime, Typewriter.Instance.SkipIsActive);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(96, 100, 200, 255);
        }
    }
}
