using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class SpriteExpand : MonoBehaviour
{
    public static SpriteExpand instance = null;

    public Character CurrentCharacter = Character.None;

    public bool isExecuting = false;

    public static float ExpandCoefficient = 1.035f;

    private float expand_time = 0.1f;

    private Dictionary<Character, Dictionary<string, Character>> namesAliases = new()
    {
        { Character.Stranger, new Dictionary<string, Character>() { { "SquareDay", Character.Katya } } },
    };

    void Awake()
    {
        instance = this;
    }

    public void ResetManager()
    {
        CurrentCharacter = Character.None;
        isExecuting = false;
    }

    public void StopPrev(bool animated)
    {
        isExecuting = false;

        StopAllCoroutines();

        if (SettingsConfig.IfAllowExpandings())
        {
            SpriteController.instance.LoadSpritesExpandingInfo(animated);
        }
    }

    private void CheckAlias(ref Character character)
    {
        if (namesAliases.ContainsKey(character))
        {
            string currentBg = BackgroundManager.instance.CurrentBG;
            if (namesAliases[character].ContainsKey(currentBg))
            {
                character = namesAliases[character][currentBg];
            }
        }
    }

    public void SetExpanding(Character character, bool skipIsActive)
    {
        CheckAlias(ref character);

        bool allowExpand = SettingsConfig.IfAllowExpandings();

        if (isExecuting)
        {
            StopPrev(false);
        }

        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteByCharacter(character);

        // Говорящий спрайт реально есть на экране или в предпрогрузке
        if (sprite_obj != null && character != Character.None)
        {
            GameSpriteObject sprite = (GameSpriteObject)sprite_obj;

            // Говорит новый спрайт, не тот что был
            if (character != CurrentCharacter)
            {
                SpriteData spriteData = SpriteController.instance.CharacterSpriteData[character];

                // Если спрайт предпрогружен, то его ещё нет на экране
                if (!spriteData.Preloaded)
                {
                    float scale = SpriteScalesBase.GetCharacterScale(character) * ExpandCoefficient;
                    Vector3 newScale = new Vector3(scale, scale, scale);
                    SpriteController.instance.SaveSpriteDataExpanded(character, true);

                    if (allowExpand && !skipIsActive)
                    {
                        StartCoroutine(Expand(sprite, newScale, expand_time, false));
                    }

                    if (allowExpand && CurrentCharacter != Character.None)
                    {
                        ShrinkOldSprite(CurrentCharacter, skipIsActive);
                    }

                    CurrentCharacter = character;
                }
            }
        }
        // Спрайта ещё нет на экране
        else
        {
            if (CurrentCharacter != Character.None)
            {
                if (allowExpand)
                {
                    ShrinkOldSprite(CurrentCharacter, skipIsActive);
                }

                CurrentCharacter = character;
            }
        }
    }

    private void ShrinkOldSprite(Character character, bool skip)
    {
        GameSpriteObject? old_sprite = SpriteController.instance.GetSpriteByCharacter(character);

        if (old_sprite != null)
        {
            GameSpriteObject sprite = (GameSpriteObject)old_sprite;

            SpriteController.instance.SaveSpriteDataExpanded(character, false);

            float scale = SpriteScalesBase.GetCharacterScale(character);
            Vector3 vScale = new Vector3(scale, scale, scale);

            StartCoroutine(Expand(sprite, vScale, expand_time, skip));
        }
    }

    public IEnumerator Expand(GameSpriteObject sprite, Vector3 newScale, float smoothTime, bool skip)
    {
        if (skip)
        {
            sprite.SetScale(newScale);
            isExecuting = false;
            yield break;
        }

        isExecuting = true;

        Vector3 velocity = Vector3.zero;

        while (sprite.GetScale() != newScale)
        {
            float x1 = sprite.GetScale().magnitude;
            float x2 = newScale.magnitude;
            float diff = Math.Max(x1, x2) - Math.Min(x1, x2);

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
