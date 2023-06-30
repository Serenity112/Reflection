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

    private float expand_time = 0.1f;

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

        SpriteController.instance.LoadSpritesExpandings();
    }

    public void SetExpanding(string characterName, bool skip)
    {
        bool allowExpand = SettingsConfig.chosenOptions[Settings.SpriteExpand].data == 1;

        int spriteNum = SpriteController.instance.GetSpriteByName(characterName);

        if (spriteNum != -1)
        {
            if (lastExpandedSpriteName != characterName)
            {
                GameObject newSprite = SpriteController.instance.GetSprite(spriteNum);
                Vector3 newScale = SpriteController.instance.CharactersScales[characterName] * expand_coefficient;

                if (!skip)
                {
                    SpriteController.instance.SaveSpriteData(spriteNum, true);
                }

                int linkedSprite = SpriteController.instance.GameSpriteData[spriteNum].prevSprite;
                if (linkedSprite != -1)
                {
                    if (allowExpand && !skip)
                    {
                        StartCoroutine(Expand(SpriteController.instance.GetSprite(linkedSprite), newScale, expand_time));
                    }
                }

                if (allowExpand && !skip)
                {
                    StartCoroutine(Expand(newSprite, newScale, expand_time));
                }

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
                StartCoroutine(Expand(SpriteController.instance.GetSprite(linkedSprite), SpriteController.instance.CharactersScales[lastExpandedSpriteName], expand_time));
            }

            StartCoroutine(Expand(oldSprite, SpriteController.instance.CharactersScales[lastExpandedSpriteName], expand_time));
        }
    }


    private IEnumerator Expand(GameObject sprite, Vector3 newScale, float smoothTime)
    {
        isExecuting = true;

        /*if (skip)
        {
            sprite.transform.localScale = newScale;
            isExecuting = false;
            yield break;
        }*/

        Vector3 velocity = Vector3.zero;

        bool targetGstart = newScale.x > sprite.transform.localPosition.x;

        while (sprite.transform.localScale != newScale)
        {
            float diff = targetGstart ? newScale.x - sprite.transform.localPosition.x : sprite.transform.localPosition.x - newScale.x;

            if (Math.Abs(diff) < 0.01)
            {
                sprite.transform.localScale = newScale;
                isExecuting = false;
                yield break;
            }

            sprite.transform.localScale = Vector3.SmoothDamp(sprite.transform.localScale, newScale, ref velocity, smoothTime);

            yield return null;
        }
    }
}
