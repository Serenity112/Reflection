using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class SpriteFade : MonoBehaviour
{
    public static SpriteFade instance = null;

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

    public void StopSpritesFading()
    {
        StopAllCoroutines();
    }

    public void SetFadingSprite(GameObject obj, bool fadein, float speed, bool skip)
    {
        StartCoroutine(ISetFadingSprite(obj, fadein, speed, skip));
    }

    public IEnumerator ISetFadingSprite(GameObject obj, bool fadein, float speed, bool skip)
    {
        if (skip)
        {
            FadeManager.ColorAlphaFadeObject(obj, fadein);
            yield return null;
        }
        else
        {
            yield return StartCoroutine(FadeManager.ColorAlphaFadeObject(obj, fadein, speed));
        }
    }

    public IEnumerator IAppearFullSprite(GameObject Body, float speed, bool skip)
    {
        GameObject Face1 = Body.transform.GetChild(0).gameObject;

        if (skip)
        {
            FadeManager.ColorAlphaFadeObject(Body, true);
            FadeManager.ColorAlphaFadeObject(Face1, true);
            yield return null;
        }
        else
        {
            StartCoroutine(FadeManager.ColorAlphaFadeObject(Body, true, speed));
            yield return StartCoroutine(FadeManager.ColorAlphaFadeObject(Face1, true, speed));
        }
    }
}
