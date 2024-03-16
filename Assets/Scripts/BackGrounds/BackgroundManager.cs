using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static TextBoxController;

public enum BgSwapType
{
    BlackFade,
    Instant,
    Overlay,
}

public struct TextBoxTheme
{
    public TextBoxTheme(ThemeStyle style, float alpha)
    {
        this.style = style;
        this.alpha = alpha;
    }

    public ThemeStyle style;
    public float alpha;
};

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager instance = null;

    public string CurrentBG { get; set; } = null;

    [SerializeField]
    private GameObject storytext;

    private GameObject _backgroundsPanel;

    [SerializeField]
    private GameObject BlackPanel;

    private AsyncOperationHandle<GameObject> bg_handler;

    private Dictionary<string, TextBoxTheme> _textBoxThemes = new Dictionary<string, TextBoxTheme>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        _backgroundsPanel = gameObject;

        _textBoxThemes.Add("AssemblyHall", new TextBoxTheme(ThemeStyle.Light, 0.5f));
        _textBoxThemes.Add("SkverDay", new TextBoxTheme(ThemeStyle.Light, 0.75f));
    }

    public IEnumerator IReleaseBackground()
    {
        if (bg_handler.IsValid())
        {
            yield return Addressables.ReleaseInstance(bg_handler);
        }
    }

    public IEnumerator ISwap(string bg_adress, BgSwapType type, float speed, float delay)
    {
        CurrentBG = bg_adress;
        Typewriter.Instance.ResetInstantSkip();
        GameButtonsManager.instance.BlockButtonsClick = true;

        switch (type)
        {
            case BgSwapType.BlackFade:
                yield return StartCoroutine(IBlackFadeBackground(bg_adress, speed, delay));
                break;
            case BgSwapType.Instant:
                yield return StartCoroutine(IInstantSwapBackground(bg_adress));
                break;
            case BgSwapType.Overlay:
                yield return StartCoroutine(IOverlayBackground(bg_adress, speed));
                break;
        }

        GameButtonsManager.instance.BlockButtonsClick = false;
    }

    // Фон -> чёрный экран -> фон
    public IEnumerator IBlackFadeBackground(string bg_adress, float speed, float delay)
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, speed));

        Typewriter.Instance.SetText("");
        NameChanger.instance.SetName(Character.None);

        // Удаление старых фонов    
        if (bg_handler.IsValid())
        {
            yield return Addressables.ReleaseInstance(bg_handler);
        }

        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            SpecialEventManager.instance.IReleaseCurrentEvent(),
            SetTextBoxTheme(bg_adress, true),
        }));

        bg_handler = Addressables.InstantiateAsync(bg_adress, _backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        //bg_handler.Result.GetComponent<CanvasGroup>().alpha = 1f;

        yield return new WaitForSeconds(delay);

        if (bg_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error loading " + bg_adress);
        }

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));
    }

    // Фон -> резкая смена -> фон
    public IEnumerator IInstantSwapBackground(string bg_adress)
    {
        AsyncOperationHandle<GameObject> old_handler = bg_handler;

        yield return SpecialEventManager.instance.IReleaseCurrentEvent();

        bg_handler = Addressables.InstantiateAsync(bg_adress, _backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        if (bg_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error loading " + bg_adress);
        }

        if (old_handler.IsValid())
        {
            Addressables.ReleaseInstance(old_handler);
        }

        GameObject newBg = bg_handler.Result;
        newBg.GetComponent<CanvasGroup>().alpha = 1f;
        newBg.transform.SetSiblingIndex(newBg.transform.childCount - 1);

        //yield return StartCoroutine(SetTextBoxTheme(bg_adress));

        Resources.UnloadUnusedAssets();
    }

    // Фон поверх старого фона
    public IEnumerator IOverlayBackground(string bg_adress, float speed)
    {
        AsyncOperationHandle<GameObject> old_handler = bg_handler;
        yield return SpecialEventManager.instance.IReleaseCurrentEvent();

        bg_handler = Addressables.InstantiateAsync(bg_adress, _backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        if (bg_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error loading " + bg_adress);
        }

        GameObject newBg = bg_handler.Result;

        newBg.GetComponent<CanvasGroup>().alpha = 0f;
        newBg.transform.SetSiblingIndex(newBg.transform.childCount - 1);

        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            SetTextBoxTheme(bg_adress, false),
            FadeManager.FadeObject(newBg, true, speed)
        }));


        if (old_handler.IsValid())
        {
            Addressables.ReleaseInstance(old_handler);
        }

        Resources.UnloadUnusedAssets();
    }

    // Для сейв системы
    public IEnumerator ILoadBackground(string bg_adress)
    {
        Typewriter.Instance.ResetInstantSkip();

        if (bg_handler.IsValid())
        {
            Addressables.ReleaseInstance(bg_handler);
        }

        yield return StartCoroutine(SetTextBoxTheme(bg_adress, true));

        if (bg_adress != null)
        {
            bg_handler = Addressables.InstantiateAsync(bg_adress, _backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
            yield return bg_handler;

            if (bg_handler.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Error loading " + bg_adress);
            }

            Resources.UnloadUnusedAssets();
        }
    }

    private IEnumerator SetTextBoxTheme(string bg, bool skip = false)
    {
        if (_textBoxThemes.ContainsKey(bg))
        {
            TextBoxTheme theme = _textBoxThemes[bg];
            yield return StartCoroutine(TextBoxController.instance.IChangeTheme(theme.style, theme.alpha, skip));
        }
        else
        {
            yield return StartCoroutine(TextBoxController.instance.IChangeTheme(ThemeStyle.Light, 0.8f, skip)); // Стандартный текстбокс
        }
    }

    public GameObject GetBackgroundPanel()
    {
        return _backgroundsPanel;
    }
}
