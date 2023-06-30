using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Fungus;
using System;

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

    public IEnumerator SwapSprites(string spriteName, int pose, int emotion, Vector3 newPosition, float disappearSpeed, float appearSpeed, float moveSpeed, bool skip, bool waitForFinished)
    {
        SpriteController.instance.printData();
        SpriteMove.instance.StopSpriteMoving();
        SpriteFade.instance.StopSpritesFading();

        SpriteController.instance.SkipSpriteActions();

        int sprite1 = SpriteController.instance.GetSpriteByName(spriteName);

        bool ChangePose = SpriteController.instance.GameSpriteData[sprite1].pose != pose;

        if (ChangePose)
        {
            int sprite2 = SpriteController.instance.GetAvaliableSpriteNum(spriteName);
            //SpriteController.instance.GameSpriteData[sprite2].prevSprite = sprite1;

            SpriteController.instance.SaveSpriteData(sprite1, 0f);
            SpriteController.instance.SaveSpriteData(sprite2, spriteName, pose, emotion);
            SpriteController.instance.SaveSpriteData(sprite2, 1f);

            GameObject Current1 = SpriteController.instance.GetSprite(sprite1);
            GameObject Current2 = SpriteController.instance.GetSprite(sprite2);

            GameObject Face1_1 = Current1.transform.GetChild(0).gameObject;
            GameObject Face1_2 = Current2.transform.GetChild(0).gameObject;

            if (SpriteController.instance.GameSpriteData[sprite1].expanded)
            {
                SpriteController.instance.SaveSpriteData(sprite2, true);
                SpriteController.instance.SetNewScale(Current2, spriteName, SpriteExpand.instance.expand_coefficient);
            }
            else
            {
                SpriteController.instance.SetDefaultScale(Current2, spriteName);
            }

            Current2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            Face1_2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

            // Изменить, если добавится гарантированное смещение в зависимости от поз
            Current2.transform.localPosition = Current1.transform.localPosition;

            yield return StartCoroutine(SpriteController.instance.LoadSpriteByParts(Current2, sprite2, spriteName, pose, emotion));

            bool move = false;
            if (newPosition.x == -1) // Если 0й вектор, спрайт не двигается
            {
                SpriteController.instance.SaveSpriteData(sprite2, Current1.transform.localPosition);
            }
            else
            {
                move = true;
                SpriteController.instance.SaveSpriteData(sprite2, newPosition);
            }

            // IEnumerators
            List<IEnumerator> enumerators = new List<IEnumerator>
            {
                SpriteFade.instance.ISetFadingSprite(Current1, false, disappearSpeed, skip),
                SpriteFade.instance.ISetFadingSprite(Face1_1, false, disappearSpeed, skip),
                SpriteFade.instance.ISetFadingSprite(Current2, true, appearSpeed, skip),
                SpriteFade.instance.ISetFadingSprite(Face1_2, true, appearSpeed, skip)
            };

            if (move)
            {
                enumerators.Add(SpriteMove.instance.IMoveSprite(Current1, newPosition, moveSpeed, skip));
                enumerators.Add(SpriteMove.instance.IMoveSprite(Current2, newPosition, moveSpeed, skip));
            }

            // Actions
            List<Action> postActions = new List<Action>
            {
                 delegate { Addressables.ReleaseInstance(SpriteController.instance.GameSpriteData[sprite1].handles[0]); },
                 delegate { Addressables.ReleaseInstance(SpriteController.instance.GameSpriteData[sprite1].handles[1]); },
                 delegate { Resources.UnloadUnusedAssets(); },
                 delegate { SpriteController.instance.DelActivity(sprite1); },
                 delegate { DialogMod.denyNextDialog = false; },
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
            SpriteController.instance.SaveSpriteData(sprite1, spriteName, pose, emotion);

            GameObject Current = SpriteController.instance.GetSprite(sprite1);
            GameObject Face1 = Current.transform.GetChild(0).gameObject;
            GameObject Face2 = Current.transform.GetChild(1).gameObject;

            //Используем 2й handle как буффер
            SpriteController.instance.GameSpriteData[sprite1].handles[2] = SpriteController.instance.GameSpriteData[sprite1].handles[1];
            Face2.GetComponent<SpriteRenderer>().sprite = Face1.GetComponent<SpriteRenderer>().sprite;

            Face2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            Face1.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

            yield return StartCoroutine(SpriteController.instance.ILoadSpriteOfSpecificObject(Face1, sprite1, spriteName, pose, emotion, SpritePart.FACE1));

            bool move = false;
            if (newPosition.x != -1) // Если 0й вектор, спрайт не двигается
            {
                move = true;
                SpriteController.instance.SaveSpriteData(sprite1, newPosition);
            }

            // IEnumerators
            List<IEnumerator> enumerators = new List<IEnumerator>
            {
                SpriteFade.instance.ISetFadingSprite(Face1, true, disappearSpeed, skip),
                SpriteFade.instance.ISetFadingSprite(Face2, false, appearSpeed, skip),
            };

            if (move)
            {
                enumerators.Add(SpriteMove.instance.IMoveSprite(Current, newPosition, moveSpeed, skip));
            }

            // Actions
            List<Action> postActions = new List<Action>
            {
                 delegate { Addressables.ReleaseInstance(SpriteController.instance.GameSpriteData[sprite1].handles[2]); },
                 delegate { Resources.UnloadUnusedAssets(); },
                 delegate { DialogMod.denyNextDialog = false; },
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
        yield return CoroutineWaitForAll.instance.WaitForAll(enumerators);

        foreach (Action action in actions)
        {
            action.Invoke();
        }
    }
}
