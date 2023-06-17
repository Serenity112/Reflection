using Fungus;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public enum StationScrollState
{
    Start,
    Scroll,
    End
}

public class StationScroll : MonoBehaviour, ISpecialEvent
{
    public static StationScroll instance = null;

    private AsyncOperationHandle<GameObject> bg_handler;

    private GameObject BlackPanel;

    private GameObject backgroundsPanel;

    private int currentState;

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

    private void OnEnable()
    {
        BlackPanel = PanelsConfig.CurrentManager.GetBlackPanel();
        backgroundsPanel = BackgroundManager.instance.GetBackgroundPanel();
    }

    public IEnumerator IAppearBg(float speed)
    {
        UserData.instance.CurrentBG = null;
        currentState = (int)StationScrollState.Start;

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, speed));
        yield return StartCoroutine(BackgroundManager.instance.IReleaseBackground());

        bg_handler = Addressables.InstantiateAsync("MuseumCG", backgroundsPanel.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));
    }

    public IEnumerator IScrollBg(float speed, bool skip)
    {
        UserData.instance.CurrentBG = null;

        currentState = (int)StationScrollState.Scroll;

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.gameGuiPanel, false, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed));

        bg_handler.Result.transform.GetChild(0).gameObject.GetComponent<Animator>().Play("Scroll");
        yield return new WaitForSeconds(2.1f);
        TextBoxController.instance.SetStoryText("");

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.gameGuiPanel, true, speed));
        yield return StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed));
    }

    public IEnumerator IEndScroll(string new_bg, float speed)
    {
        UserData.instance.CurrentBG = new_bg;
        SpecialEventManager.instance.currentEvent = null;
        SpecialEventManager.instance.currentEventEnum = SpecialEvent.none;

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, speed));

        yield return StartCoroutine(IReleaseEvent());
 
        Resources.UnloadUnusedAssets();

        yield return StartCoroutine(BackgroundManager.instance.ISwapBackground(new_bg));

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));
    }

    public IEnumerator IReleaseEvent()
    {
        if (bg_handler.IsValid())
        {
            yield return Addressables.ReleaseInstance(bg_handler);
        }
    }

    public int GetState()
    {
        return currentState;
    }

    public IEnumerator ILoadEventByState(int state)
    {
        currentState = state;
        switch ((StationScrollState)state)
        {
            case StationScrollState.Start:
                bg_handler = Addressables.InstantiateAsync("MuseumCG", backgroundsPanel.GetComponent<RectTransform>(), false, true);
                yield return bg_handler;
                break;

            case StationScrollState.Scroll:
                bg_handler = Addressables.InstantiateAsync("MuseumCG", backgroundsPanel.GetComponent<RectTransform>(), false, true);
                yield return bg_handler;
                bg_handler.Result.transform.GetChild(0).gameObject.GetComponent<Animator>().Play("Scrolled");
                break;
        }
    }
}
