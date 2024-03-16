using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBoxController : MonoBehaviour
{
    public static TextBoxController instance = null;

    [SerializeField]
    private GameObject TextBoxGuiLight;

    [SerializeField]
    private GameObject TextBoxGuiDark;

    private GameObject[] ThemesObjects;

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
        ThemesObjects = new GameObject[] { TextBoxGuiLight, TextBoxGuiDark };
    }

    public IEnumerator ClearThemes()
    {
        TextBoxGuiLight.GetComponent<CanvasGroup>().alpha = 0f;
        TextBoxGuiDark.GetComponent<CanvasGroup>().alpha = 0f;
        yield return null;
    }

    public IEnumerator IChangeTheme(ThemeStyle newTheme, float targetAlpha, bool skip = false)
    {
        if (!skip && Typewriter.Instance.SkipIsActive)
        {
            skip = true;
        }

        if (newTheme == _currentTheme)
        {
            GameObject mathingGui = ThemesObjects[(int)newTheme];

            if (skip)
            {
                mathingGui.GetComponent<CanvasGroup>().alpha = targetAlpha;
                yield return null;
            }
            else
            {
                yield return StartCoroutine(FadeManager.FadeToTargetAlpha(mathingGui, targetAlpha, _speed));
            }
        }
        else
        {
            GameObject oldGui = ThemesObjects[(int)_currentTheme];
            GameObject newGui = ThemesObjects[(int)newTheme];

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
                yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>()
                {
                    FadeManager.FadeToTargetAlpha(oldGui, 0f, _speed),
                    FadeManager.FadeToTargetAlpha(newGui, targetAlpha, _speed),
                }));
            }
        }

        _currentTheme = newTheme;
    }
}
