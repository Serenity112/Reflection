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

    public ISpecialEvent CurrentEventObject { get; set; } = null;

    public SpecialEvent CurrentEventEnum { get; set; } = SpecialEvent.none;

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

    public void SetEvent(ISpecialEvent eventObj, SpecialEvent evenEnum)
    {
        CurrentEventObject = eventObj;
        CurrentEventEnum = evenEnum;
    }

    public void DeleteEvent()
    {
        CurrentEventEnum = SpecialEvent.none;
        CurrentEventObject = null;
    }

    public IEnumerator IReleaseCurrentEvent()
    {
        if (CurrentEventEnum == SpecialEvent.none)
        {
            yield break;
        }

        yield return StartCoroutine(CurrentEventObject.IReleaseEvent());
        DeleteEvent();
    }

    public IEnumerator ILoadCurrentEventByState(SpecialEvent specialEvent, string data)
    {
        //SetEventEnum(specialEvent);

        if (CurrentEventEnum == SpecialEvent.none)
        {
            yield break;
        }

        //Debug.Log("if (currentEvent != null)");
        if (CurrentEventObject != null)
        {
            yield return StartCoroutine(CurrentEventObject.ILoadEventByData(data));
        }
        else
        {
            Debug.Log($"Event {specialEvent} was null");
        }
    }
}
