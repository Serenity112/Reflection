using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoroutineTest : MonoBehaviour
{
    public GameObject img1;
    public GameObject img2;
    public GameObject img3;

    private IEnumerator enumerator;

    public void OnClick()
    {
        StartCoroutine(i1());
    }

    private IEnumerator i1()
    {
        enumerator = i0();
        StartCoroutine(enumerator);
        StopCoroutine(enumerator);
        yield return null;

        
    }

    private IEnumerator i0()
    {
        Debug.Log("1");
        StartCoroutine(i2());
        Debug.Log("2");
        yield return new WaitForSeconds(3f);
        Debug.Log("3");
        StartCoroutine(i3());
    }


    private IEnumerator i2()
    {
        yield return StartCoroutine(FadeManager.FadeObject(img1, false, 1f));
    }

    private IEnumerator i3()
    {
        yield return StartCoroutine(FadeManager.FadeObject(img2, false, 1f));
    }
}
