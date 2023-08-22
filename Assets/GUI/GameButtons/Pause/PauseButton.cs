using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : IExpandableButtonGroup
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnExit;

    public PauseButton() : base()
    {
        OnAwakeActions(new List<Action>
        {
            delegate { animator = GetComponent<Animator>(); },
        });
    }

    public override void RegisterManager()
    {
        SetManager(GameButtonsManager.instance);
    }

    public override void EnterAction()
    {
        if (shrinkOnExit != null)
            StopCoroutine(shrinkOnExit);

        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, expandTime);
        StartCoroutine(expandOnEnter);
    }

    public override void ExitAction()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);

        shrinkOnExit = ExpandManager.ExpandObject(gameObject, origScale, expandTime);
        StartCoroutine(shrinkOnExit);
    }

    public override IEnumerator IClick()
    {
        gameObject.GetComponent<Button>().interactable = false;

        Typewriter.Instance.DenySkip();

        animator.Play("pauseanim");

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed * 0.75f));
        StartCoroutine(FadeManager.FadeObject(PanelsManager.instance.PausePanel, true, speed));

        PauseButtonsManager.instance.EnableButtons();
        PauseButtonsManager.instance.UnSelectButtons();

        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

        GetComponent<Button>().interactable = true;
    }
}
