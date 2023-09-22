using Fungus;
using System;
using System.Collections;
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

    public void StopPrev(bool allowExpand, bool skip)
    {
        // isExecuting добавлено чтобы не вызывать загрузку позиций каждый диалог, а только когда что-то реально движется
        // Другие анимации редкие, поэтому там это условие игнорируется. Диалогов же больше чем свапов, появлений.
        if (isExecuting)
        {
            isExecuting = false;

            if (!skip && allowExpand)
            {
                StopAllCoroutines();
                SpriteController.instance.LoadSpritesExpandingInfo();
            }
        }
    }

    public void SetExpanding(string characterName, bool skip)
    {
        bool allowExpand = SettingsConfig.IfAllowExpandings();

        if (!Typewriter.Instance.SkipIsActive)
        {
            StopPrev(allowExpand, skip);
        } else
        {
            Typewriter.Instance.SkipIsActive = false;
            StopAllCoroutines();
            SpriteController.instance.LoadSpritesExpandingInfo();
        }

        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteNumByName(characterName);

        if (sprite_obj != null)
        {
            GameSpriteObject sprite = (GameSpriteObject)sprite_obj;

            if (lastExpandedSpriteName != characterName)
            {
                SpriteData spriteData = SpriteController.instance.GameSpriteData[sprite.num];

                if (!spriteData.preloaded)
                {
                    Vector3 newScale = SpriteController.instance.CharactersScales[characterName] * expand_coefficient;

                    SpriteController.instance.SaveSpriteData(sprite.num, true);

                    if (Typewriter.Instance.SkipIsActive)
                    {
                        
                    }

                    // Для синхронизации увеличения при смене спрайтов
                    int linkedSprite = spriteData.prevSprite;
                    if (linkedSprite != -1)
                    {
                        SpriteData linkedSpriteData = SpriteController.instance.GameSpriteData[linkedSprite];
                        if (allowExpand && !skip && !linkedSpriteData.preloaded)
                        {
                            StartCoroutine(Expand(SpriteController.instance.GameSprites[linkedSprite], newScale, expand_time));
                        }
                    }

                    if (allowExpand && !skip && !spriteData.preloaded)
                    {
                        StartCoroutine(Expand(sprite, newScale, expand_time));
                    }

                    // Нет условия на скип, т.к. при начале сикпа старые спрайты должны уменьшиться
                    if (allowExpand && lastExpandedSpriteName != null)
                    {
                        shrinkOldSprite(lastExpandedSpriteName, skip);
                    }

                    lastExpandedSpriteName = characterName;
                }
            }
        }
        else
        {
            if (lastExpandedSpriteName != null)
            {
                if (allowExpand)
                {
                    shrinkOldSprite(lastExpandedSpriteName, Typewriter.Instance.SkipIsActive);
                }

                lastExpandedSpriteName = null;
            }
        }
    }

    private void shrinkOldSprite(string name, bool skip)
    {
        GameSpriteObject? old_sprite = SpriteController.instance.GetSpriteNumByName(name);

        if (old_sprite != null)
        {
            GameSpriteObject sprite = (GameSpriteObject)old_sprite;

            SpriteController.instance.SaveSpriteData(sprite.num, false);

            /*int linkedSprite = SpriteController.instance.GameSpriteData[sprite.num].prevSprite;
            if (linkedSprite != -1)
            {
                StartCoroutine(Expand(SpriteController.instance.GameSprites[linkedSprite].ByPart(SpritePart.Body), SpriteController.instance.CharactersScales[name], expand_time));
            }*/

            StartCoroutine(Expand(sprite, SpriteController.instance.CharactersScales[name], expand_time, skip));
        }
    }

    public IEnumerator Expand(GameSpriteObject sprite, Vector3 newScale, float smoothTime, bool skip = false)
    {
        isExecuting = true;

        if (skip)
        {
            sprite.SetScale(newScale);
            isExecuting = false;
            yield break;
        }

        Vector3 velocity = Vector3.zero;

        bool targetGstart = newScale.x > sprite.GetScale().x;

        while (sprite.GetScale() != newScale)
        {
            float diff = targetGstart ? newScale.x - sprite.GetScale().x : sprite.GetScale().x - newScale.x;

            if (Math.Abs(diff) < 0.0001)
            {
                sprite.SetScale(newScale);
                isExecuting = false;
                yield break;
            }

            sprite.SetScale(Vector3.SmoothDamp(sprite.GetScale(), newScale, ref velocity, smoothTime));

            yield return null;
        }
    }
}
