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

    public void SetMovementSprites(GameObject sprite, Vector3 vector, float smoothTime, float vExtend, bool skip)
    {
        StartCoroutine(IMoveSprite(sprite, vector, smoothTime, vExtend, skip));
    }

    public IEnumerator IMoveSprite(GameObject sprite, Vector3 vect, float smoothTime, float vExtend, bool skip)
    {
        if (skip || sprite.transform.localPosition == vect)
        {
            sprite.transform.localPosition = vect;

            yield return null;
        }
        else
        {
            Vector3 velocity1 = Vector3.zero;
            Vector3 Evect1 = new Vector3(vExtend * vect.x, vExtend * vect.y, vExtend * vect.z);

            while (sprite.transform.localPosition != Evect1)
            {
                if (Math.Abs(Math.Abs(sprite.transform.localPosition.x) - Math.Abs(vect.x)) < 1)
                {
                    sprite.transform.localPosition = vect;
                    DialogMod.denyNextDialog = false;
                    yield break;
                }

                sprite.transform.localPosition = Vector3.SmoothDamp(sprite.transform.localPosition, Evect1, ref velocity1, smoothTime);

                yield return null;
            }
        }

        DialogMod.denyNextDialog = false;
    }
}