using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static TextBoxController;
using Fungus;

public enum bgSwapType
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

    [SerializeField]
    private GameObject storytext;

    private GameObject _backgroundsPanel;

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

        _textBoxThemes.Add("AssemblyHall", new TextBoxTheme(ThemeStyle.Light, 0.75f));
        _textBoxThemes.Add("Performance1", new TextBoxTheme(ThemeStyle.Light, 0.9f));
    }

    private void Start()
    {
        BlackPanel = PanelsConfig.CurrentManager.GetBlackPanel();
    }

    public IEnumerator IReleaseBackground()
    {
        if (bg_handler.IsValid())
        {
            yield return Addressables.ReleaseInstance(bg_handler);
        }
    }

    public IEnumerator ISwap(string bg_adress, bgSwapType type, float speed, float delay)
    {
        switch (type)
        {
            case bgSwapType.BlackFade:
                yield return StartCoroutine(IBlackFadeBackground(bg_adress, speed, delay));
                break;
            case bgSwapType.Instant:
                yield return StartCoroutine(IInstantSwapBackground(bg_adress));
                break;
            case bgSwapType.Overlay:
                yield return StartCoroutine(IOverlayBackground(bg_adress, speed));
                break;
        }
    }

    // Фон -> чёрный экран -> фон
    public IEnumerator IBlackFadeBackground(string bg_adress, float speed, float delay)
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, speed));
        TextBoxController.instance.SetStoryText("");

        // Удаление старых фонов    
        if (bg_handler.IsValid())
        {
            yield return Addressables.ReleaseInstance(bg_handler);
        }

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            SpecialEventManager.instance.IReleaseCurrentEvent(),
            SetTextBoxTheme(bg_adress),
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

        yield return SpecialEventManager.instance.IReleaseCurrentEvent();

        //yield return StartCoroutine(SetTextBoxTheme(bg_adress));

        Resources.UnloadUnusedAssets();
    }

    // Фон поверх старого фона
    public IEnumerator IOverlayBackground(string bg_adress, float speed)
    {
        AsyncOperationHandle<GameObject> old_handler = bg_handler;

        bg_handler = Addressables.InstantiateAsync(bg_adress, _backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        if (bg_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error loading " + bg_adress);
        }

        GameObject newBg = bg_handler.Result;

        newBg.GetComponent<CanvasGroup>().alpha = 0f;
        newBg.transform.SetSiblingIndex(newBg.transform.childCount - 1);

        yield return StartCoroutine(FadeManager.FadeObject(newBg, true, speed));

        if (old_handler.IsValid())
        {
            Addressables.ReleaseInstance(old_handler);
        }

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            SpecialEventManager.instance.IReleaseCurrentEvent(),
            //SetTextBoxTheme(bg_adress),
        }));

        Resources.UnloadUnusedAssets();
    }

    // Для сейвов
    public IEnumerator ILoadBackground(string bg_adress)
    {
        if (bg_handler.IsValid())
        {
            Addressables.ReleaseInstance(bg_handler);
        }

        if (bg_adress != null)
        {
            bg_handler = Addressables.InstantiateAsync(bg_adress, _backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
            yield return bg_handler;

            if (bg_handler.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Error loading " + bg_adress);
            }

            yield return StartCoroutine(SetTextBoxTheme(bg_adress));

            Resources.UnloadUnusedAssets();
        }
    }

    private IEnumerator SetTextBoxTheme(string bg)
    {
        if (_textBoxThemes.ContainsKey(bg))
        {
            TextBoxTheme theme = _textBoxThemes[bg];
            yield return StartCoroutine(TextBoxController.instance.IChangeTheme(theme.style, theme.alpha));
        }
        else
        {
            yield return StartCoroutine(TextBoxController.instance.IChangeTheme(ThemeStyle.Light, 0.8f)); // Стандартный текстбокс
        }
    }

    public GameObject GetBackgroundPanel()
    {
        return _backgroundsPanel;
    }
}
