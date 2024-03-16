using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsBackButton : IExpandableButton
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnEnter;
    private IEnumerator iupdate;

    private bool AllowClick()
    {
        return (GetComponent<Button>().interactable && StaticVariables.IN_SETTINGS_MENU);
    }

    private void Start()
    {
        animator.Play("Idle");
    }

    private void OnEnable()
    {
        if (iupdate != null)
        {
            StopCoroutine(iupdate);
        }

        iupdate = IUpdate();
        StartCoroutine(iupdate);
    }

    private void OnDisable()
    {
        if (iupdate != null)
        {
            StopCoroutine(iupdate);
        }
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

    public override IEnumerator IClick()
    {
        if (!AllowClick())
        {
            yield break;
        }

        GetComponent<Button>().interactable = false;

        animator.Play("Back");
        SettingsConfig.currentManager.CloseSettings();

        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

        GetComponent<Button>().interactable = true;
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

    public override void ResetButtonState()
    {
        base.ResetButtonState();
        animator.Play("Idle");
        GetComponent<Button>().interactable = true;
    }
}
