using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AboutUsBackButton : IDraggableButton
{
    private Animator animator;

    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnEnter;

    private GameObject buttonParent;

    private Vector3 origScale;
    private Vector3 expandedScale;

    void Awake()
    {
        animator = GetComponent<Animator>();

        buttonParent = transform.parent.gameObject;

        origScale = gameObject.GetComponent<RectTransform>().localScale;
        expandedScale = origScale * 1.1f;
    }

    public override void EnterActioin()
    {
        if (shrinkOnEnter != null)
            StopCoroutine(shrinkOnEnter);
        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expandOnEnter);
    }

    public override void ExitActioin()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);
        shrinkOnEnter = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrinkOnEnter);
    }

    public override IEnumerator IClick()
    {
        GetComponent<Button>().interactable = false;

        animator.Play("Back");
        MMPanelsManager.instance.CloseInfoMenu();

        Vector3 currParentScale = buttonParent.GetComponent<RectTransform>().localScale;
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, 0.85f, 0.05f));

        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, currParentScale, 0.05f));

        GetComponent<Button>().interactable = true;
    }
}
