using System;
using System.Collections;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusSwapSprites",
                 "FungusSwapSprites")]
    public class FungusSwapSprites : Command
    {
        [SerializeField]
        private string CharacterName;

        [SerializeField]
        private int Pose;

        [SerializeField]
        private int Emotion;

        [SerializeField]
        private Vector3 newPosition = new Vector3(-1, 0, 0);

        [SerializeField]
        private float AppearSpeed = 3f;

        [SerializeField]
        private float DisappearSpeed = 5f;

        [SerializeField]
        private float MovementTime = 0.1f;

        [SerializeField]
        private bool WaitForFinished = false;

        [SerializeField]
        private bool StopPrev = true;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            StaticVariables.SPRITE_LOADING = true;

            var parsed = Enum.TryParse<global::Character>(CharacterName, true, out global::Character character);
            var character_input = parsed ? character : global::Character.None;

            yield return StartCoroutine(SpritesSwapper.instance.SwapSprites(character_input, Pose, Emotion, newPosition, DisappearSpeed, AppearSpeed, MovementTime, Typewriter.Instance.SkipIsActive, WaitForFinished, StopPrev));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(70, 110, 200, 255);
        }
    }
}
