using UnityEngine;

public class ConfirmationPanelButton : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [HideInInspector]
    public GameObject spacing;

    void OnEnable()
    {
        spacing = transform.GetChild(0).gameObject;
        spacing.GetComponent<CanvasGroup>().alpha = 0f;
        spacing.SetActive(false);
    }

    private void OnMouseEnter()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeObject(spacing, true, speed));
    }

    private void OnMouseExit()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeObject(spacing, false, speed));
    }
}
