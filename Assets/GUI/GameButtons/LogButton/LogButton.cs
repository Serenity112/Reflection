using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogButton : IExpandableButtonGroup
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

    public LogButton() : base()
    {
        OnAwakeActions(new List<Action>
        {
            delegate { ShadeBox = transform.GetChild(0).transform.GetChild(0).gameObject; },
            delegate { ShadeLine1 = transform.GetChild(1).transform.GetChild(0).gameObject; },
            delegate { ShadeLine2 = transform.GetChild(2).transform.GetChild(0).gameObject; },
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
        shrinkOnExit = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
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

    public void Click()
    {
        Typewriter.Instance.DenySkip();
        StartCoroutine(IClick());
    }

    public override IEnumerator IClick()
    {
        GetComponent<Button>().interactable = false;
        animator.Play("OpenLog");

        Vector3 currParentScale = buttonParent.GetComponent<RectTransform>().localScale;


        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.ChatLog, true, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, speed));
        PanelsManager.instance.ChatLog.GetComponent<CanvasGroup>().blocksRaycasts = true;

        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, 0.85f, 0.06f));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, currParentScale, 0.06f));

        GetComponent<Button>().interactable = true;
    }
}
