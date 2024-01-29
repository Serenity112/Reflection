using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFade : MonoBehaviour
{
    public static SpriteFade instance = null;

    void Awake()
    {
        instance = this;
    }

    public void StopSpritesFading()
    {
        StopAllCoroutines();
    }

    private const float SkipStep = 10f;
    private const float DefaultStep = 1f;

    public IEnumerator IFadeSprite(GameObject sprite, float fadeTime, float target_alpha, bool skip)
    {
        if (skip)
        {
            sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, target_alpha);
            yield break;
        }

        float currentTime = 0;
        float cur_a = sprite.GetComponent<SpriteRenderer>().color.a;
        while (currentTime < fadeTime)
        {
            float step = DefaultStep;
            if (Typewriter.Instance.SkipIsActive)
            {
                step = SkipStep;
            }

            currentTime += Time.deltaTime * step;
            float new_a = Mathf.Lerp(cur_a, target_alpha, (currentTime / fadeTime));
            sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, new_a);
            yield return null;
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
