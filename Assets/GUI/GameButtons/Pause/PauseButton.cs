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

        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expandOnEnter);
    }

    public override void ExitAction()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);

        shrinkOnExit = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrinkOnExit);
    }

    public override IEnumerator IClick()
    {
        gameObject.GetComponent<Button>().interactable = false;

        Typewriter.Instance.denyNextDialog = true;

        animator.Play("pauseanim");
        PauseButtonsManager.instance.unlinePauseButtons();

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed * 0.5f));
        StartCoroutine(FadeManager.FadeObject(PanelsManager.instance.pausePanel, true, speed));

        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

        GetComponent<Button>().interactable = true;
    }

    public void ContinueGame() => StartCoroutine(IContinue());

    private IEnumerator IContinue()
    {
        PauseButtonsManager.instance.unlinePauseButtons();

        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed * 0.5f),
            FadeManager.FadeObject(PanelsManager.instance.pausePanel, false, speed)
        });

        Typewriter.Instance.denyNextDialog = false;
    }
}
