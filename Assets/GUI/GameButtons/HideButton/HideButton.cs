using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideButton : IButtonGroup
{
    private float shadesSpeed = 5f;
    private float expandTime = 0.05f;

    [SerializeField]
    private GameObject HideOverlayButton;

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

    private Vector3 parentOrigScale;
    private Vector3 parentShrinkScale;

    public override void OnAwake()
    {
        buttonParent = transform.parent.gameObject;

        animator = GetComponent<Animator>();

        origScale = gameObject.GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;

        parentOrigScale = buttonParent.GetComponent<RectTransform>().localScale;
        parentShrinkScale = parentOrigScale * 0.85f;

        CircleShade = transform.GetChild(0).transform.GetChild(0).gameObject;
        LineShade = transform.GetChild(1).transform.GetChild(0).gameObject;
    }

    public override void RegisterManager()
    {
        SetManager(GameButtonsManager.instance);
    }

    public override void EnterActioin()
    {
        if (shrinkOnEnter != null)
            StopCoroutine(shrinkOnEnter);
        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, expandTime);
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

    public override void ExitActioin()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);
        shrinkOnEnter = ExpandManager.ExpandObject(gameObject, origScale, expandTime);
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

    public override IEnumerator IClick()
    {
        Typewriter.Instance.denySkip = true;
        Typewriter.Instance.buttonAutoSkip = false;

        HideOverlayButton.SetActive(true);
        animator.Play("Hide");

        if (gui1in != null)
        {
            StopCoroutine(gui1in);
            StopCoroutine(gui2in);
        }

        gui1out = FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, shadesSpeed / 2f);
        gui2out = FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, shadesSpeed / 2f);

        StartCoroutine(gui1out);
        StartCoroutine(gui2out);

        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

        HideOverlayButton.GetComponent<Button>().interactable = true;
    }

    public void ShowHiddeUI() => StartCoroutine(IShowHiddenUI());

    IEnumerator IShowHiddenUI()
    {
        animator.Play("Unhide");

        if (gui1out != null)
        {
            StopCoroutine(gui1out);
            StopCoroutine(gui2out);
        }

        gui1in = FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, shadesSpeed / 2f);
        gui2in = FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, shadesSpeed / 2f);

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            gui1in,
            gui2in
        }));

        HideOverlayButton.GetComponent<Button>().interactable = false;
        HideOverlayButton.SetActive(false);
        Typewriter.Instance.denySkip = false;
    }
}
