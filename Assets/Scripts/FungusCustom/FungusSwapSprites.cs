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
        private string characterName;

        [SerializeField]
        private int Pose;

        [SerializeField]
        private int Emotion;

        [SerializeField]
        private Vector3 newPosition;

        [SerializeField]
        private float disappearSpeed;

        [SerializeField]
        private float appearSpeed;

        [SerializeField]
        private float movementSpeed;

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

            if (disappearSpeed * appearSpeed == 0)
            {
                appearSpeed = 3f;
                disappearSpeed = 3.2f;
            }

            if(newPosition != Vector3.zero && movementSpeed == 0)
            {
                movementSpeed = 3f;
            }

            if(delayBetweenNext)
            {
                yield return StartCoroutine(SpritesSwapper.instance.SwapSprites(characterName, Pose, Emotion, newPosition, disappearSpeed, appearSpeed, movementSpeed, DialogMod.skipping));

                Continue();
            } else
            {
                StartCoroutine(SpritesSwapper.instance.SwapSprites(characterName, Pose, Emotion, newPosition, disappearSpeed, appearSpeed, movementSpeed, DialogMod.skipping));

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