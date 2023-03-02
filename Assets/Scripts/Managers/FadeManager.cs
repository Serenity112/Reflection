using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeManager : MonoBehaviour
{
    public static FadeManager instance = null;
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

    public static IEnumerator FadeObject(GameObject obj, bool fadein, float speed)
    {
        obj.SetActive(true);
        float color = obj.GetComponent<CanvasGroup>().alpha;
        //Debug.Log("color=" + color);

        if (fadein)
        {
            for (float i = color; i <= 1 + 0.05; i += speed * Time.deltaTime)
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
        }
        else
        {
            //obj.GetComponent<CanvasGroup>().alpha = 1;
            for (float i = color; i >= 0 - 0.05; i -= speed * Time.deltaTime)
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
            //obj.GetComponent<CanvasGroup>().alpha = 0;
            obj.SetActive(false);
        }
    }

    public static IEnumerator FadeOnly(GameObject obj, bool fadein, float speed)
    {
        obj.SetActive(true);
        float color = obj.GetComponent<CanvasGroup>().alpha;

        if (fadein)
        {
            for (float i = color; i <= 1 + 0.05; i += speed * Time.deltaTime)
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
        }
        else
        {
            for (float i = color; i >= 0 - 0.05; i -= speed * Time.deltaTime)
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }

        }
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
    }

    public static IEnumerator FadeTextToColor(Text text, Color newColor, float speed)
    {
        Color textColor = text.GetComponent<Text>().color;

        for (float i = 0; i <= 1.05; i += speed * Time.deltaTime)
        {
            text.color = Color.Lerp(textColor, newColor, i);
            yield return null;
        }
    }

    public static IEnumerator ColorFadeObject(GameObject obj, bool fadein, float speed)
    {
        obj.SetActive(true);
        float color = obj.GetComponent<SpriteRenderer>().color.a;

        if (fadein)
        {
            for (float i = color; i <= 1 + 0.05; i += speed * Time.deltaTime)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        else
        {
            for (float i = color; i >= 0 - 0.05; i -= speed * Time.deltaTime)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
    public static IEnumerator ColorFadeObject(GameObject obj, bool fadein)
    {
        obj.SetActive(true);

        if (fadein)
        {
            obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            yield break;
        }
        else
        {
            obj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            yield break;
        }
    }
}
