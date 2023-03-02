using UnityEngine;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusNewSpriteAppear",
                 "FungusNewSpriteAppear")]
    public class FungusNewSpriteAppear : Command
    {
        [SerializeField]
        private string characterName;

        [SerializeField]
        private int Pose;

        [SerializeField]
        private int Emotion;

        [SerializeField]
        private Vector3 Position;

        [SerializeField]
        private float appearSpeed;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            if (appearSpeed == 0)
                appearSpeed = 3f;

            yield return StartCoroutine(SpriteApearer.instance.SpriteAppear(characterName, Pose, Emotion, Position, appearSpeed, DialogMod.skipping));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(100, 200, 220, 255);
        }

    }
}