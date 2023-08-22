using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxController : MonoBehaviour
{
    public static TextBoxController instance = null;

    [SerializeField]
    private GameObject TextBoxGuiLight;

    [SerializeField]
    private GameObject TextBoxGuiDark;

    private ThemeStyle _currentTheme = ThemeStyle.Light;

    private float _speed = 5f;

    public enum ThemeStyle
    {
        Light,
        Dark,
    }

    void Awake()
    {
        instance = this;
    }

    public IEnumerator ISetDefaultTheme()
    {
        yield return StartCoroutine(IChangeTheme(ThemeStyle.Light, 0.8f));
    }

    public void ChangeTheme()
    {

    }

    public IEnumerator ClearThemes()
    {
        TextBoxGuiLight.GetComponent<CanvasGroup>().alpha = 0f;
        TextBoxGuiDark.GetComponent<CanvasGroup>().alpha = 0f;
        yield return null;
    }

    public IEnumerator IChangeTheme(ThemeStyle newTheme, float targetAlpha, bool skip = false)
    {
        if (!skip && Typewriter.Instance.isSkipping)
        {
            skip = true;
        }

        if (newTheme == _currentTheme)
        {
            GameObject mathingGui;
            if (newTheme == ThemeStyle.Light)
            {
                mathingGui = TextBoxGuiLight;
            }
            else
            {
                mathingGui = TextBoxGuiDark;
            }

            if (skip)
            {
                mathingGui.GetComponent<CanvasGroup>().alpha = targetAlpha;
                Debug.Log($"targetAlpha {targetAlpha} set");
                yield return null;
            }
            else
            {
                yield return StartCoroutine(FadeManager.FadeToTargetAlpha(mathingGui, targetAlpha, _speed));
            }
        }
        else
        {
            GameObject oldGui;
            GameObject newGui;

            if (newTheme == ThemeStyle.Light && _currentTheme == ThemeStyle.Dark)
            {
                newGui = TextBoxGuiLight;
                oldGui = TextBoxGuiDark;
            }
            else
            {
                newGui = TextBoxGuiDark;
                oldGui = TextBoxGuiLight;
            }

            newGui.GetComponent<CanvasGroup>().alpha = 0f;
            newGui.transform.SetSiblingIndex(0);

            if (skip)
            {
                oldGui.GetComponent<CanvasGroup>().alpha = 0f;
                newGui.GetComponent<CanvasGroup>().alpha = targetAlpha;
                yield return null;
            }
            else
            {
                yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
                {
                    FadeManager.FadeToTargetAlpha(oldGui, 0f, _speed),
                    FadeManager.FadeToTargetAlpha(newGui, targetAlpha, _speed),
                }));
            }
        }

        _currentTheme = newTheme;
    }
}
