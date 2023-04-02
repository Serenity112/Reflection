using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class SkipButton : MonoBehaviour
{
    public static SkipButton instance = null;
    private enum State
    {
        DoSkip,
        ReturnSkip
    }

    [SerializeField]
    private float shadesSpeed;

    private Animator animator;
    private GameObject buttonParent;
    private GameObject ShadeTriangle;
    private GameObject ShadeArrow;
    private GameObject ShadeArrowCopy;
    private State state;
    private Vector3 origScale;
    private Vector3 expandedScale;

    private IEnumerator shade1in;
    private IEnumerator shade2in;
    private IEnumerator shade3in;
    private IEnumerator shade1out;
    private IEnumerator shade2out;
    private IEnumerator shade3out;
    private IEnumerator shrink;
    private IEnumerator expand;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        buttonParent = transform.parent.gameObject;
        ShadeTriangle = transform.GetChild(0).transform.GetChild(0).gameObject;
        ShadeArrow = transform.GetChild(1).transform.GetChild(0).gameObject;
        ShadeArrowCopy = transform.GetChild(2).transform.GetChild(0).gameObject;

        state = State.ReturnSkip;
        animator = GetComponent<Animator>();

        origScale = GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;
    }

    private void OnMouseEnter()
    {
        if (shrink != null)
            StopCoroutine(shrink);
        expand = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
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

    private void OnMouseExit()
    {
        if (expand != null)
            StopCoroutine(expand);
        shrink = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
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

    public void Click()
    {
        StartCoroutine(IClick());
    }

    private IEnumerator IClick()
    {
        GetComponent<Button>().interactable = false;

        switch (state)
        {
            case State.ReturnSkip:
                DialogMod.autoSkip = true;
                EnableSkip();
                break;
            case State.DoSkip:
                DialogMod.autoSkip = false;
                DisableSkip();
                break;
        }

        Vector3 currParentScale = buttonParent.GetComponent<RectTransform>().localScale;
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, 0.85f, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, currParentScale, 0.05f));

        GetComponent<Button>().interactable = true;
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
