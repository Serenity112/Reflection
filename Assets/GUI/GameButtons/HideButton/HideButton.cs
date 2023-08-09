using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HideButton : MonoBehaviour
{
    private float shadesSpeed = 5f;

    [SerializeField]
    private GameObject HideOverlayButton;

    [SerializeField]
    private GameObject ContinueButton;

    private Animator animator;

    private IEnumerator shades1in;
    private IEnumerator shades2in;
    private IEnumerator shades1out;
    private IEnumerator shades2out;

    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnEnter;

    private IEnumerator gui1in;
    private IEnumerator gui2in;
    private IEnumerator gui1out;
    private IEnumerator gui2out;

    private GameObject buttonParent;

    private GameObject CircleShade;
    private GameObject LineShade;

    private Vector3 origScale;
    private Vector3 expandedScale;

    void Start()
    {
        buttonParent = transform.parent.gameObject;

        animator = GetComponent<Animator>();
        origScale = gameObject.GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;

        CircleShade = transform.GetChild(0).transform.GetChild(0).gameObject;
        LineShade = transform.GetChild(1).transform.GetChild(0).gameObject;
    }

    void OnMouseEnter()
    {
        if (shrinkOnEnter != null)
            StopCoroutine(shrinkOnEnter);
        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expandOnEnter);

        if (shades1out != null)
        {
            StopCoroutine(shades1out);
            StopCoroutine(shades2out);
        }

        shades1in = FadeManager.FadeObject(CircleShade, true, shadesSpeed);
        shades2in = FadeManager.FadeObject(LineShade, true, shadesSpeed);

        StartCoroutine(shades1in);
        StartCoroutine(shades2in);
    }

    void OnMouseExit()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);
        shrinkOnEnter = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrinkOnEnter);

        if (shades1in != null)
        {
            StopCoroutine(shades1in);
            StopCoroutine(shades2in);
        }

        shades1out = FadeManager.FadeObject(CircleShade, false, shadesSpeed);
        shades2out = FadeManager.FadeObject(LineShade, false, shadesSpeed);

        StartCoroutine(shades1out);
        StartCoroutine(shades2out);
    }

    public void Click()
    {
        Typewriter.Instance.denySkip = true;
        StartCoroutine(IClick());
    }

    public IEnumerator IClick()
    {
        ContinueButton.GetComponent<Button>().interactable = false;
        GetComponent<Button>().interactable = false;
        animator.Play("Hide");

        if (gui1in != null)
        {
            StopCoroutine(gui1in);
            StopCoroutine(gui2in);
        }

        gui1out = FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, shadesSpeed / 2.5f);
        gui2out = FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, shadesSpeed / 2.5f);

        StartCoroutine(gui1out);
        StartCoroutine(gui2out);

        Vector3 currParentScale = buttonParent.GetComponent<RectTransform>().localScale;
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, 0.85f, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, currParentScale, 0.05f));

        GetComponent<Button>().interactable = true;
        ContinueButton.GetComponent<Button>().interactable = true;
        HideOverlayButton.SetActive(true);
    }

    public void ShowHiddeUI()
    {
        StartCoroutine(IShowHiddenUI());
    }

    IEnumerator IShowHiddenUI()
    {
        HideOverlayButton.GetComponent<Button>().interactable = false;
        animator.Play("Unhide");

        if (gui1out != null)
        {
            StopCoroutine(gui1out);
            StopCoroutine(gui2out);
        }

        gui1in = FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, shadesSpeed / 2f);
        gui2in = FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, shadesSpeed / 2f);

        StartCoroutine(gui1in);
        yield return StartCoroutine(gui2in);

        HideOverlayButton.GetComponent<Button>().interactable = true;
        HideOverlayButton.SetActive(false);
        Typewriter.Instance.denySkip = false;
    }
}
        