using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineUtils : MonoBehaviour
{
    public static CoroutineUtils instance;

    private void Awake()
    {
        instance = this;
    }

    public static IEnumerator WaitForAll(List<IEnumerator> coroutines, MonoBehaviour source = null)
    {
        if (source == null)
        {
            source = instance;
        }

        int tally = 0;

        foreach (IEnumerator c in coroutines)
        {
            source.StartCoroutine(RunCoroutine(c));
        }

        while (tally > 0)
        {
            yield return null;
        }

        IEnumerator RunCoroutine(IEnumerator c)
        {
            tally++;
            yield return source.StartCoroutine(c);
            tally--;
        }
    }

    public static IEnumerator WaitForSequence(List<IEnumerator> coroutines, MonoBehaviour source = null)
    {
        if (source == null)
        {
            source = instance;
        }

        foreach (IEnumerator c in coroutines)
        {
            yield return source.StartCoroutine(c);
        }
    }

    public static IEnumerator WaitForAny(List<IEnumerator> coroutines, MonoBehaviour source = null)
    {
        if (source == null)
        {
            source = instance;
        }

        int completedCount = 0;

        foreach (var coroutine in coroutines)
        {
            source.StartCoroutine(WaitForCoroutine(coroutine, () =>
            {
                completedCount++;
            }));
        }

        while (completedCount == 0)
        {
            yield return null;
        }

        IEnumerator WaitForCoroutine(IEnumerator coroutine, Action onComplete)
        {
            yield return source.StartCoroutine(coroutine);
            onComplete?.Invoke();
        }
    }
}
