using System.Collections;
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

    public IEnumerator IScrollBg(float speed, bool skip)
    {
        currentData = ((int)StationScrollState.Scroll).ToString();

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed));

        StationPanel.GetComponent<Animator>().Play("Scroll");
        yield return new WaitForSeconds(2.1f);
        Typewriter.Instance.SetText("");

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, speed));
        yield return StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed));
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
