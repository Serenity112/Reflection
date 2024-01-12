using System.Collections;
using UnityEngine;

public class WarningPanelButton : IDraggableButton
{
    [HideInInspector]
    public GameObject spacing;

    public override void Awake()
    {
        base.Awake();
        spacing = transform.GetChild(0).gameObject;
    }

    public override void EnterAction()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeObject(spacing, true, 5f));
    }

    public override void ExitAction()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeObject(spacing, false, 5f));
    }

    public override IEnumerator IClick()
    {
        WarningPanel.instance.CloseWarningPanel();
        yield return null;
    }

    public override void ResetButtonState()
    {
        FadeManager.FadeObject(spacing, false);
        ResetFlags();
    }
}
