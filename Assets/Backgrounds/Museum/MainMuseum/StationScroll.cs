using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StationScrollState
{
    Start,
    Scroll,
}

public class StationScroll : MonoBehaviour, ISpecialEvent
{
    [SerializeField]
    private GameObject StationPanel;

    private string currentData;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        currentData = ((int)StationScrollState.Start).ToString();
        SpecialEventManager.instance.SetEvent(this, SpecialEvent.StationScroll);
    }

    public IEnumerator IScrollBg(float speed)
    {
        currentData = ((int)StationScrollState.Scroll).ToString();

        GameButtonsManager.instance.BlockButtonsClick = true;

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed));

        StationPanel.GetComponent<Animator>().Play("Scroll");
        yield return new WaitForSeconds(2.1f);
        Typewriter.Instance.SetText("");

        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed)
        }));

        GameButtonsManager.instance.BlockButtonsClick = false;
    }


    public IEnumerator IReleaseEvent()
    {
        yield return null;
    }

    public string GetData()
    {
        return currentData;
    }

    public IEnumerator ILoadEventByData(string data)
    {
        currentData = data;
        switch ((StationScrollState)int.Parse(data))
        {
            case StationScrollState.Scroll:
                StationPanel.GetComponent<Animator>().Play("Scrolled");
                break;
        }
        yield return null;
    }
}
