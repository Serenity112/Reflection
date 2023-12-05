using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SettingsBackButton : IExpandableButton
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnEnter;

    private void Start()
    {
        animator.Play("Idle");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape) && GetComponent<Button>().interactable && StaticVariables.IN_SETTINGS_MENU)
        {
            OnClick();
        }
    }

    public override IEnumerator IClick()
    {
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
        expandOnEnter = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expandOnEnter);
    }

    public override void ExitAction()
    {
        if (expandOnEnter != null)
            StopCoroutine(expandOnEnter);
        shrinkOnEnter = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrinkOnEnter);
    }

    public override void ResetButtonState()
    {
        animator.Play("Idle");
        GetComponent<Button>().interactable = true;
    }
}
