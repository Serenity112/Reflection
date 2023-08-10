using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideButton : IExpandableButtonGroup
{
    [SerializeField]
    private GameObject HideOverlayButton;

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

    private GameObject CircleShade;
    private GameObject LineShade;

    public HideButton() : base()
    {
        OnAwakeActions(new List<Action>
        {  
            delegate { CircleShade = transform.GetChild(0).transform.GetChild(0).gameObject; },
            delegate { LineShade = transform.GetChild(1).transform.GetChild(0).gameObject; },
        });
    }

    public override void RegisterManager()
    {
        SetManager(GameButtonsManager.instance);
    }

    public override void EnterAction()
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

        shades1in = FadeManager.FadeObject(CircleShade, true, speed);
        shades2in = FadeManager.FadeObject(LineShade, true, speed);

        StartCoroutine(shades1in);
        StartCoroutine(shades2in);
    }

    public override void ExitAction()
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

        shades1out = FadeManager.FadeObject(CircleShade, false, speed);
        shades2out = FadeManager.FadeObject(LineShade, false, speed);

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

        gui1out = FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, speed / 2f);
        gui2out = FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed / 2f);

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

        gui1in = FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, speed / 2f);
        gui2in = FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed / 2f);

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
