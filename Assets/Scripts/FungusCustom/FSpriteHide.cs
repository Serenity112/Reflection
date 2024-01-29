using UnityEngine;
using System.Collections;
using System;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FSpriteHide",
                 "FSpriteHide")]
    public class FSpriteHide : Command
    {
        [SerializeField]
        private string CharacterName;

        [SerializeField]
        private float DisappearTime = 0.5f;

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

            yield return StartCoroutine(SpriteRemover.instance.RemoveSprite(character_input, DisappearTime, skip, false, WaitForFinished));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(100, 100, 150, 255);
        }
    }
}
