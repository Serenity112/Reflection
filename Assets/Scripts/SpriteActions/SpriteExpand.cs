using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpriteExpand : MonoBehaviour
{
    public static SpriteExpand instance = null;

    public Character lastExpandedSpriteName = Character.None;

    public bool isExecuting = false;

    public float expand_coefficient = 1.035f;

    private float expand_time = 0.1f;

    private Dictionary<Character, Character> namesAliases = new()
    {
        { Character.Stranger, Character.Nastya },
    };

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

        lastExpandedSpriteName = Character.None;
    }

    public void StopPrev(bool animated)
    {
        // isExecuting добавлено чтобы не вызывать загрузку позиций каждый диалог, а только когда что-то реально движется
        // Другие анимации редкие, поэтому там это условие игнорируется. Диалогов же больше чем свапов, появлений.

        isExecuting = false;

        StopAllCoroutines();

        if (SettingsConfig.IfAllowExpandings())
        {
            SpriteController.instance.LoadSpritesExpandingInfo(animated);
        }
    }

    public void SetExpanding(Character character, bool skipIsActive)
    {
        //Debug.Log("SetExpanding skip: " + skipIsActive);

        // Местоимения, аля Незнакомка - Настя итд
        if (namesAliases.ContainsKey(character))
        {
            character = namesAliases[character];
        }

        bool allowExpand = SettingsConfig.IfAllowExpandings();

        if (isExecuting)
        {
            StopPrev(false);
        }

        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteByName(character);

        // Говорящий спрайт реально есть на экране или в предпрогрузке
        if (sprite_obj != null && character != Character.None)
        {
            GameSpriteObject sprite = (GameSpriteObject)sprite_obj;

            // Говорит новый спрайт, не тот что был
            if (character != lastExpandedSpriteName)
            {
                SpriteData spriteData = SpriteController.instance.GameSpriteData[sprite.num];

                // Если спрайт предпрогружен, то его ещё нет на экране
                if (!spriteData.preloaded)
                {
                    Vector3 newScale = SpriteController.instance.CharactersScales[character] * expand_coefficient;

                    SpriteController.instance.SaveSpriteData(sprite.num, true);

                    // Для синхронизации увеличения при смене спрайтов
                    int linkedSprite = spriteData.prevSprite;
                    if (linkedSprite != -1)
                    {
                        /*SpriteData linkedSpriteData = SpriteController.instance.GameSpriteData[linkedSprite];
                        if (allowExpand && !skipIsActive && !linkedSpriteData.preloaded)
                        {
                            StartCoroutine(Expand(SpriteController.instance.GameSprites[linkedSprite], newScale, expand_time));
                        }*/
                    }

                    if (allowExpand && !skipIsActive)
                    {
                        StartCoroutine(Expand(sprite, newScale, expand_time));
                    }

                    if (allowExpand && lastExpandedSpriteName != Character.None)
                    {
                        ShrinkOldSprite(lastExpandedSpriteName, skipIsActive);
                    }

                    lastExpandedSpriteName = character;
                }
            }
        }
        // Спрайта ещё нет на экране
        else
        {
            if (lastExpandedSpriteName != Character.None)
            {
                if (allowExpand)
                {
                    ShrinkOldSprite(lastExpandedSpriteName, skipIsActive);
                }

                lastExpandedSpriteName = character;
            }
        }
    }

    private void ShrinkOldSprite(Character character, bool skip)
    {
        GameSpriteObject? old_sprite = SpriteController.instance.GetSpriteByName(character);

        if (old_sprite != null)
        {
            GameSpriteObject sprite = (GameSpriteObject)old_sprite;

            SpriteController.instance.SaveSpriteData(sprite.num, false);

            /*int linkedSprite = SpriteController.instance.GameSpriteData[sprite.num].prevSprite;
            if (linkedSprite != -1)
            {
                StartCoroutine(Expand(SpriteController.instance.GameSprites[linkedSprite].ByPart(SpritePart.Body), SpriteController.instance.CharactersScales[name], expand_time));
            }*/

            Vector3 scale = SpriteController.instance.CharactersScales[character];

            StartCoroutine(Expand(sprite, scale, expand_time, skip));
        }
    }

    public IEnumerator Expand(GameSpriteObject sprite, Vector3 newScale, float smoothTime, bool skip = false)
    {
        if (skip)
        {
            sprite.SetScale(newScale);
            isExecuting = false;
            yield break;
        }

        isExecuting = true;

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
