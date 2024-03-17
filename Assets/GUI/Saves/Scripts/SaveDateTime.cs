using Fungus;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveDateTime : MonoBehaviour
{
    private Text _text;
    private float _speed = 4f;

    private static string DateTimeShortFormat = "HH:mm dd/MM/yy";

    private void Awake()
    {
        _text = GetComponent<Text>();
    }

    public void SetText(string text)
    {
        DateTime dateTime = DateTime.ParseExact(text, SaveManager.DateTimeFormat, null);
        _text.text = dateTime.ToString(DateTimeShortFormat);
    }

    public void ClearText()
    {
        _text.text = "";
    }

    public void HideText()
    {
        GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void ShowText()
    {
        GetComponent<CanvasGroup>().alpha = 1f;
    }

    public IEnumerator IHideText()
    {
        yield return StartCoroutine(FadeManager.FadeOnly(gameObject, false, _speed));
    }

    public IEnumerator IShowText()
    {
        yield return StartCoroutine(FadeManager.FadeOnly(gameObject, true, _speed));
    }

    public IEnumerator IShowText(string text)
    {
        SetText(text);
        yield return StartCoroutine(FadeManager.FadeOnly(gameObject, true, _speed));
    }
}
