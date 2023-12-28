using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : IExpandableButton
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnExit;

    private float _speed = 5.0f;

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
            if (!StaticVariables.PAUSED && 
                !StaticVariables.OVER_UI &&
                !StaticVariables.GAME_LOADING)
            {
                StaticVariables.PAUSED = true;
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

        PauseButtonsManager.instance.PausePanel.GetComponent<CanvasGroup>().alpha = 0f;
        PauseButtonsManager.instance.PausePanel.SetActive(true);

        PauseButtonsManager.instance.EnableButtons();
        PauseButtonsManager.instance.ResetManager();
        PauseButtonsManager.instance.uIBlur.BeginBlur(_speed);

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, _speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, _speed * 0.75f),
            FadeManager.FadeObject(PauseButtonsManager.instance.PausePanel, true, _speed),
            CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
            {
                ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime),
                ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime)
            })
        }));

        GetComponent<Button>().interactable = true;
        StaticVariables.PAUSE_ANIM_ENDED = true;
        ResetButtonState();
    }

    public override void ResetButtonState()
    {
        GetComponent<Button>().interactable = true;
        gameObject.transform.localScale = origScale;
    }
}
