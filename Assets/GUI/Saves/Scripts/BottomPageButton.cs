using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BottomPageButton : MonoBehaviour
{
    public float speed;
    public int buttonnumber;

    GameObject pages;
    GameObject number;
    Text text;

    IEnumerator numbertowhite;
    IEnumerator numbertogray;
    IEnumerator numbertoblank;

    IEnumerator expand;
    IEnumerator shrink;

    IEnumerator active;
    IEnumerator notactive;

    private void Awake()
    {
        pages = transform.parent.gameObject;
        number = transform.GetChild(0).gameObject;
        text = number.GetComponent<Text>();

        string toBeSearched = "Page";
        buttonnumber = Int32.Parse(name.Substring(name.IndexOf(toBeSearched) + toBeSearched.Length));
    }
    private void OnMouseEnter()
    {
        NumberToWhite();
        Expand();
        SquareAppear();
    }

    private void OnMouseExit()
    {
        ClearNumber();
        Shrink();
        if(SaveManager.instance.currentPage != buttonnumber)
            SquareHide();
    }

    public void ClearNumber()
    {
        int curr = SaveManager.instance.currentPage;
        if (buttonnumber == curr - 1 
            || buttonnumber == curr + 1)
        {
           NumberToGray();
        } else if (buttonnumber != curr)
        {
           NumberToBlank();
        }
    }
    public void Expand()
    {
        if(shrink != null)
            StopCoroutine(shrink);

        expand = ExpandManager.ExpandObject(gameObject, 1.15f, 0.05f);
        StartCoroutine(expand);
    }

    public void Shrink()
    {
        if (expand != null)
            StopCoroutine(expand);

        shrink = ExpandManager.ExpandObject(gameObject, new Vector3(1f, 1f, 1f), 0.05f);
        StartCoroutine(shrink);
    }

    public void SquareAppear()
    {
        if (notactive != null)
            StopCoroutine(notactive);

        active = FadeManager.FadeImageToColor(gameObject, new Color(1f, 1f, 1f, 1.05f), speed);
        StartCoroutine(active);
    }

    public void SquareHide()
    {
        if (active != null)
            StopCoroutine(active);

        notactive = FadeManager.FadeImageToColor(gameObject, new Color(0.6f, 0.6f, 0.6f, 1.05f), speed); 
        StartCoroutine(notactive);
    }
    public void NumberToWhite()
    {
        if(numbertogray != null)
            StopCoroutine(numbertogray);
        if (numbertoblank != null)
            StopCoroutine(numbertoblank);

        numbertowhite = FadeManager.FadeTextToColor(text, new Color(1f, 1f, 1f, 1.05f), speed);
        StartCoroutine(numbertowhite);
    }

    public void NumberToGray()
    {
        if (numbertowhite != null)
            StopCoroutine(numbertowhite);
        if (numbertoblank != null)
            StopCoroutine(numbertoblank);

        numbertogray = FadeManager.FadeTextToColor(text, new Color(0.6f, 0.6f, 0.6f, 1.05f), speed);
        StartCoroutine(numbertogray);
    }

    public void NumberToBlank()
    {
        if (numbertowhite != null)
            StopCoroutine(numbertowhite);
        if (numbertogray != null)
            StopCoroutine(numbertogray);

        float r = text.color.r;
        float g = text.color.g;
        float b = text.color.b;

        numbertoblank = FadeManager.FadeTextToColor(text, new Color(r, g, b, -0.05f), speed);
        StartCoroutine(numbertoblank);
    }
}
