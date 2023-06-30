using System.Collections;
using UnityEngine;
using Fungus;
using System;

public enum SpacePortState
{
    Start,
    Launch,
}

public class SpacePort : MonoBehaviour, ISpecialEvent
{
    [SerializeField]
    private GameObject RocketStandPanel;

    [SerializeField]
    private GameObject RocketLaunchPanel;

    private string currentData;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        currentData = ((int)SpacePortState.Start).ToString();
        SpecialEventManager.instance.SetEventEnum(SpecialEvent.SpacePort);
        SpecialEventManager.instance.SetEventInstance(this);
    }

    public void RocketLaunch()
    {
        currentData = ((int)SpacePortState.Launch).ToString();

        RocketStandPanel.GetComponent<CanvasGroup>().alpha = 0f;
        RocketLaunchPanel.GetComponent<CanvasGroup>().alpha = 1f;

        if (!DialogMod.skipping)
        {
            RocketLaunchPanel.GetComponent<Shaker>().Shake();
        }
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

        switch ((SpacePortState)int.Parse(data))
        {
            case SpacePortState.Start:
                RocketStandPanel.GetComponent<CanvasGroup>().alpha = 1f;
                RocketLaunchPanel.GetComponent<CanvasGroup>().alpha = 0f;
                yield return null;
                break;
            case SpacePortState.Launch:
                RocketStandPanel.GetComponent<CanvasGroup>().alpha = 0f;
                RocketLaunchPanel.GetComponent<CanvasGroup>().alpha = 1f;
                yield return null;
                break;
        }
    }
}
