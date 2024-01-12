using System.Collections;
using UnityEngine;

public class ConfirmationPanelButton : IDraggableButton
{
    private float _fadingSpeed = 5f;

    private GameObject spacing;

    public override void Awake()
    {
        base.Awake();
        spacing = transform.GetChild(0).gameObject;
        spacing.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public override void EnterAction()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeOnly(spacing, true, _fadingSpeed));
    }

    public override void ExitAction()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeOnly(spacing, false, _fadingSpeed));
    }

    public override IEnumerator IClick()
    {
        yield break;
    }

    public override void ResetButtonState()
    {
        spacing.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
