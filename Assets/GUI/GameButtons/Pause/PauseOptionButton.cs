using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseOptionButton : IDraggableButton
{
    [SerializeField] private PauseOptions option;

    private GameObject _spacing;

    private IEnumerator _appear = null;
    private IEnumerator _disappear = null;

    private float speed = 5.0f;

    public override void Awake()
    {
        base.Awake();

        _spacing = transform.GetChild(0).gameObject;
        _spacing.GetComponent<CanvasGroup>().alpha = 0f;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Start()
    {
        PauseButtonsManager.instance.SubscribeButton(this.gameObject);
    }

    public override void EnterAction()
    {
        if (_disappear != null)
        {
            StopCoroutine(_disappear);
        }

        _appear = FadeManager.FadeOnly(_spacing, true, speed);
        StartCoroutine(_appear);
    }

    public override void ExitAction()
    {
        if (_appear != null)
        {
            StopCoroutine(_appear);
        }

        _disappear = FadeManager.FadeOnly(_spacing, false, speed);
        StartCoroutine(_disappear);
    }

    public override IEnumerator IClick()
    {
        StopAllCoroutines();
        PauseButtonsManager.instance.ExecuteOption(option);
        yield return null;
    }

    public override void ResetButtonState()
    {
        StopAllCoroutines();
        ResetFlags();
        _spacing.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
