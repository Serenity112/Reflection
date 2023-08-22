using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteApearer : MonoBehaviour
{
    public static SpriteApearer instance = null;

    private void Awake()
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

    public IEnumerator SpriteAppear(string characterName, int pose, int emotion, Vector3 position, float speed, bool skip, bool waitForFinished, bool stopPrev)
    {
        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteNumByName(characterName);

        if (sprite_obj == null)
        {
            yield break;
        }

        if (stopPrev)
        {
            SpriteMove.instance.StopSpriteMoving();
            SpriteFade.instance.StopSpritesFading();
            SpriteController.instance.SkipSpriteActions();
        }

        GameSpriteObject sprite = (GameSpriteObject)sprite_obj;
        SpriteController.instance.SaveSpriteDataPreloaded(sprite.num, false);

        SpriteController.instance.SaveSpriteData(sprite.num, characterName, pose, emotion, position, 1f, false);

        sprite.SetAlpha(0f);

        SpriteController.instance.SetScaleByName(sprite, characterName);

        sprite.SetPosition(position);

        yield return StartCoroutine(SpriteController.instance.LoadSpriteByParts(sprite, characterName, pose, emotion));

        List<IEnumerator> list = new List<IEnumerator>()
        {
            SpriteFade.instance.ISetFadingSprite(sprite.ByPart(SpritePart.Body), true, speed, skip),
            SpriteFade.instance.ISetFadingSprite(sprite.ByPart(SpritePart.Face1), true, speed, skip)
        };

        if (waitForFinished)
        {
            yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(list));
        }
        else
        {
            StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(list));
        }
    }
}
