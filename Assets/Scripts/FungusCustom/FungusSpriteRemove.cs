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
        private string characterName;

        [SerializeField]
        private float speed;

        [SerializeField]
        private bool extend;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }
        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            if(speed == 0f)
            {
                speed = 3f;
            }

            yield return StartCoroutine(SpriteRemover.instance.RemoveSprite(characterName, speed, extend, DialogMod.skipping));

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(100, 100, 150, 255);
        }
    }
}