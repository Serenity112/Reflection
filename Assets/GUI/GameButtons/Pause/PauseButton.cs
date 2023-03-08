using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private GameObject buttonParent;

    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnExit;

    private Vector3 origScale;
    private Vector3 expandedScale;
    private Animator animator;

    private void Start()
    {
        buttonParent = transform.parent.gameObject;

        animator = GetComponent<Animator>();
        origScale = gameObject.GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;
    }
    private void OnMouseEnter()
    {
        if (shrinkOnExit != null)
            StopCoroutine(shrinkOnExit);

        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expandOnEnter);
    }

    private void OnMouseExit()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);

        shrinkOnExit = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrinkOnExit);
    }

    public void OnClick()
    {
        DialogMod.denyNextDialog = true;

        StartCoroutine(IOnClick());
    }
    private IEnumerator IOnClick()
    {
        gameObject.GetComponent<Button>().interactable = false;
  
        animator.Play("pauseanim");
        ButtonsManager.instance.unlinePauseButtons();

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.gameGuiPanel, false, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed * 0.5f));
        StartCoroutine(FadeManager.FadeObject(PanelsManager.instance.pausePanel, true, speed));

        Vector3 currParentScale = buttonParent.GetComponent<RectTransform>().localScale;
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, 0.9f, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, currParentScale, 0.05f));

        GetComponent<Button>().interactable = true;
    }

    public void continueGame()
    {
        StartCoroutine(IContinue());
    }

    private IEnumerator IContinue()
    {
        ButtonsManager.instance.unlinePauseButtons();

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.gameGuiPanel, true, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed * 0.5f));
        yield return StartCoroutine(FadeManager.FadeObject(PanelsManager.instance.pausePanel, false, speed));

        DialogMod.denyNextDialog = false;
    }
}