using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BottomPageButton : IExpandableButton
{
    private float _speed = 2.5f;
    private int _buttonNum;

    private Text _text;
    private Image _image;

    private IEnumerator numbertowhite;
    private IEnumerator numbertogray;
    private IEnumerator numbertoblank;

    private IEnumerator expand;
    private IEnumerator shrink;

    private IEnumerator _squareWhite;
    private IEnumerator _squareGray;

    private BottomPages _bottomPages;

    public override void Awake()
    {
        base.Awake();

        _image = GetComponent<Image>();
        _text = transform.GetChild(0).gameObject.GetComponent<Text>();
    }

    public void InitializeButton(int buttonNum, BottomPages bottomPages)
    {
        _buttonNum = buttonNum;
        _bottomPages = bottomPages;
        GetComponent<Button>().onClick.AddListener(delegate { OnClick(); });
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public override void EnterAction()
    {
        NumberToWhite();
        Expand();
        SquareToWhite();
    }

    public override void ExitAction()
    {
        HideNumber();
        Shrink();
        if (SaveManager.instance.currentPage != _buttonNum)
        {
            SquareToGray();
        }
    }

    // Номера
    public void HideNumber()
    {
        int curr = SaveManager.instance.currentPage;
        if (_buttonNum == curr - 1 || _buttonNum == curr + 1)
        {
            NumberToGray();
        }
        else if (_buttonNum != curr)
        {
            NumberToBlank();
        }
    }

    public void NumberToWhite()
    {
        if (numbertogray != null)
            StopCoroutine(numbertogray);
        if (numbertoblank != null)
            StopCoroutine(numbertoblank);

        numbertowhite = FadeManager.FadeTextToColor(_text, new Color(1f, 1f, 1f, 1f), _speed);
        StartCoroutine(numbertowhite);
    }

    public void NumberToGray()
    {
        if (numbertowhite != null)
            StopCoroutine(numbertowhite);
        if (numbertoblank != null)
            StopCoroutine(numbertoblank);

        numbertogray = FadeManager.FadeTextToColor(_text, new Color(0.6f, 0.6f, 0.6f, 1f), _speed);
        StartCoroutine(numbertogray);
    }

    public void NumberToBlank()
    {
        if (numbertowhite != null)
            StopCoroutine(numbertowhite);
        if (numbertogray != null)
            StopCoroutine(numbertogray);

        float r = _text.color.r;
        float g = _text.color.g;
        float b = _text.color.b;

        numbertoblank = FadeManager.FadeTextToColor(_text, new Color(r, g, b, 0f), _speed);
        StartCoroutine(numbertoblank);
    }

    // Расширение/сжатие
    public void Expand()
    {
        if (shrink != null)
            StopCoroutine(shrink);

        expand = ExpandManager.ExpandObject(gameObject, expandedScale, expandTime);
        StartCoroutine(expand);
    }

    public void Shrink()
    {
        if (expand != null)
            StopCoroutine(expand);

        shrink = ExpandManager.ExpandObject(gameObject, origScale, expandTime);
        StartCoroutine(shrink);
    }

    // Квадрат
    public void SquareToWhite()
    {
        if (_squareGray != null)
            StopCoroutine(_squareGray);

        _squareWhite = FadeManager.FadeImageToColor(_image, new Color(1f, 1f, 1f, 1f), _speed);
        StartCoroutine(_squareWhite);
    }

    public void SquareToGray()
    {
        if (_squareWhite != null)
            StopCoroutine(_squareWhite);

        _squareGray = FadeManager.FadeImageToColor(_image, new Color(0.6f, 0.6f, 0.6f, 1f), _speed);
        StartCoroutine(_squareGray);
    }

    public override IEnumerator IClick()
    {
        _bottomPages.loadPageOnClick(_buttonNum);
        yield return null;
    }

    public override void ResetButtonState()
    {
        _image.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        _text.color = new Color(0.6f, 0.6f, 0.6f, 0f);
    }
}
