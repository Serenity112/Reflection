using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PauseButtonsManager;

public class PauseOptionButton : IButtonGroup
{
    private float speed = 5f;

    [SerializeField] private PauseOptions option;

    public GameObject spacing;

    private IEnumerator _appear = null;
    private IEnumerator _disappear = null;

    public PauseOptionButton() : base()
    {
        OnAwakeActions(new List<Action>
        {
            delegate { spacing = transform.GetChild(0).gameObject; },
            delegate { GetComponent<Button>().onClick.AddListener(OnClick); },
        });
    }

    public override void RegisterManager()
    {
        SetManager(PauseButtonsManager.instance);
    }

    public override void EnterAction()
    {
        if (_disappear != null)
        {
            StopCoroutine(_disappear);
        }

        _appear = FadeManager.FadeOnly(spacing, true, speed);
        StartCoroutine(_appear);
    }

    public override void ExitAction()
    {
        if (_appear != null)
        {
            StopCoroutine(_appear);
        }

        _disappear = FadeManager.FadeOnly(spacing, false, speed);
        StartCoroutine(_disappear);
    }

    public override IEnumerator IClick()
    {
        StopAllCoroutines();
        PauseButtonsManager.instance.DisableButtons();
        PauseButtonsManager.instance.ExecuteOption(option);
        yield return null;
    }
}
