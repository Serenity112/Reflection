using System;
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

    public IEnumerator SpriteAppear(Character character, int pose, int emotion, Vector3 position, float speed, bool skip, bool waitForFinished, bool stopPrev)
    {
        StaticVariables.SPRITE_LOADING = true;

        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteByName(character);

        if (sprite_obj == null)
        {
            yield break;
        }

        if (stopPrev)
        {
            SpriteMove.instance.StopSpriteMoving();
            SpriteFade.instance.StopSpritesFading();
            SpriteController.instance.LoadSpritesDataInfo();
        }

        GameSpriteObject sprite = (GameSpriteObject)sprite_obj;
        SpriteController.instance.SaveSpriteDataPreloaded(sprite.num, false);

        SpriteController.instance.SaveSpriteData(sprite.num, character, pose, emotion, position, 1f, false);

        sprite.SetAlpha(0f);

        SpriteController.instance.SetScaleByName(sprite, character);

        sprite.SetPosition(position);

        yield return StartCoroutine(SpriteController.instance.LoadSpriteByParts(sprite, character, pose, emotion));

        List<IEnumerator> list = new List<IEnumerator>()
        {
            SpriteFade.instance.ISetFadingSprite(sprite.ByPart(SpritePart.Body), true, 0.1f, skip),
            SpriteFade.instance.ISetFadingSprite(sprite.ByPart(SpritePart.Face1), true, 0.1f, skip)
        };

        if (waitForFinished || skip)
        {
            yield return SpriteFade.instance.StartCoroutine(SpriteFade.instance.WaitForAll(list));

            StaticVariables.SPRITE_LOADING = false;
        }
        else
        {
            /*List<Action> postActions = new List<Action>
            {
                delegate { StaticVariables.SPRITE_LOADING = false; },
            };

            StartCoroutine(IDelayedActions(list, postActions));*/
        }
    }

    private IEnumerator IDelayedActions(List<IEnumerator> enumerators, List<Action> actions)
    {
        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(enumerators));

        foreach (Action action in actions)
        {
            action.Invoke();
            yield return null;
        }
    }
}
