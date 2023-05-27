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

public class StationScroll : MonoBehaviour
{
    public static StationScroll instance = null;

    private AsyncOperationHandle<GameObject> bg_handler;

    public StationScrollState currentState;

    [SerializeField]
    private GameObject BlackPanel;

    [SerializeField]
    private GameObject backgroundsPanel;

    [SerializeField]
    private GameObject ContinueButton;

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

    public IEnumerator ILoadEventByState(StationScrollState state)
    {
        switch (state)
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

    public IEnumerator IAppearBg(float speed)
    {
        UserData.instance.specialEvent = SpecialEvent.StationScroll;
        currentState = StationScrollState.Start;

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, speed));
        yield return StartCoroutine(BackgroundManager.instance.IReleaseBackground());

        bg_handler = Addressables.InstantiateAsync("MuseumCG", backgroundsPanel.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));
    }

    public IEnumerator IScrollBg(float speed, bool skip)
    {
        DialogMod.denyNextDialog = true;

        currentState = StationScrollState.Scroll;

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.gameGuiPanel, false, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed));

        bg_handler.Result.transform.GetChild(0).gameObject.GetComponent<Animator>().Play("Scroll");
        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.gameGuiPanel, true, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed));

        DialogMod.denyNextDialog = false;
    }

    public IEnumerator IEndScroll(string new_bg, float speed)
    {
        UserData.instance.CurrentBG = new_bg;
        UserData.instance.specialEvent = SpecialEvent.none;
        currentState = StationScrollState.End;

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
}
