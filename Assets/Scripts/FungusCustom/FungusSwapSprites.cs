using UnityEngine;
using System.Collections;

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
        private Vector3 newPosition = Vector3.zero;

        [SerializeField]
        private float appearSpeed = 3f;

        [SerializeField]
        private float disappearSpeed = 5f;

        [SerializeField]
        private float movementSpeed = 0f;

        // ����� �����, ����� ������ ��� ����� �������� � ������� � ��������. ��� ����� �����, ���� �������� ����������. ����� ����� ������������� ������
        [SerializeField]
        private bool delayBetweenNext = false;

        public override void OnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            DialogMod.denyNextDialog = true; // ���������� ����� � ����� ���������� SwapSprites.

            if (delayBetweenNext)
            {
                yield return StartCoroutine(SpritesSwapper.instance.SwapSprites(CharacterName, Pose, Emotion, newPosition, disappearSpeed, appearSpeed, movementSpeed, DialogMod.skipping));

                Continue();
            }
            else
            {
                StartCoroutine(SpritesSwapper.instance.SwapSprites(CharacterName, Pose, Emotion, newPosition, disappearSpeed, appearSpeed, movementSpeed, DialogMod.skipping));

                yield return null;

                Continue();
            }
        }

        public override Color GetButtonColor()
        {
            return new Color32(70, 110, 200, 255);
        }
    }
}