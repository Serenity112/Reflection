using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AboutUsBackButton : IExpandableButton
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnEnter;
    private IEnumerator iupdate;

    private bool AllowClick()
    {
        return GetComponent<Button>().interactable && AboutUsButtonManager.IN_ABOUTUS_MENU;
    }

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        AboutUsButtonManager.instance.SubscribeButton(this);
    }

    private void OnEnable()
    {
        StartUpdate();
    }

    private void OnDisable()
    {
        EndUpdate();
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

        animator.Play("AO_Back");

        MMPanelsManager.instance.CloseInfoMenu();

        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
        yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

        GetComponent<Button>().interactable = true;
    }

    public override void ResetButtonState()
    {
        base.ResetButtonState();

        animator.Play("AO_Idle");
        GetComponent<Button>().interactable = true;
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

    public void StartUpdate()
    {
        if (iupdate != null)
        {
            StopCoroutine(iupdate);
        }

        iupdate = IUpdate();
        StartCoroutine(iupdate);
    }

    public void EndUpdate()
    {
        if (iupdate != null)
        {
            StopCoroutine(iupdate);
        }
    }
}
