using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBack : MonoBehaviour
{
    public float speed;

    public Animator logButtonAnimator;

    private Animator animator;

    private GameObject Arrow;

    private Vector3 origScale;

    private Vector3 expandedScale;

    IEnumerator shrink;

    IEnumerator expand;

    private void OnMouseEnter()
    {
        if (shrink != null)
            StopCoroutine(shrink);
        expand = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expand);
    }

    private void OnMouseExit()
    {
        if (expand != null)
            StopCoroutine(expand);
        shrink = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrink);
    }

    private void Start()
    {
        origScale = GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;

        animator = GetComponent<Animator>();
        Arrow = transform.GetChild(0).gameObject;
    }
    public void Click()
    {
        StartCoroutine(IClick());
    }

    IEnumerator IClick()
    {
        GetComponent<Button>().interactable = false;

        animator.Play("CloseLog");
        logButtonAnimator.Play("CloseLog");

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.ChatLog, false, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.gameGuiPanel, true, speed));
        PanelsManager.instance.ChatLog.GetComponent<CanvasGroup>().blocksRaycasts = false;


        Vector3 currArrowScale = gameObject.GetComponent<RectTransform>().localScale;
        yield return StartCoroutine(ExpandManager.ExpandObject(Arrow, 0.85f, 0.06f));
        yield return StartCoroutine(ExpandManager.ExpandObject(Arrow, currArrowScale, 0.06f));

        GetComponent<Button>().interactable = true;
       Typewriter.Instance.denyNextDialog = false;
    }
}
