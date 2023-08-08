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
        private string CharacterName;

        [SerializeField]
        private int Pose;

        [SerializeField]
        private int Emotion;

        [SerializeField]
        private Vector3 Position = Vector3.zero;

        [SerializeField]
        private float AppearSpeed = 3f;

        [SerializeField]
        private bool WaitForFinished = true;

        [SerializeField]
        private bool StopPrev = true;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return StartCoroutine(SpriteApearer.instance.SpriteAppear(CharacterName, Pose, Emotion, Position, AppearSpeed, Typewriter.Instance.skipping, WaitForFinished, StopPrev));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(100, 200, 220, 255);
        }

    }
}
