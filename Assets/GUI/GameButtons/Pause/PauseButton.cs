using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : IExpandableButton
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnExit;

    private float speed = 5.0f;

    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        GameButtonsManager.instance.SubscribeButton(this.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!StaticVariables.PAUSED && !StaticVariables.OVER_UI)
            {
                StartCoroutine(IClickAnimation());
            }
        }
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

        yield return StartCoroutine(IClickAnimation());
    }

    private IEnumerator IClickAnimation()
    {
        animator.Play("pauseanim");
        gameObject.GetComponent<Button>().interactable = false;
        StaticVariables.PAUSED = true;

        PanelsManager.instance.PausePanel.SetActive(true);
        PauseButtonsManager.instance.EnableButtons();
        PauseButtonsManager.instance.ResetManager();
        PauseButtonsManager.instance.uIBlur.BeginBlur(speed);

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed * 0.75f),
            FadeManager.FadeOnly(PanelsManager.instance.PausePanel, true, speed),
            CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
            {
                ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime),
                ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime)
            })
        }));

        GetComponent<Button>().interactable = true;
        StaticVariables.PAUSED_ALLOW_BUTTON = true;
        ResetButtonState();
    }

    public override void ResetButtonState()
    {
        gameObject.transform.localScale = origScale;
    }
}
