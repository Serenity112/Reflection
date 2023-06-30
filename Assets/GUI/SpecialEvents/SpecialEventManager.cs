using System;
using System.Collections;
using UnityEngine;

public enum SpecialEvent
{
    none,
    SpacePort,
    StationScroll,
    TanyaCG,
    SergeyRoom
}

public class SpecialEventManager : MonoBehaviour
{
    public static SpecialEventManager instance = null;

    public ISpecialEvent currentEvent;

    public SpecialEvent currentEventEnum;

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

    public void SetEventInstance(ISpecialEvent specialEvent)
    {
        currentEvent = specialEvent;
    }

    public void SetEventEnum(SpecialEvent specialEvent)
    {
        currentEventEnum = specialEvent;
    }

    public void DeleteEvent(SpecialEvent specialEvent)
    {
        currentEventEnum = SpecialEvent.none;
        currentEvent = null;
    }

    public IEnumerator IReleaseCurrentEvent()
    {
        if (currentEventEnum == SpecialEvent.none)
        {
            yield break;
        }

        yield return StartCoroutine(currentEvent.IReleaseEvent());
        DeleteEvent(currentEventEnum);
    }

    public IEnumerator ILoadCurrentEventByState(SpecialEvent specialEvent, string data)
    {
        SetEventEnum(specialEvent);
        if (currentEventEnum == SpecialEvent.none)
        {
            yield break;
        }

        if (currentEvent != null)
        {
            yield return StartCoroutine(currentEvent.ILoadEventByData(data));
        }
        else
        {
            Debug.Log($"Event {specialEvent} was null");
        }
    }
}
