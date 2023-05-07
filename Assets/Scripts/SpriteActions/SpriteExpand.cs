using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Fungus;


public class SpriteExpand : MonoBehaviour
{
    public static SpriteExpand instance = null;

    public string lastExpandedSpriteName = null;

    public bool isExecuting = false;

    public float expand_coefficient = 1.035f;

    private float expand_time = 0.2f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        lastExpandedSpriteName = null;
    }

    public void StopPrev()
    {
        // isExecuting добавлено чтобы не вызывать загрузку позиций каждый диалог, а только когда что-то реально движется
        // Другие анимации редкие, поэтому там это условие игнорируется. Диалогов же больше чем свапов, появлений.
        if (!isExecuting)
            return;

        isExecuting = false;

        StopAllCoroutines();

        SpriteController.instance.SkipSpriteActions_Expand();
    }

    public void SetExpanding(string characterName, bool skip)
    {
        int spriteNum = SpriteController.instance.GetSpriteByName(characterName);

        if (spriteNum != -1)
        {
            if (lastExpandedSpriteName != characterName)
            {
                GameObject newSprite = SpriteController.instance.GetSprite(spriteNum);
                Vector3 newScale = newSprite.transform.localScale * expand_coefficient;

                SpriteController.instance.SaveSpriteData(spriteNum, true);

                int linkedSprite = SpriteController.instance.GameSpriteData[spriteNum].prevSprite;
                if (linkedSprite != -1)
                {
                    StartCoroutine(Expand(SpriteController.instance.GetSprite(linkedSprite), newScale, expand_time, skip));
                }

                StartCoroutine(Expand(newSprite, newScale, expand_time, skip));

                if (lastExpandedSpriteName != null)
                {
                    shrinkOldSprite(skip);
                }

                lastExpandedSpriteName = characterName;
            }
        }
        else
        {
            if (lastExpandedSpriteName != null)
            {
                shrinkOldSprite(skip);

                lastExpandedSpriteName = null;
            }
        }
    }

    private void shrinkOldSprite(bool skip)
    {
        int oldSpriteNum = SpriteController.instance.GetSpriteByName(lastExpandedSpriteName);

        if (oldSpriteNum != -1)
        {
            GameObject oldSprite = SpriteController.instance.GetSprite(oldSpriteNum);

            SpriteController.instance.SaveSpriteData(oldSpriteNum, false);

            int linkedSprite = SpriteController.instance.GameSpriteData[oldSpriteNum].prevSprite;
            if (linkedSprite != -1)
            {
                StartCoroutine(Expand(SpriteController.instance.GetSprite(linkedSprite), SpriteController.instance.CharactersScales[lastExpandedSpriteName], expand_time, skip));
            }

            StartCoroutine(Expand(oldSprite, SpriteController.instance.CharactersScales[lastExpandedSpriteName], expand_time, skip));
        }
    }


    private IEnumerator Expand(GameObject Sprite, Vector3 newScale, float smoothTime, bool skip)
    {
        isExecuting = true;

        if (skip)
        {
            Sprite.transform.localScale = newScale;
            isExecuting = false;
            yield break;
        }
        else
        {
            Vector3 velocity1 = Vector3.zero;

            while (Sprite.transform.localScale != newScale)
            {
                if (Math.Abs(Math.Abs(Sprite.transform.localScale.x) - Math.Abs(newScale.x)) < 0.001)
                {
                    Sprite.transform.localScale = newScale;
                    isExecuting = false;
                    yield break;
                }

                Sprite.transform.localScale = Vector3.SmoothDamp(Sprite.transform.localScale, newScale, ref velocity1, smoothTime);

                yield return null;
            }
        }
    }
}