using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipButton : IExpandableButton
{
    private enum SkipButtonState
    {
        SkipOn,
        SkipOff
    }

    private GameObject ShadeTriangle;
    private GameObject ShadeArrow;
    private GameObject ShadeArrowCopy;

    private SkipButtonState state;

    private IEnumerator shade1in;
    private IEnumerator shade2in;
    private IEnumerator shade3in;
    private IEnumerator shade1out;
    private IEnumerator shade2out;
    private IEnumerator shade3out;
    private IEnumerator shrink;
    private IEnumerator expand;

    private string SkipOnA = "DoSkip";
    private string SkipOffA = "ReturnSkip";

    private float speed = 5.0f;

    public override void Awake()
    {
        base.Awake();
        ShadeTriangle = transform.GetChild(0).transform.GetChild(0).gameObject;
        ShadeArrow = transform.GetChild(1).transform.GetChild(0).gameObject;
        ShadeArrowCopy = transform.GetChild(2).transform.GetChild(0).gameObject;
        animator = GetComponent<Animator>();
        state = SkipButtonState.SkipOff;
    }

    public void Start()
    {
        GameButtonsManager.instance.SubscribeButton(this.gameObject);
    }

    public override void EnterAction()
    {
        if (shrink != null)
            StopCoroutine(shrink);
        expand = ExpandManager.ExpandObject(gameObject, expandedScale, expandTime);
        StartCoroutine(expand);

        if (shade1out != null)
        {
            StopCoroutine(shade1out);
            StopCoroutine(shade2out);
            StopCoroutine(shade3out);
        }

        shade1in = FadeManager.FadeObject(ShadeTriangle, true, speed);
        shade2in = FadeManager.FadeObject(ShadeArrow, true, speed);
        shade3in = FadeManager.FadeObject(ShadeArrowCopy, true, speed);

        StartCoroutine(shade1in);
        StartCoroutine(shade2in);
        StartCoroutine(shade3in);
    }

    public override void ExitAction()
    {
        if (expand != null)
            StopCoroutine(expand);
        shrink = ExpandManager.ExpandObject(gameObject, origScale, expandTime);
        StartCoroutine(shrink);


        if (shade1in != null)
        {
            StopCoroutine(shade1in);
            StopCoroutine(shade2in);
            StopCoroutine(shade3in);
        }

        shade1out = FadeManager.FadeObject(ShadeTriangle, false, speed);
        shade2out = FadeManager.FadeObject(ShadeArrow, false, speed);
        shade3out = FadeManager.FadeObject(ShadeArrowCopy, false, speed);

        StartCoroutine(shade1out);
        StartCoroutine(shade2out);
        StartCoroutine(shade3out);
    }

    public override IEnumerator IClick()
    {
        StopAllCoroutines();
        StartCoroutine(IClickAnimation());

        switch (state)
        {
            case SkipButtonState.SkipOff:
                EnableSkipState();
                EnableSkipAnimation();
                break;
            case SkipButtonState.SkipOn:
                if (!(Input.GetKey(KeyCode.Tab)))
                {
                    DisableSkipState();
                    DisableSkipAnimation();
                }
                break;
        }


        yield return null;
    }

    private IEnumerator IClickAnimation()
    {
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));
    }

    public void EnableSkipAnimation()
    {
        animator.SetTrigger(SkipOnA);
    }

    public void DisableSkipAnimation()
    {
        animator.SetTrigger(SkipOffA);
    }

    public void EnableSkipState()
    {
        state = SkipButtonState.SkipOn;
    }

    public void DisableSkipState()
    {
        state = SkipButtonState.SkipOff;
    }

    public bool IfButtonClicked()
    {
        return state == SkipButton.SkipButtonState.SkipOn;
    }

    public override void ResetButtonState()
    {
        gameObject.transform.localScale = origScale;
    }
}
