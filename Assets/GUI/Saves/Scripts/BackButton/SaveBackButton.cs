using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveBackButton : IExpandableButton
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnEnter;
    private IEnumerator _update;

    private void Start()
    {
        animator.Play("Idle");
    }

    private void OnEnable()
    {
        if (_update != null)
        {
            StopCoroutine(_update);
        }

        _update = IUpdate();
        StartCoroutine(_update);
    }

    private void OnDisable()
    {
        if (_update != null)
        {
            StopCoroutine(_update);
        }
    }

    private bool AllowClick()
    {
        return GetComponent<Button>().interactable &&
                StaticVariables.IN_SAVE_MENU &&
                !StaticVariables.GAME_IS_LOADING &&
                !StaticVariables.OVERLAY_ACTIVE &&
                !SaveManagerStatic.ClickBlocker &&
                !SaveManagerStatic.DelayedSaveAction;
    }

    private IEnumerator IUpdate()
    {
        while (true)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                OnClick();
            }

            yield return null;
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
        if (!AllowClick())
        {
            yield break;
        }

        GetComponent<Button>().interactable = false;

        animator.Play("Back");
        SaveManager.instance.CloseSave();

        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

        GetComponent<Button>().interactable = true;
    }


    public override void ResetButtonState()
    {
        base.ResetButtonState();

        animator.Play("Idle");
        GetComponent<Button>().interactable = true;
    }
}
