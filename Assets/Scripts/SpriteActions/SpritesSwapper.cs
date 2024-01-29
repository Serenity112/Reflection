using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

public class SpritesSwapper : MonoBehaviour
{
    public static SpritesSwapper instance = null;

    public static bool SPRITE_LOADING { get; private set; } = false;

    private const int ignoreXval = -1;

    private Action PostAction = delegate { };

    void Awake()
    {
        instance = this;
    }

    public IEnumerator SwapSprites(
        Character character, int pose, int emotion,
        Vector3 newPosition, float disappearTime, float appearTime, float moveTime,
        bool skip, bool waitForFinished)
    {
        PostAction.Invoke();

        GameSpriteObject? sprite1_obj = SpriteController.instance.GetSpriteByCharacter(character);
        if (sprite1_obj == null)
        {
            yield break;
        }
        GameSpriteObject sprite1 = (GameSpriteObject)sprite1_obj;

        sprite1.CompletePostAction();
        SpriteController.instance.CompleteAnimations(sprite1.Number);
        SpriteController.instance.LoadSpriteActualData(character);

        bool ChangePose = SpriteController.instance.CharacterSpriteData[character].Pose != pose;
        if (ChangePose)
        {
            SpriteController.instance.ClearSpriteData(character);

            GameSpriteObject? sprite2_obj = SpriteController.instance.GetAvaliableSprite(character);
            if (sprite2_obj == null)
            {
                yield break;
            }
            GameSpriteObject sprite2 = (GameSpriteObject)sprite2_obj;

            sprite2.CompletePostAction();
            SpriteController.instance.CompleteAnimations(sprite2.Number);
            SpriteController.instance.LoadSpriteActualData(character);
            sprite2.AddPostAction(delegate
            {
                SpriteController.instance.ClearSpriteData(sprite1.Number);
            });

            SpriteController.instance.SaveSpriteData(character, pose, emotion);
            SpriteController.instance.SaveSpriteAlpha(character, 1f);

            float scaleCoef = SpriteScalesBase.GetCharacterScale(character);
            if (SpriteController.instance.CharacterSpriteData[character].Expanded)
            {
                SpriteController.instance.SaveSpriteDataExpanded(character, true);
                if (!skip && SettingsConfig.IfAllowExpandings())
                {
                    scaleCoef *= SpriteExpand.ExpandCoefficient;
                }
            }

            sprite2.SetScale(new Vector3(scaleCoef, scaleCoef, scaleCoef));
            sprite2.SetAlpha(0f);

            yield return StartCoroutine(WaitForAll(new List<IEnumerator>()
            {
                sprite1.ReleaseHandler(SpritePart.Body),
                sprite1.ReleaseHandler(SpritePart.Face1)
            }));

            SPRITE_LOADING = true;
            yield return StartCoroutine(SpriteController.instance.LoadSpriteByParts(sprite2, character, pose, emotion));
            SPRITE_LOADING = false;

            // Move
            List<IEnumerator> move_enumerators = new List<IEnumerator>();
            if (newPosition.x == ignoreXval)
            {
                SpriteController.instance.SaveSpritePosition(character, sprite1.GetPosition());
                sprite2.SetPosition(sprite1.GetPosition());
            }
            else
            {
                SpriteController.instance.SaveSpritePosition(character, newPosition);
                move_enumerators.Add(SpriteMove.instance.IMoveSprite(sprite1, newPosition, moveTime, skip));
                move_enumerators.Add(SpriteMove.instance.IMoveSprite(sprite2, newPosition, moveTime, skip));
            }
            SpriteController.instance.AddAnimation(sprite2.Number, move_enumerators);


            // Fade
            List<IEnumerator> fade_enumerators = new List<IEnumerator>
            {
                SpriteFade.instance.IFadeSprite(sprite1.ByPart(SpritePart.Body), disappearTime, 0f, skip),
                SpriteFade.instance.IFadeSprite(sprite1.ByPart(SpritePart.Face1), disappearTime, 0f, skip),
                SpriteFade.instance.IFadeSprite(sprite2.ByPart(SpritePart.Body), appearTime, 1f, skip),
                SpriteFade.instance.IFadeSprite(sprite2.ByPart(SpritePart.Face1), appearTime, 1f, skip)
            };
            SpriteController.instance.AddAnimation(sprite2.Number, fade_enumerators);

            if (waitForFinished || skip)
            {
                yield return StartCoroutine(ISwap());
            }
            else
            {
                StartCoroutine(ISwap());
            }

            IEnumerator ISwap()
            {
                yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
                {
                    WaitForAll(fade_enumerators),
                    WaitForAll(move_enumerators)
                }));
                sprite2.CompletePostAction();
            }
        }
        else
        {
            SpriteController.instance.SaveSpriteData(character, pose, emotion);

            sprite1.SetSprite(SpritePart.Face2, sprite1.GetSprite(SpritePart.Face1));
            sprite1.SetAlpha(SpritePart.Face2, 1f);
            sprite1.SetAlpha(SpritePart.Face1, 0f);
            sprite1.SetSprite(SpritePart.Face1, null);

            yield return sprite1.ReleaseHandler(SpritePart.Face1);

            SPRITE_LOADING = true;
            yield return StartCoroutine(SpriteController.instance.ILoadSpriteOfSpecificObject(sprite1, SpritePart.Face1, character, pose, emotion));
            SPRITE_LOADING = false;

            // Move
            List<IEnumerator> move_enumerators = new List<IEnumerator>();
            if (newPosition.x != ignoreXval)
            {
                SpriteController.instance.SaveSpritePosition(character, newPosition);
                move_enumerators.Add(SpriteMove.instance.IMoveSprite(sprite1, newPosition, moveTime, skip));
            }
            SpriteController.instance.AddAnimation(sprite1.Number, move_enumerators);

            // Fade
            List<IEnumerator> fade_enumerators = new List<IEnumerator>
            {
                SpriteFade.instance.IFadeSprite(sprite1.ByPart(SpritePart.Face1), disappearTime, 1f, skip),
                SpriteFade.instance.IFadeSprite(sprite1.ByPart(SpritePart.Face2), appearTime, 0f, skip),
            };
            SpriteController.instance.AddAnimation(sprite1.Number, move_enumerators);

            PostAction = delegate
            {
                Resources.UnloadUnusedAssets();
            };

            if (waitForFinished || skip)
            {
                yield return StartCoroutine(ISwap());
            }
            else
            {
                StartCoroutine(ISwap());
            }

            IEnumerator ISwap()
            {
                yield return StartCoroutine(WaitForAll(new List<IEnumerator>()
                {
                    WaitForAll(fade_enumerators),
                    WaitForAll(move_enumerators)
                }));
                //sprite2.CompletePostAction();
            }
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
