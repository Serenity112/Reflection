using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCrossButton : IDraggableButton
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnExit;

    private IEnumerator CrossFadeIn;
    private IEnumerator CrossFadeOut;

    private GameObject buttonParent;

    private Vector3 origScale;
    private Vector3 expandedScale;

    private float _speed = 7f;

    public override void Awake()
    {
        base.Awake();

        buttonParent = transform.parent.gameObject;

        origScale = gameObject.GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public override void EnterAction()
    {
        if (shrinkOnExit != null)
            StopCoroutine(shrinkOnExit);
        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expandOnEnter);

        AppearCross();

        if (!SaveManagerStatic.OverlayPanelActive)
        {

        }
    }

    public override void ExitAction()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);
        shrinkOnExit = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrinkOnExit);

        DisappearCross();

        if (!SaveManagerStatic.OverlayPanelActive)
        {

        }
    }

    public void AppearCross()
    {
        if (CrossFadeOut != null)
            StopCoroutine(CrossFadeOut);

        CrossFadeIn = FadeManager.FadeOnly(gameObject, true, _speed);
        StartCoroutine(CrossFadeIn);
    }

    public void DisappearCross()
    {
        if (CrossFadeIn != null)
            StopCoroutine(CrossFadeIn);

        CrossFadeOut = FadeManager.FadeOnly(gameObject, false, _speed);
        StartCoroutine(CrossFadeOut);
    }

    public override IEnumerator IClick()
    {
       yield return StartCoroutine(IOnClickAnimation());
    }

    private IEnumerator IOnClickAnimation()
    {
        GetComponent<Button>().interactable = false;
        Vector3 currParentScale = buttonParent.GetComponent<RectTransform>().localScale;
        Vector3 ParentShrinkScale = currParentScale * 0.85f;
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, ParentShrinkScale, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, currParentScale, 0.05f));
        GetComponent<Button>().interactable = true;
    }

    public override void ResetButtonState()
    {
        GetComponent<CanvasGroup>().alpha = 0f;
        GetComponent<Button>().interactable = true;
    }
}
