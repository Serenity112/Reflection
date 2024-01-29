using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

public class SpriteMove : MonoBehaviour
{
    public static SpriteMove instance = null;

    void Awake()
    {
        instance = this;
    }

    public void StopSpriteMoving()
    {
        StopAllCoroutines();
    }

    // Вместо класса SpriteMover
    public IEnumerator IMoveSprite(Character character, Vector3 newPosition, float smoothTime, bool skip)
    {
        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteByCharacter(character);
        if (sprite_obj == null)
        {
            yield break;
        }
        GameSpriteObject sprite = (GameSpriteObject)sprite_obj;

        sprite.CompletePostAction();
        SpriteController.instance.CompleteAnimations(sprite.Number);
        SpriteController.instance.LoadSpriteActualData(character);

        SpriteController.instance.SaveSpritePosition(character, newPosition);

        IEnumerator move = IMoveSprite(sprite, newPosition, smoothTime, skip);
        SpriteController.instance.AddAnimation(sprite.Number, move);
        StartCoroutine(move);

        yield return null;
    }

    private const float SkipStep = 10f;
    private const float DefaultStep = 1f;
    public IEnumerator IMoveSprite(GameSpriteObject sprite, Vector3 targetVect, float smoothTime, bool skip)
    {
        if (skip || sprite.GetPosition() == targetVect)
        {
            sprite.SetPosition(targetVect);
            yield break;
        }
        else
        {
            Vector3 velocity = Vector3.zero;

            while (sprite.GetPosition() != targetVect)
            {
                float x1 = sprite.GetPosition().x;
                float x2 = targetVect.x;
                float diff = Math.Max(x1, x2) - Math.Min(x1, x2);

                if (Math.Abs(diff) < 1)
                {
                    sprite.SetPosition(targetVect);
                    yield break;
                }

                sprite.SetPosition(Vector3.SmoothDamp(sprite.GetPosition(), targetVect, ref velocity, smoothTime));
                yield return null;
            }
        }
    }

    private IEnumerator WaitForAll(List<IEnumerator> coroutines)
    {
        int tally = 0;

        foreach (IEnumerator c in coroutines)
        {
            StartCoroutine(RunCoroutine(c));
        }

        while (tally > 0)
        {
            yield return null;
        }

        IEnumerator RunCoroutine(IEnumerator c)
        {
            tally++;
            yield return StartCoroutine(c);
            tally--;
        }
    }
}
