using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TextBoxController : MonoBehaviour
{
    public static TextBoxController instance = null;

    [SerializeField]
    private GameObject StoryText;

    private AsyncOperationHandle<GameObject> _textBoxThemeHandler;

    public enum ThemeStyle
    {
        Light,
        Dark,
    }

    void Awake()
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

    public IEnumerator ISetDefaultTheme()
    {
        yield return StartCoroutine(IChangeTheme(ThemeStyle.Light, 0.8f));
    }

    public IEnumerator IChangeTheme(ThemeStyle theme, float alpha)
    {
        if (_textBoxThemeHandler.IsValid())
        {
            yield return Addressables.ReleaseInstance(_textBoxThemeHandler);
        }

        string assetName = theme == ThemeStyle.Light ? "TextBoxLight" : "TextBoxDark";
        _textBoxThemeHandler = Addressables.InstantiateAsync(assetName, gameObject.GetComponent<RectTransform>(), false, true);
        yield return _textBoxThemeHandler;

        if (_textBoxThemeHandler.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject result = _textBoxThemeHandler.Result;
            result.name = assetName;
            result.transform.SetSiblingIndex(0); // Чтобы был в начале списка, под текстом итд.

            Image image = result.GetComponent<Image>();
            Color tempColor = image.color;
            tempColor.a = alpha;
            image.color = tempColor;
        }
    }

    public void SetStoryText(string text)
    {
        StoryText.GetComponent<Text>().text = text;
    }
}
