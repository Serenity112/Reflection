using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MMButtonsManager;

public class MMOptionButton : IDraggableButtonGroup
{
    [SerializeField] private MainMenuOption option;

    [HideInInspector]
    public GameObject spacing;

    [SerializeField] private AudioSource ScrollSource;
    [SerializeField] private AudioSource ClickSource;

    private IEnumerator _appear = null;
    private IEnumerator _disappear = null;

    private float speed = 5.0f;

    public override void Awake()
    {
        base.Awake();
        spacing = transform.GetChild(0).gameObject;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public override void EnterAction()
    {
        if (_disappear != null)
        {
            StopCoroutine(_disappear);
        }

        _appear = FadeManager.FadeObject(spacing, true, speed);
        StartCoroutine(_appear);
    }

    public override void ExitAction()
    {
        if (_appear != null)
        {
            StopCoroutine(_appear);
        }

        _disappear = FadeManager.FadeObject(spacing, false, speed);
        StartCoroutine(_disappear);
    }

    public override IEnumerator IClick()
    {
        MMButtonsManager.instance.ExecuteOption(option);
        yield return null;
    }

    public override void ResetButtonState()
    {
        throw new NotImplementedException();
    }
}
