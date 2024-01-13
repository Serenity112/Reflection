using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogBack : IExpandableButton
{
    private float _speed = 5f;

    private IEnumerator shrink;
    private IEnumerator expand;
    private IEnumerator keyExitListener;

    public override void Awake()
    {
        base.Awake();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        LogManager.instance.SubscribeButton(this);
        ResetButtonState();
    }

    private bool GetAllowStatus()
    {
        return (LogManager.LOG_PANEL_ACTIVE && LogManager.LOG_PANEL_ACTIVE_POST);
    }

    public override IEnumerator IClick()
    {
        if (!GetAllowStatus())
        {
            yield break;
        }

        animator.Play("CloseLog");
        StopExitListener();
        LogManager.LOG_PANEL_ACTIVE_POST = false;
        GetComponent<Button>().interactable = false;
        LogManager.instance.logOpenButton.ResetButtonState();

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeOnly(LogManager.instance.LogPanel, false, _speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, _speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, _speed),
            CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
            {
                ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime),
                ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime)
            })
        }));

        LogManager.LOG_PANEL_ACTIVE = false;
        GetComponent<Button>().interactable = true;
        LogManager.instance.LogPanel.SetActive(false);
    }

    public override void EnterAction()
    {
        if (shrink != null)
            StopCoroutine(shrink);
        expand = ExpandManager.ExpandObject(gameObject, expandedScale, 0.05f);
        StartCoroutine(expand);
    }

    public override void ExitAction()
    {
        if (expand != null)
            StopCoroutine(expand);
        shrink = ExpandManager.ExpandObject(gameObject, origScale, 0.05f);
        StartCoroutine(shrink);
    }

    private void StopExitListener()
    {
        if (keyExitListener != null)
        {
            StopCoroutine(keyExitListener);
        }
    }

    private IEnumerator IExitKey()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (GetAllowStatus())
                {
                    StartCoroutine(IClick());
                }
            }
            yield return null;
        }
    }

    public override void ResetButtonState()
    {
        StopExitListener();
        keyExitListener = IExitKey();
        StartCoroutine(keyExitListener);

        animator.Play("Idle");
        GetComponent<Button>().interactable = true;
    }
}
