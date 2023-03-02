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

    public void SetMovementSprites(int Character, Vector3 vector, float smoothTime, float vExtend, bool skip)
    {
        StartCoroutine(MoveSprite(Character, vector, smoothTime, vExtend, skip));
    }

    private IEnumerator MoveSprite(int spriteNum, Vector3 vect, float smoothTime, float vExtend, bool skip)
    {
        GameObject Sprite = SpriteController.instance.GetSprite(spriteNum);

        if (skip)
        {
            Sprite.transform.localPosition = vect;

            yield return null;
        }
        else
        {
            Vector3 velocity1 = Vector3.zero;
            Vector3 Evect1 = new Vector3(vExtend * vect.x, vExtend * vect.y, vExtend * vect.z);

            while (Sprite.transform.localPosition != Evect1)
            {
                if (Math.Abs(Math.Abs(Sprite.transform.localPosition.x) - Math.Abs(vect.x)) < 0.001)
                {
                    Sprite.transform.localPosition = vect;
                    DialogMod.denyNextDialog = false;
                    yield break;
                }

                Sprite.transform.localPosition = Vector3.SmoothDamp(Sprite.transform.localPosition, Evect1, ref velocity1, smoothTime);

                yield return null;
            }
        }

        DialogMod.denyNextDialog = false;
    }
}