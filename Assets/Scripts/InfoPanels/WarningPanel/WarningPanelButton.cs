using UnityEngine;

public class WarningPanelButton : MonoBehaviour
{
    [HideInInspector]
    public GameObject spacing;

    void Start()
    {
        spacing = transform.GetChild(0).gameObject;
    }

    public void CloseWarningPanel()
    {
        WarningPanel.instance.CloseWarningPanel();
    }

    private void OnMouseEnter()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeObject(spacing, true, 5f));
    }

    private void OnMouseExit()
    {
        StopAllCoroutines();
        StartCoroutine(FadeManager.FadeObject(spacing, false, 5f));
    }
}
