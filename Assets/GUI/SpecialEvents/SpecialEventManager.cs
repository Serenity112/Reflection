using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public enum SpecialEvent
{
    none,
    DreamSnow,
    StationScroll,
    TanyaCG,
    RoomClock,
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

    public void AddEvent(SpecialEvent specialEvent)
    {
        currentEventEnum = specialEvent;

        switch (specialEvent)
        {
            case SpecialEvent.none:
                break;
            case SpecialEvent.DreamSnow:
                gameObject.AddComponent<DreamSnow>();
                currentEvent = GetComponent<DreamSnow>();
                break;
            case SpecialEvent.StationScroll:
                gameObject.AddComponent<StationScroll>();
                currentEvent = GetComponent<StationScroll>();
                break;
        }
    }

    public void DeleteEvent(SpecialEvent specialEvent)
    {
        currentEventEnum = SpecialEvent.none;
        currentEvent = null;

        switch (specialEvent)
        {
            case SpecialEvent.none:
                break;
            case SpecialEvent.DreamSnow:
                Destroy(gameObject.GetComponent<DreamSnow>());
                break;
            case SpecialEvent.StationScroll:
                Destroy(gameObject.GetComponent<StationScroll>());
                break;
        }
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

    public IEnumerator ILoadCurrentEventByState(int state)
    {
        if (currentEventEnum == SpecialEvent.none)
        {
            yield break;
        }

        AddEvent(currentEventEnum);
        yield return StartCoroutine(currentEvent.ILoadEventByState(state));
    }
}
