using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpritesSwapper : MonoBehaviour
{
    public static SpritesSwapper instance = null;

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

    private int ignoreXval = -1;

    public IEnumerator SwapSprites(string spriteName, int pose, int emotion, Vector3 newPosition, float disappearSpeed, float appearSpeed, float moveSpeed, bool skip, bool waitForFinished, bool stopPrev)
    {
        if (stopPrev)
        {
            SpriteMove.instance.StopSpriteMoving();
            SpriteFade.instance.StopSpritesFading();
            SpriteController.instance.SkipSpriteActions();
        }

        GameSpriteObject? sprite1_obj = SpriteController.instance.GetSpriteNumByName(spriteName);

        if (sprite1_obj == null)
        {
            yield break;
        }

        GameSpriteObject sprite1 = (GameSpriteObject)sprite1_obj;

        bool ChangePose = SpriteController.instance.GameSpriteData[sprite1.num].pose != pose;

        if (ChangePose)
        {
            GameSpriteObject? sprite2_obj = SpriteController.instance.GetAvaliableSprite(spriteName);

            if (sprite2_obj == null)
            {
                yield break;
            }

            GameSpriteObject sprite2 = (GameSpriteObject)sprite2_obj;

            SpriteController.instance.SaveSpriteData(sprite1.num, 0f);
            SpriteController.instance.SaveSpriteData(sprite2.num, spriteName, pose, emotion);
            SpriteController.instance.SaveSpriteData(sprite2.num, 1f);

            if (SpriteController.instance.GameSpriteData[sprite1.num].expanded)
            {
                SpriteController.instance.SaveSpriteData(sprite2.num, true);
                SpriteController.instance.SetScaleByName(sprite2, spriteName, SpriteExpand.instance.expand_coefficient);
            }
            else
            {
                SpriteController.instance.SetScaleByName(sprite2, spriteName);
            }

            sprite2.SetAlpha(0f);

            // Изменить, если добавится гарантированное смещение в зависимости от поз
            sprite2.SetScale(sprite1.GetScale());

            yield return StartCoroutine(SpriteController.instance.LoadSpriteByParts(sprite2, spriteName, pose, emotion));

            bool move = false;
            if (newPosition.x == ignoreXval)
            {
                SpriteController.instance.SaveSpriteData(sprite2.num, sprite1.GetPosition());
                sprite2.SetPosition(sprite1.GetPosition());
            }
            else
            {
                move = true;
                SpriteController.instance.SaveSpriteData(sprite2.num, newPosition);
            }

            // IEnumerators
            List<IEnumerator> enumerators = new List<IEnumerator>
            {
                SpriteFade.instance.ISetFadingSprite(sprite1.ByPart(SpritePart.Body), false, disappearSpeed, skip),
                SpriteFade.instance.ISetFadingSprite(sprite1.ByPart(SpritePart.Face1), false, disappearSpeed, skip),
                SpriteFade.instance.ISetFadingSprite(sprite2.ByPart(SpritePart.Body), true, appearSpeed, skip),
                SpriteFade.instance.ISetFadingSprite(sprite2.ByPart(SpritePart.Face1), true, appearSpeed, skip)
            };

            if (move)
            {
                enumerators.Add(SpriteMove.instance.IMoveSprite(sprite1, newPosition, moveSpeed, skip));
                enumerators.Add(SpriteMove.instance.IMoveSprite(sprite2, newPosition, moveSpeed, skip));
            }

            // Actions
            List<Action> postActions = new List<Action>
            {
                 delegate { Addressables.Release(SpriteController.instance.GameSprites[sprite1.num].Handlers[0]); },
                 delegate { Addressables.Release(SpriteController.instance.GameSprites[sprite1.num].Handlers[1]); },
                 delegate { Resources.UnloadUnusedAssets(); },
                 delegate { SpriteController.instance.DelActivity(sprite1.num); },
                 delegate { Typewriter.Instance.denyNextDialog = false; },
            };

            if (waitForFinished)
            {
                yield return StartCoroutine(IDelayedActions(enumerators, postActions));
            }
            else
            {
                StartCoroutine(IDelayedActions(enumerators, postActions));
            }

            yield return null;
        }
        else
        {
            SpriteController.instance.SaveSpriteData(sprite1.num, spriteName, pose, emotion);

            //Используем oldHandler как буффер
            AsyncOperationHandle<Sprite> oldFaceHandler = SpriteController.instance.GameSprites[sprite1.num].Handlers[1];

            sprite1.SetImage(SpritePart.Face2, sprite1.GetImage(SpritePart.Face1));
            sprite1.SetAlpha(SpritePart.Face2, 1f);
            sprite1.SetAlpha(SpritePart.Face1, 0f);
            sprite1.SetImage(SpritePart.Face1, null);

            yield return StartCoroutine(SpriteController.instance.ILoadSpriteOfSpecificObject(sprite1, SpritePart.Face1, spriteName, pose, emotion));

            bool move = false;
            if (newPosition.x != ignoreXval)
            {
                move = true;
                SpriteController.instance.SaveSpriteData(sprite1.num, newPosition);
            }

            // IEnumerators
            List<IEnumerator> enumerators = new List<IEnumerator>
            {
                SpriteFade.instance.ISetFadingSprite(sprite1.ByPart(SpritePart.Face1), true, disappearSpeed, skip),
                SpriteFade.instance.ISetFadingSprite(sprite1.ByPart(SpritePart.Face2), false, appearSpeed, skip),
            };

            if (move)
            {
                enumerators.Add(SpriteMove.instance.IMoveSprite(sprite1, newPosition, moveSpeed, skip));
            }

            // Actions
            List<Action> postActions = new List<Action>
            {
                 delegate { Addressables.ReleaseInstance(oldFaceHandler); },
                 delegate { Resources.UnloadUnusedAssets(); },
                 delegate { Typewriter.Instance.denyNextDialog = false; },
            };

            if (waitForFinished)
            {
                yield return StartCoroutine(IDelayedActions(enumerators, postActions));
            }
            else
            {
                StartCoroutine(IDelayedActions(enumerators, postActions));
            }

            yield return null;
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
