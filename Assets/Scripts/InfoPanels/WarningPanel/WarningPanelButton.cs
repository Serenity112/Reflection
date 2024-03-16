using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WarningPanelButton : IDraggableButton
{
    private GameObject spacing;
    private float _speed = 5f;

    public override void Awake()
    {
        base.Awake();
        spacing = transform.GetChild(0).gameObject;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public override void EnterAction()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeObject(spacing, true, _speed));
    }

    public override void ExitAction()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeObject(spacing, false, _speed));
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
