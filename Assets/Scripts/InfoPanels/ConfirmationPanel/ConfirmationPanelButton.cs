using System.Collections;
using UnityEngine;

public class ConfirmationPanelButton : IDraggableButton
{
    [SerializeField]
    private float FadingSpeed;

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
        StartCoroutine(FadeManager.FadeOnly(spacing, true, FadingSpeed));
    }

    public override void ExitAction()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeOnly(spacing, false, FadingSpeed));
    }

    public override IEnumerator IClick()
    {
        yield break;
    }

    public override void ResetButtonState()
    {
        spacing.GetComponent<CanvasGroup>().alpha = 0f;
    }

    private void OnEnable()
    {
        spacing.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
