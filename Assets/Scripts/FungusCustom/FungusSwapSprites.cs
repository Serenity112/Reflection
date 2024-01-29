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
        private float PositionX = -1;

        [SerializeField]
        private float AppearTime = 0.3f;

        [SerializeField]
        private float DisappearTime = 0.3f;

        [SerializeField]
        private float MovementTime = 0.1f;

        [SerializeField]
        private bool WaitForFinished = false;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            var parsed = Enum.TryParse<global::Character>(CharacterName, true, out global::Character character);
            var character_input = parsed ? character : global::Character.None;

            bool skip = Typewriter.Instance.SkipIsActive;
            Vector3 position = new Vector3(PositionX, 0, 0);

            yield return StartCoroutine(
                SpritesSwapper.instance.SwapSprites(
                    character_input, Pose, Emotion, position, DisappearTime, AppearTime, MovementTime, skip, WaitForFinished));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(70, 110, 200, 255);
        }
    }
}
