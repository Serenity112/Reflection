using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveBackButton : IExpandableButton
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnEnter;

    private void Start()
    {
        animator.Play("Idle");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape) &&
            GetComponent<Button>().interactable &&
            StaticVariables.IN_SAVE_MENU &&
            !StaticVariables.GAME_IS_LOADING &&
            !SaveManagerStatic.OverlayPanelActive &&
            !StaticVariables.OVERLAY_ACTIVE)
        {
            OnClick();
        }
    }

    public override void EnterAction()
    {
        if (shrinkOnEnter != null)
            StopCoroutine(shrinkOnEnter);
        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, expandTime);
        StartCoroutine(expandOnEnter);
    }

    public override void ExitAction()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);
        shrinkOnEnter = ExpandManager.ExpandObject(gameObject, origScale, expandTime);
        StartCoroutine(shrinkOnEnter);
    }

    public override IEnumerator IClick()
    {
        GetComponent<Button>().interactable = false;

        animator.Play("Back");
        SaveManager.instance.CloseSave();

        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

        GetComponent<Button>().interactable = true;
    }

    public override void ResetButtonState()
    {
        animator.Play("Idle");
        GetComponent<Button>().interactable = true;
    }
}
