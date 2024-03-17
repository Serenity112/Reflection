using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogButton : IExpandableButton
{
    private GameObject ShadeBox;
    private GameObject ShadeLine1;
    private GameObject ShadeLine2;

    private IEnumerator shades1in;
    private IEnumerator shades2in;
    private IEnumerator shades3in;

    private IEnumerator shades1out;
    private IEnumerator shades2out;
    private IEnumerator shades3out;

    private IEnumerator shrinkOnExit;
    private IEnumerator expandOnEnter;

    private float speed = 5.0f;

    public override void Awake()
    {
        base.Awake();
        ShadeBox = transform.GetChild(0).transform.GetChild(0).gameObject;
        ShadeLine1 = transform.GetChild(1).transform.GetChild(0).gameObject;
        ShadeLine2 = transform.GetChild(2).transform.GetChild(0).gameObject;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Start()
    {
        GameButtonsManager.instance.SubscribeButton(this.gameObject.GetComponent<IExpandableButton>());
    }

    private bool GetDenyStatus()
    {
        return (
           StaticVariables.GAME_IS_LOADING ||
           StaticVariables.OVERLAY_ACTIVE ||
           GameButtonsManager.instance.BlockButtonsClick ||
           LogManager.LOG_PANEL_ACTIVE ||
           PauseButtonsManager.GAME_IS_PAUSED);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!GetDenyStatus())
            {
                StartCoroutine(IClick());
            }
        }
    }

    public override void EnterAction()
    {
        if (shrinkOnExit != null)
            StopCoroutine(shrinkOnExit);
        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, expandTime);
        StartCoroutine(expandOnEnter);

        if (shades1out != null)
        {
            StopCoroutine(shades1out);
            StopCoroutine(shades2out);
            StopCoroutine(shades3out);
        }

        shades1in = FadeManager.FadeObject(ShadeBox, true, speed);
        shades2in = FadeManager.FadeObject(ShadeLine1, true, speed);
        shades3in = FadeManager.FadeObject(ShadeLine2, true, speed);

        StartCoroutine(shades1in);
        StartCoroutine(shades2in);
        StartCoroutine(shades3in);
    }

    public override void ExitAction()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);
        shrinkOnExit = ExpandManager.ExpandObject(gameObject, origScale, expandTime);
        StartCoroutine(shrinkOnExit);


        if (shades1in != null)
        {
            StopCoroutine(shades1in);
            StopCoroutine(shades2in);
            StopCoroutine(shades3in);
        }

        shades1out = FadeManager.FadeObject(ShadeBox, false, speed);
        shades2out = FadeManager.FadeObject(ShadeLine1, false, speed);
        shades3out = FadeManager.FadeObject(ShadeLine2, false, speed);

        StartCoroutine(shades1out);
        StartCoroutine(shades2out);
        StartCoroutine(shades3out);
    }

    public override IEnumerator IClick()
    {
        if (GetDenyStatus())
        {
            yield break;
        }

        LogManager.LOG_PANEL_ACTIVE = true;
        LogManager.LOG_PANEL_ACTIVE_POST = false;
        GetComponent<Button>().interactable = false;
        animator.Play("OpenLog");

        LogManager.instance.LogPanel.SetActive(true);
        LogManager.instance.ResetManager();

        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeObject(LogManager.instance.LogPanel, true, speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, speed),
            CoroutineUtils.WaitForSequence(new List<IEnumerator>()
            {
                ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime),
                ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime)
            })
        }));

        LogManager.LOG_PANEL_ACTIVE_POST = true;
        GetComponent<Button>().interactable = true;
    }

    public override void ResetButtonState()
    {
        animator.Play("Idle");
        GetComponent<Button>().interactable = true;
        gameObject.transform.localScale = origScale;
    }
}
