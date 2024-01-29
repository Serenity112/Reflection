using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteApearer : MonoBehaviour
{
    public static SpriteApearer instance = null;

    public static bool SPRITE_LOADING { get; private set; } = false;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator SpriteAppear(Character character, int pose, int emotion, Vector3 position, float fadeTime, bool skip, bool waitForFinished)
    {
        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteByCharacter(character);
        if (sprite_obj == null)
        {
            yield break;
        }
        GameSpriteObject sprite = (GameSpriteObject)sprite_obj;

        sprite.CompletePostAction();
        SpriteController.instance.CompleteAnimations(sprite.Number);
        SpriteController.instance.LoadSpriteActualData(character);

        SpriteController.instance.SaveSpriteDataPreloaded(character, false);
        SpriteController.instance.SaveSpriteData(character, pose, emotion, position, 1f, false);

        float scale = SpriteScalesBase.GetCharacterScale(character);
        sprite.SetScale(new Vector3(scale, scale, scale));
        sprite.SetAlpha(0f);
        sprite.SetPosition(position);

        SPRITE_LOADING = true;
        yield return StartCoroutine(SpriteController.instance.LoadSpriteByParts(sprite, character, pose, emotion));
        SPRITE_LOADING = false;

        List<IEnumerator> list = new List<IEnumerator>()
        {
            SpriteFade.instance.IFadeSprite(sprite.ByPart(SpritePart.Body), fadeTime, 1f, skip),
            SpriteFade.instance.IFadeSprite(sprite.ByPart(SpritePart.Face1), fadeTime, 1f, skip)
        };
        SpriteController.instance.AddAnimation(sprite.Number, list);

        if (waitForFinished || skip)
        {
            yield return StartCoroutine(WaitForAll(list));
        }
        else
        {
            StartCoroutine(WaitForAll(list));
        }
    }

    private IEnumerator WaitForAll(List<IEnumerator> coroutines)
    {
        int tally = 0;

        foreach (IEnumerator c in coroutines)
        {
            StartCoroutine(RunCoroutine(c));
        }

        while (tally > 0)
        {
            yield return null;
        }

        IEnumerator RunCoroutine(IEnumerator c)
        {
            tally++;
            yield return StartCoroutine(c);
            tally--;
        }
    }
}
