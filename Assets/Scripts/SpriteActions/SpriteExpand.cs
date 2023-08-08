using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


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

        SpriteController.instance.SkipSpritesExpanding();
    }

    public void SetExpanding(string characterName, bool skip)
    {
        bool allowExpand = SettingsConfig.chosenOptions[Settings.SpriteExpand].data == 1;

        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteNumByName(characterName);

        if (sprite_obj != null)
        {
            GameSpriteObject sprite = (GameSpriteObject)sprite_obj;

            if (lastExpandedSpriteName != characterName)
            {
                Vector3 newScale = SpriteController.instance.CharactersScales[characterName] * expand_coefficient;

                if (!skip)
                {
                    SpriteController.instance.SaveSpriteData(sprite.num, true);
                }

                int linkedSprite = SpriteController.instance.GameSpriteData[sprite.num].prevSprite;
                if (linkedSprite != -1)
                {
                    if (allowExpand && !skip)
                    {
                        StartCoroutine(Expand(SpriteController.instance.GameSprites[linkedSprite].ByPart(SpritePart.Body), newScale, expand_time));
                    }
                }

                if (allowExpand && !skip)
                {
                    StartCoroutine(Expand(sprite.ByPart(SpritePart.Body), newScale, expand_time));
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
        GameSpriteObject? old_sprite = SpriteController.instance.GetSpriteNumByName(lastExpandedSpriteName);

        if (old_sprite != null)
        {
            GameSpriteObject sprite = (GameSpriteObject)old_sprite;

            SpriteController.instance.SaveSpriteData(sprite.num, false);

            int linkedSprite = SpriteController.instance.GameSpriteData[sprite.num].prevSprite;
            if (linkedSprite != -1)
            {
                StartCoroutine(Expand(SpriteController.instance.GameSprites[linkedSprite].ByPart(SpritePart.Body), SpriteController.instance.CharactersScales[lastExpandedSpriteName], expand_time));
            }

            StartCoroutine(Expand(sprite.ByPart(SpritePart.Body), SpriteController.instance.CharactersScales[lastExpandedSpriteName], expand_time));
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
