using UnityEngine;
using System.Collections;
using System;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusNewSpriteAppear",
                 "FungusNewSpriteAppear")]
    public class FungusNewSpriteAppear : Command
    {
        [SerializeField]
        private string CharacterName;

        [SerializeField]
        private int Pose;

        [SerializeField]
        private int Emotion;

        [SerializeField]
        private float PositionX = 0;

        [SerializeField]
        private float AppearTime = 0.5f;

        [SerializeField]
        private bool WaitForFinished = true;

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
                SpriteApearer.instance.SpriteAppear(
                    character_input, Pose, Emotion, position, AppearTime, skip, WaitForFinished));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(100, 200, 220, 255);
        }
    }
}
