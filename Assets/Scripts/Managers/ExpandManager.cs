using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExpandManager : MonoBehaviour
{
    public static ExpandManager instance = null;

    void Awake()
    {
        instance = this;
    }

    public static IEnumerator ExpandObject(GameObject obj, float fraction, float smoothTime)
    {
        Vector3 scale = obj.transform.localScale;
        Vector3 newScale = new Vector3(scale.x * fraction, scale.y * fraction, scale.z * fraction);

        Vector3 velocity1 = Vector3.zero;

        while (obj.transform.localScale != newScale)
        {
            if ((Math.Abs(Math.Abs(obj.transform.localScale.x) - Math.Abs(newScale.x)) < 0.001))
            {
                obj.transform.localScale = newScale;
                yield break;
            }

            obj.transform.localScale = Vector3.SmoothDamp(obj.transform.localScale, newScale, ref velocity1, smoothTime);
            yield return null;
        }
        yield break;
    }

    public static IEnumerator ExpandObject(GameObject obj, Vector3 newScale, float smoothTime)
    {
        Vector3 velocity1 = Vector3.zero;

        while (obj.transform.localScale != newScale)
        {
            if (Math.Abs(newScale.magnitude - obj.transform.localScale.magnitude) < 0.01)
            {
                obj.transform.localScale = newScale;
                yield break;
            }

            obj.transform.localScale = Vector3.SmoothDamp(obj.transform.localScale, newScale, ref velocity1, smoothTime);
            yield return null;
        }

        obj.transform.localScale = newScale;
        yield break;
    }
}
