using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineWaitForAll : MonoBehaviour
{
    public static CoroutineWaitForAll instance;

    private void Start()
    {
        instance = this;
    }

    public IEnumerator WaitForAll(List<IEnumerator> coroutines)
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