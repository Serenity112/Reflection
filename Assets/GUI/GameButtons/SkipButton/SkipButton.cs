using System.Collections;
using UnityEngine;

public class SkipButton : IButtonGroup
{
    private enum State
    {
        DoSkip,
        ReturnSkip
    }

    private float shadesSpeed = 5f;
    private float expandTime = 0.05f;

    private Animator animator;
    private GameObject buttonParent;
    private GameObject ShadeTriangle;
    private GameObject ShadeArrow;
    private GameObject ShadeArrowCopy;

    private State state;

    private Vector3 origScale;
    private Vector3 expandedScale;

    private Vector3 parentOrigScale;
    private Vector3 parentShrinkScale;

    private IEnumerator shade1in;
    private IEnumerator shade2in;
    private IEnumerator shade3in;
    private IEnumerator shade1out;
    private IEnumerator shade2out;
    private IEnumerator shade3out;
    private IEnumerator shrink;
    private IEnumerator expand;

    public override void OnAwake()
    {
        buttonParent = transform.parent.gameObject;

        ShadeTriangle = transform.GetChild(0).transform.GetChild(0).gameObject;
        ShadeArrow = transform.GetChild(1).transform.GetChild(0).gameObject;
        ShadeArrowCopy = transform.GetChild(2).transform.GetChild(0).gameObject;

        state = State.ReturnSkip;
        animator = GetComponent<Animator>();

        origScale = GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;

        parentOrigScale = buttonParent.GetComponent<RectTransform>().localScale;
        parentShrinkScale = parentOrigScale * 0.85f;
    }

    public override void RegisterManager()
    {
        SetManager(GameButtonsManager.instance);
    }

    public override void EnterActioin()
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

        shade1in = FadeManager.FadeObject(ShadeTriangle, true, shadesSpeed);
        shade2in = FadeManager.FadeObject(ShadeArrow, true, shadesSpeed);
        shade3in = FadeManager.FadeObject(ShadeArrowCopy, true, shadesSpeed);

        StartCoroutine(shade1in);
        StartCoroutine(shade2in);
        StartCoroutine(shade3in);
    }

    public override void ExitActioin()
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

        shade1out = FadeManager.FadeObject(ShadeTriangle, false, shadesSpeed);
        shade2out = FadeManager.FadeObject(ShadeArrow, false, shadesSpeed);
        shade3out = FadeManager.FadeObject(ShadeArrowCopy, false, shadesSpeed);

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
            case State.ReturnSkip:
                Typewriter.Instance.buttonAutoSkip = true;
                EnableSkip();
                break;
            case State.DoSkip:
                Typewriter.Instance.buttonAutoSkip = false;
                DisableSkip();
                break;
        }


        yield return null;
    }

    private IEnumerator IClickAnimation()
    {
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));
    }

    public void EnableSkip()
    {
        animator.Play("DoSkip");
        state = State.DoSkip;
    }

    public void DisableSkip()
    {
        animator.Play("ReturnSkip");
        state = State.ReturnSkip;
    }
}
