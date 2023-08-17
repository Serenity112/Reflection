using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TextBoxController : MonoBehaviour
{
    public static TextBoxController instance = null;

    [SerializeField]
    private GameObject TextBoxGui1;

    [SerializeField]
    private GameObject TextBoxGui2;

    [SerializeField]
    private AssetReference TextBoxLightRef;
    private AsyncOperationHandle<Sprite> _handlerLight;

    [SerializeField]
    private AssetReference TextBoxDarkRef;
    private AsyncOperationHandle<Sprite> _handlerDark;

    private AsyncOperationHandle<GameObject> _textBoxThemeHandler;

    private ThemeStyle _currentTheme = ThemeStyle.Light;

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

    public IEnumerator IChangeTheme(ThemeStyle theme, float targetAlpha)
    {
        bool skip = Typewriter.Instance.isSkipping;

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
            tempColor.a = targetAlpha;
            image.color = tempColor;
        }
    }
}
