using System;
using System.Collections;
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
        private float PositionX = 0;

        [SerializeField]
        private float MovementTime = 0.1f;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            StaticVariables.SPRITE_MOVING = true;

            var parsed = Enum.TryParse<global::Character>(CharacterName, true, out global::Character character);
            var character_input = parsed ? character : global::Character.None;
            bool skip = Typewriter.Instance.SkipIsActive;
            Vector3 newPosition = new Vector3(PositionX, 0, 0);
     
            yield return StartCoroutine(SpriteMove.instance.IMoveSprite(character_input, newPosition, MovementTime, skip));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(96, 100, 200, 255);
        }
    }
}
