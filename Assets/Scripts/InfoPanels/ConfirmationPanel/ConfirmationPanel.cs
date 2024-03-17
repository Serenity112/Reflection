using System.Collections;
using UnityEngine;

public class ConfirmationPanel : MonoBehaviour
{
    public static ConfirmationPanel instance = null;

    public static bool CONFIRM_PANEL_ACTIVE { get; set; } = false;

    private static bool ANIMATION_ENDED { get; set; } = false;

    private float _fadingSpeed = 5f;

    private IEnumerator IYes;
    private IEnumerator INo;

    private GameObject Panel;
    private Confirmation confirmation;

    private void Awake()
    {
        instance = this;

        Panel = transform.GetChild(0).gameObject;
        confirmation = Panel.GetComponent<Confirmation>();
    }

    public IEnumerator CreateConfirmationPanel(string title, IEnumerator YesAction, IEnumerator NoAction)
    {
        if (CONFIRM_PANEL_ACTIVE)
        {
            yield break;
        }

        ANIMATION_ENDED = false;
        CONFIRM_PANEL_ACTIVE = true;

        Panel.GetComponent<CanvasGroup>().alpha = 0f;
        Panel.SetActive(true);
        confirmation.ResetButtons();
        confirmation.SetTitle(title);

        IYes = YesAction;
        INo = NoAction;

        yield return StartCoroutine(FadeManager.FadeObject(Panel, true, _fadingSpeed));

        ANIMATION_ENDED = true;
    }

    public IEnumerator ClosePanel()
    {
        yield return StartCoroutine(FadeManager.FadeObject(Panel, false, _fadingSpeed));

        CONFIRM_PANEL_ACTIVE = false;
        ANIMATION_ENDED = false;
    }

    public void ChooseYes()
    {
        if (IYes != null && ANIMATION_ENDED)
        {
            StartCoroutine(IYes);
        }
    }

    public void ChooseNo()
    {
        if (INo != null && ANIMATION_ENDED)
        {
            StartCoroutine(INo);
        }
    }
}
