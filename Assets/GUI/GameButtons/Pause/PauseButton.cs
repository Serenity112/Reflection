using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : IButtonGroup
{
    private float speed = 5f;
    private float expandTime = 0.05f;

    private GameObject buttonParent;

    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnExit;

    private Vector3 origScale;
    private Vector3 expandedScale;

    private Vector3 parentOrigScale;
    private Vector3 parentShrinkScale;

    private Animator animator;

    public override void OnAwake()
    {
        buttonParent = transform.parent.gameObject;

        animator = GetComponent<Animator>();

        origScale = gameObject.GetComponent<RectTransform>().localScale;
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
        if (shrinkOnExit != null)
            StopCoroutine(shrinkOnExit);

        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expandOnEnter);
    }

    public override void ExitActioin()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);

        shrinkOnExit = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrinkOnExit);
    }

    public override IEnumerator IClick()
    {
        gameObject.GetComponent<Button>().interactable = false;

        Typewriter.Instance.denyNextDialog = true;

        animator.Play("pauseanim");
        ButtonsManager.instance.unlinePauseButtons();

        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, false, speed));
        StartCoroutine(FadeManager.FadeOnly(PanelsManager.instance.GameButtons, false, speed * 0.5f));
        StartCoroutine(FadeManager.FadeObject(PanelsManager.instance.pausePanel, true, speed));

        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

        GetComponent<Button>().interactable = true;
    }

    public void ContinueGame() => StartCoroutine(IContinue());

    private IEnumerator IContinue()
    {
        ButtonsManager.instance.unlinePauseButtons();

        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed * 0.5f),
            FadeManager.FadeObject(PanelsManager.instance.pausePanel, false, speed)
        });

        Typewriter.Instance.denyNextDialog = false;
    }
}
