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
}
