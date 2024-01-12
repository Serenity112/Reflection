using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpritesSwapper : MonoBehaviour
{
    public static SpritesSwapper instance = null;

    public static bool SPRITE_LOADING { get; private set; } = false;

    private const int ignoreXval = -1;

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
    }

    private int _codeCounter = 0;

    private List<Action> post_actions = new List<Action>();

    public void StopAllOperations()
    {
        SpriteMove.instance.StopSpriteMoving();
        SpriteFade.instance.StopSpritesFading();

        /*foreach (var pa in post_actions)
        {
            pa.Invoke();
        }
        post_actions.Clear();*/
    }

    public IEnumerator SwapSprites(Character character, int pose, int emotion, Vector3 newPosition, float disappearSpeed, float appearSpeed, float moveSpeed, bool skip, bool waitForFinished, bool stopPrev)
    {
        SPRITE_LOADING = true;

        GameSpriteObject? sprite1_obj = SpriteController.instance.GetSpriteByName(character);

        if (sprite1_obj == null)
        {
            yield break;
        }

        if (stopPrev)
        {
            SpriteMove.instance.StopSpriteMoving();
            SpriteFade.instance.StopSpritesFading();
            SpriteController.instance.LoadSpritesDataInfo();

            if (SettingsConfig.IfAllowExpandings() && !skip)
            {
                SpriteController.instance.LoadSpritesExpandingInfo();
            }
        }

        GameSpriteObject sprite1 = (GameSpriteObject)sprite1_obj;

        bool ChangePose = SpriteController.instance.GameSpriteData[sprite1.num].pose != pose;

        if (ChangePose)
        {
            GameSpriteObject? sprite2_obj = SpriteController.instance.GetAvaliableSprite(character);

            if (sprite2_obj == null)
            {
                yield break;
            }

            GameSpriteObject sprite2 = (GameSpriteObject)sprite2_obj;

            SpriteController.instance.SaveSpriteData(sprite1.num, 0f);
            SpriteController.instance.SaveSpriteData(sprite2.num, character, pose, emotion);
            SpriteController.instance.SaveSpriteData(sprite2.num, 1f);

            if (SpriteController.instance.GameSpriteData[sprite1.num].expanded)
            {
                SpriteController.instance.SaveSpriteData(sprite2.num, true);

                if (!skip && SettingsConfig.IfAllowExpandings())
                {
                    SpriteController.instance.SetScaleByName(sprite2, character, SpriteExpand.instance.expand_coefficient);
                }
            }
            else
            {
                SpriteController.instance.SetScaleByName(sprite2, character);
            }

            // SpriteController.instance.GameSpriteData[sprite2.num].prevSprite = sprite1.num;

            sprite2.SetAlpha(0f);

            // Изменить, если добавится гарантированное смещение в зависимости от поз
            sprite2.SetScale(sprite1.GetScale());

            yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
            {
                sprite1.ReleaseHandler(SpritePart.Body),
                sprite1.ReleaseHandler(SpritePart.Face1)
            }));

            yield return StartCoroutine(SpriteController.instance.LoadSpriteByParts(sprite2, character, pose, emotion));

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

            SpriteController.instance.ClearSpriteData(sprite1.num);

            // Actions
            List<Action> postActions = new List<Action>
            {
                //delegate { Resources.UnloadUnusedAssets(); },
                //delegate {  },
                //delegate { StaticVariables.SPRITE_LOADING = false; },
            };

            //post_actions.AddRange(postActions);

            if (waitForFinished || skip)
            {
                yield return StartCoroutine(IDelayedActions(enumerators, postActions));
                SPRITE_LOADING = false;
            }
            else
            {
                StartCoroutine(IDelayedActions(enumerators, postActions));
                SPRITE_LOADING = false;
            }

            yield return null;
        }
        else
        {
            SpriteController.instance.SaveSpriteData(sprite1.num, character, pose, emotion);

            AsyncOperationHandle<Sprite> Face1Handler = sprite1.GetHandler(SpritePart.Face1);

            sprite1.SetSprite(SpritePart.Face2, sprite1.GetSprite(SpritePart.Face1));
            sprite1.SetAlpha(SpritePart.Face2, 1f);
            sprite1.SetAlpha(SpritePart.Face1, 0f);
            sprite1.SetSprite(SpritePart.Face1, null);

            yield return sprite1.ReleaseHandler(SpritePart.Face1);

            yield return StartCoroutine(SpriteController.instance.ILoadSpriteOfSpecificObject(sprite1, SpritePart.Face1, character, pose, emotion));

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

                 delegate { Resources.UnloadUnusedAssets(); },
                 delegate { SPRITE_LOADING = false; },
            };


            if (waitForFinished || skip)
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
        yield return StartCoroutine(SpriteFade.instance.WaitForAll(enumerators));

        foreach (Action action in actions)
        {
            action.Invoke();
        }
    }
}
