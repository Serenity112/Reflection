using UnityEngine;

public class ConfirmationPanelButton : MonoBehaviour
{
    [SerializeField]
    private float FadingSpeed;

    private GameObject spacing;

    private void OnEnable()
    {
        spacing = transform.GetChild(0).gameObject;
        spacing.GetComponent<CanvasGroup>().alpha = 0f;
    }

    private void OnMouseEnter()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeOnly(spacing, true, FadingSpeed));
    }

    private void OnMouseExit()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeOnly(spacing, false, FadingSpeed));
    }
}
