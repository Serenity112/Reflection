using UnityEngine;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusSpriteRemove",
                 "FungusSpriteRemove")]
    public class FungusSpriteRemove : Command
    {
        [SerializeField]
        private string CharacterName;

        [SerializeField]
        private float DisappearSpeed = 3f;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }
        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            yield return StartCoroutine(SpriteRemover.instance.RemoveSprite(CharacterName, DisappearSpeed, Typewriter.Instance.skipping));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(100, 100, 150, 255);
        }
    }
}
