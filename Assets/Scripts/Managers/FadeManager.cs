using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance = null;

    public static IEnumerator FadeObject(GameObject obj, bool fadein, float speed)
    {
        obj.SetActive(true);
        float color = obj.GetComponent<CanvasGroup>().alpha;

        if (fadein)
        {
            for (float i = color; i <= 1; i += speed * Time.deltaTime)
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
            obj.GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            for (float i = color; i >= 0; i -= speed * Time.deltaTime)
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
            obj.GetComponent<CanvasGroup>().alpha = 0;
            obj.SetActive(false);
        }
    }

    public static IEnumerator FadeOnly(GameObject obj, bool fadein, float speed)
    {
        obj.SetActive(true);
        float color = obj.GetComponent<CanvasGroup>().alpha;

        if (fadein)
        {
            for (float i = color; i <= 1; i += (speed * Time.deltaTime))
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
            obj.GetComponent<CanvasGroup>().alpha = 1;
        }
        else
        {
            for (float i = color; i >= 0; i -= speed * Time.deltaTime)
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
            obj.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    public static IEnumerator FadeToTargetAlpha(GameObject obj, float targetAlpha, float speed)
    {
        obj.SetActive(true);

        float color = obj.GetComponent<CanvasGroup>().alpha;
        bool fadein = targetAlpha >= color ? true : false;

        if (fadein)
        {
            for (float i = color; i <= targetAlpha; i += (speed * Time.deltaTime))
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
        }
        else
        {
            for (float i = color; i >= targetAlpha; i -= speed * Time.deltaTime)
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
        }

        obj.GetComponent<CanvasGroup>().alpha = targetAlpha;
    }

    public static void FadeObject(GameObject obj, bool fadein)
    {
        if (fadein)
        {
            obj.SetActive(true);
            obj.GetComponent<CanvasGroup>().alpha = 1;

        }
        else
        {
            obj.GetComponent<CanvasGroup>().alpha = 0;
            obj.SetActive(false);
        }
    }

    public static IEnumerator FadeImageToColor(GameObject obj, Color newColor, float speed)
    {
        Color objectColor = obj.GetComponent<Image>().color;

        for (float i = 0; i <= 1.05; i += speed * Time.deltaTime)
        {
            obj.GetComponent<Image>().color = Color.Lerp(objectColor, newColor, i);
            yield return null;
        }

        obj.GetComponent<Image>().color = newColor;
    }

    public static IEnumerator FadeTextToColor(Text text, Color newColor, float speed)
    {
        Color textColor = text.GetComponent<Text>().color;

        for (float i = 0; i <= 1.05; i += speed * Time.deltaTime)
        {
            text.color = Color.Lerp(textColor, newColor, i);
            yield return null;
        }

        text.color = newColor;
    }

    public static IEnumerator ColorAlphaFadeObject(GameObject obj, bool fadein, float speed)
    {
        obj.SetActive(true);
        float alpha = obj.GetComponent<SpriteRenderer>().color.a;

        if (fadein)
        {
            for (float i = alpha; i <= 1; i += speed * Time.deltaTime)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
                yield return null;
            }
            obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            for (float i = alpha; i >= 0; i -= speed * Time.deltaTime)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
                yield return null;
            }
            obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }

    public static void ColorAlphaFadeObject(GameObject obj, bool fadein)
    {
        obj.SetActive(true);

        if (fadein)
        {
            obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
        else
        {
            obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }
}
