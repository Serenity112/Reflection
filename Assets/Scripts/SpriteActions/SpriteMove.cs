using System.Collections;
using UnityEngine;
using System;

public class SpriteMove : MonoBehaviour
{
    public static SpriteMove instance = null;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }

    public void StopSpriteMoving()
    {
        StopAllCoroutines();
    }

    public void SetSpriteMovement(string name, Vector3 vector, float smoothTime, bool skip)
    {
        StartCoroutine(IMoveSprite(name, vector, smoothTime, skip));
    }

    public IEnumerator IMoveSprite(string name, Vector3 targetVect, float smoothTime, bool skip)
    {
        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteNumByName(name);

        if (sprite_obj == null)
        {
            yield break;
        }

        GameSpriteObject sprite = (GameSpriteObject)sprite_obj;

        yield return StartCoroutine(IMoveSprite(sprite, targetVect, smoothTime, skip));
    }

    public IEnumerator IMoveSprite(GameSpriteObject sprite, Vector3 targetVect, float smoothTime, bool skip)
    {
        StopSpriteMoving();
        SpriteFade.instance.StopSpritesFading();
        SpriteController.instance.LoadSpritesDataInfo();

        SpriteController.instance.SaveSpriteData(sprite.num, targetVect);

        if (skip || sprite.GetPosition() == targetVect)
        {
            sprite.SetPosition(targetVect);

            yield return null;
        }
        else
        {
            Vector3 velocity = Vector3.zero;

            bool targetGstart = targetVect.x > sprite.GetPosition().x;

            while (sprite.GetPosition() != targetVect)
            {
                float diff = targetGstart ? targetVect.x - sprite.GetPosition().x : sprite.GetPosition().x - targetVect.x;

                if (Math.Abs(diff) < 1)
                {
                    sprite.SetPosition(targetVect);
                    Typewriter.Instance.AllowSkip();
                    yield break;
                }

                sprite.SetPosition(Vector3.SmoothDamp(sprite.GetPosition(), targetVect, ref velocity, smoothTime));

                yield return null;
            }
        }

        Typewriter.Instance.AllowSkip();
    }
}
