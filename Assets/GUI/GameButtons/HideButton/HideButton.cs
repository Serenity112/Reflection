using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideButton : IExpandableButton
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

    private IEnumerator waitForInput;

    private GameObject CircleShade;
    private GameObject LineShade;

    private float speed = 5.0f;

    private string HideA = "Hide";
    private string UnHideA = "Unhide";

    public static bool UI_HIDDEN = false;

    public override void Awake()
    {
        base.Awake();
        CircleShade = transform.GetChild(0).transform.GetChild(0).gameObject;
        LineShade = transform.GetChild(1).transform.GetChild(0).gameObject;
    }

    public void Start()
    {
        GameButtonsManager.instance.SubscribeButton(this.gameObject.GetComponent<IExpandableButton>());
    }

    private bool GetDenyStatus()
    {
        return (PauseButtonsManager.GAME_IS_PAUSED ||
                StaticVariables.OVERLAY_ACTIVE ||
                StaticVariables.GAME_IS_LOADING ||
                GameButtonsManager.instance.BlockButtonsClick ||
                UI_HIDDEN ||
                ChoiceManager.CHOICE_IS_ACTIVE);
    }

    public override void EnterAction()
    {
        if (shrinkOnEnter != null)
            StopCoroutine(shrinkOnEnter);
        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, expandTime);
        StartCoroutine(expandOnEnter);

        if (shades1out != null && shades2out != null)
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

        if (shades1in != null && shades2in != null)
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
        yield return null;

        if (GetDenyStatus())
        {
            yield break;
        }

        UI_HIDDEN = true;

        HideOverlayButton.SetActive(true);
        animator.Play(HideA);

        if (gui1in != null && gui2in != null)
        {
            StopCoroutine(gui1in);
            StopCoroutine(gui2in);
        }

        gui1out = FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, speed / 2f);
        gui2out = FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed / 2f);

        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            CoroutineUtils.WaitForSequence(new List<IEnumerator>()
            {
                ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime),
                ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime)
            }),
            gui1out,
            gui2out
        }));

        HideOverlayButton.GetComponent<Button>().interactable = true;

        StopInputWait();
        waitForInput = IWaitForInput();
        StartCoroutine(waitForInput);
    }

    private void StopInputWait()
    {
        if (waitForInput != null)
        {
            StopCoroutine(waitForInput);
        }
    }

    private IEnumerator IWaitForInput()
    {
        bool inputFlag = false;

        while (!inputFlag)
        {
            if (Input.GetKey(KeyCode.Space) ||
                Input.GetKey(KeyCode.Return) ||
                Input.GetKey(KeyCode.Tab) ||
                Input.GetKey(KeyCode.Escape))
            {
                inputFlag = true;
            }

            yield return null;
        }

        ShowHiddenUI();
    }

    public void ShowHiddenUI()
    {
        StopInputWait();
        HideOverlayButton.GetComponent<Button>().interactable = false;
        StartCoroutine(IShowHiddenUI());
    }

    private IEnumerator IShowHiddenUI()
    {
        animator.Play(UnHideA);

        if (gui1out != null && gui2out != null)
        {
            StopCoroutine(gui1out);
            StopCoroutine(gui2out);
        }

        gui1in = FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, speed / 2f);
        gui2in = FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed / 2f);

        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            gui1in,
            gui2in
        }));

        HideOverlayButton.SetActive(false);
        UI_HIDDEN = false;
    }

    public override void ResetButtonState()
    {
        base.ResetButtonState();

        StopInputWait();
        HideOverlayButton.SetActive(false);
    }
}
