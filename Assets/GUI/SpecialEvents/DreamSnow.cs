using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using Fungus;

public enum DreamSnowState
{
    Start,
    Launch,
    End
}

public class DreamSnow : MonoBehaviour
{
    public static DreamSnow instance = null;

    private AsyncOperationHandle<GameObject> bg_handler;

    private AsyncOperationHandle<GameObject> snow_handler;

    [SerializeField]
    private GameObject BlackPanel;

    [SerializeField]
    private GameObject backgroundsPanel;

    public DreamSnowState currentState;

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
    }
    public IEnumerator ILoadEventByState(DreamSnowState state)
    {
        switch (state)
        {
            case DreamSnowState.Start:
                bg_handler = Addressables.InstantiateAsync("SpaceportStart", backgroundsPanel.GetComponent<RectTransform>(), false, true);
                yield return bg_handler;

                snow_handler = Addressables.InstantiateAsync("Snow", backgroundsPanel.GetComponent<RectTransform>(), false, true);
                yield return bg_handler;

                break;
            case DreamSnowState.Launch:
                bg_handler = Addressables.InstantiateAsync("SpaceportLaunch", backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
                yield return bg_handler;

                snow_handler = Addressables.InstantiateAsync("Snow", backgroundsPanel.GetComponent<RectTransform>(), false, true);
                yield return bg_handler;

                break;
        }
    }


    public IEnumerator IStartDreamSnow(float speed)
    {
        UserData.instance.specialEvent = SpecialEvents.DreamSnow;
        currentState = DreamSnowState.Start;

        FadeManager.FadeObject(BlackPanel, true);

        bg_handler = Addressables.InstantiateAsync("SpaceportStart", backgroundsPanel.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        snow_handler = Addressables.InstantiateAsync("Snow", backgroundsPanel.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));
    }


    public IEnumerator IRocketLaunch(float speed)
    {
        currentState = DreamSnowState.Launch;

        AsyncOperationHandle<GameObject> old_handler = bg_handler;

        bg_handler = Addressables.InstantiateAsync("SpaceportLaunch", backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        if (old_handler.IsValid())
        {
            yield return Addressables.ReleaseInstance(old_handler);
        }

        if (!DialogMod.skipping)
        {
            GameObject result = bg_handler.Result;
            result.GetComponent<Shaker>().Shake();
        }
        
        Resources.UnloadUnusedAssets();
    }

    public IEnumerator IEndDreamSnow(string new_bg, float speed)
    {
        UserData.instance.CurrentBG = new_bg;
        UserData.instance.specialEvent = SpecialEvents.none;
        currentState = DreamSnowState.End;

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, speed));

        if (bg_handler.IsValid() && snow_handler.IsValid())
        {
            Addressables.ReleaseInstance(bg_handler);
            Addressables.ReleaseInstance(snow_handler);
        }

        yield return StartCoroutine(BackgroundManager.instance.ISwapBackground(new_bg));

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));
    }

    public IEnumerator IReleaseEvent()
    {
        if (bg_handler.IsValid() && snow_handler.IsValid())
        {
            yield return Addressables.ReleaseInstance(bg_handler);
            yield return Addressables.ReleaseInstance(snow_handler);
        }
    }
}