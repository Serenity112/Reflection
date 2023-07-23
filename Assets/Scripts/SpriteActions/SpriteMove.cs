using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Fungus;

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

    public void SetMovementSprites(GameObject sprite, Vector3 vector, float smoothTime, bool skip)
    {
        StartCoroutine(IMoveSprite(sprite, vector, smoothTime, skip));
    }

    public IEnumerator IMoveSprite(GameObject sprite, Vector3 targetVect, float smoothTime, bool skip)
    {
        if (skip || sprite.transform.localPosition == targetVect)
        {
            sprite.transform.localPosition = targetVect;

            yield return null;
        }
        else
        {
            Vector3 velocity = Vector3.zero;

            bool targetGstart = targetVect.x > sprite.transform.localPosition.x;

            while (sprite.transform.localPosition != targetVect)
            {
                float diff = targetGstart ? targetVect.x - sprite.transform.localPosition.x : sprite.transform.localPosition.x - targetVect.x;

                if (Math.Abs(diff) < 1)
                {
                    sprite.transform.localPosition = targetVect;
                   Typewriter.Instance.denyNextDialog = false;
                    yield break;
                }

                sprite.transform.localPosition = Vector3.SmoothDamp(sprite.transform.localPosition, targetVect, ref velocity, smoothTime);

                yield return null;
            }
        }

       Typewriter.Instance.denyNextDialog = false;
    }
}
