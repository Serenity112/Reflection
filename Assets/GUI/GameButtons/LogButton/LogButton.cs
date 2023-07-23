using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogButton : MonoBehaviour
{
    [SerializeField]
    private float shadesSpeed;

    private GameObject ShadeBox;
    private GameObject ShadeLine1;
    private GameObject ShadeLine2;

    private GameObject buttonParent;

    private Animator animator;

    private Vector3 origScale;
    private Vector3 expandedScale;

    private IEnumerator shades1in;
    private IEnumerator shades2in;
    private IEnumerator shades3in;

    private IEnumerator shades1out;
    private IEnumerator shades2out;
    private IEnumerator shades3out;

    IEnumerator shrinkOnExit;
    IEnumerator expandOnEnter;


    private void Start()
    {
        buttonParent = transform.parent.gameObject;

        ShadeBox = transform.GetChild(0).transform.GetChild(0).gameObject;
        ShadeLine1 = transform.GetChild(1).transform.GetChild(0).gameObject;
        ShadeLine2 = transform.GetChild(2).transform.GetChild(0).gameObject;

        animator = GetComponent<Animator>();

        origScale = GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;
    }

    private void OnMouseEnter()
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

        shades1in = FadeManager.FadeObject(ShadeBox, true, shadesSpeed);
        shades2in = FadeManager.FadeObject(ShadeLine1, true, shadesSpeed);
        shades3in = FadeManager.FadeObject(ShadeLine2, true, shadesSpeed);

        StartCoroutine(shades1in);
        StartCoroutine(shades2in);
        StartCoroutine(shades3in);
    }

    private void OnMouseExit()
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

        shades1out = FadeManager.FadeObject(ShadeBox, false, shadesSpeed);
        shades2out = FadeManager.FadeObject(ShadeLine1, false, shadesSpeed);
        shades3out = FadeManager.FadeObject(ShadeLine2, false, shadesSpeed);

        StartCoroutine(shades1out);
        StartCoroutine(shades2out);
        StartCoroutine(shades3out);
    }

    public void Click()
    {
       Typewriter.Instance.denyNextDialog = true;
        StartCoroutine(IClick());
    }

    IEnumerator IClick()
    {
        GetComponent<Button>().interactable = false;
        animator.Play("OpenLog");

        Vector3 currParentScale = buttonParent.GetComponent<RectTransform>().localScale;


        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.ChatLog, true, shadesSpeed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, shadesSpeed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.gameGuiPanel, false, shadesSpeed));
        PanelsManager.instance.ChatLog.GetComponent<CanvasGroup>().blocksRaycasts = true;


        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, 0.85f, 0.06f));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, currParentScale, 0.06f));

        GetComponent<Button>().interactable = true;
    }
}
