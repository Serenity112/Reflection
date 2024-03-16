using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCrossButton : ISaveSystemButton
{
    private IEnumerator expandOnEnter;
    private IEnumerator shrinkOnExit;

    private IEnumerator CrossFadeIn;
    private IEnumerator CrossFadeOut;

    private float _speed = 7f;

    [SerializeField] private SaveChoiseAnimator saveChoiceAnimator;

    public override void Awake()
    {
        base.Awake();

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(delegate
        {
            StartCoroutine(IClick());
        });
    }

    public override void EnterAction()
    {
        if (!SaveManagerStatic.UiBloker)
        {
            AppearCross();
        }
    }

    public override void ExitAction()
    {
        if (!SaveManagerStatic.UiBloker)
        {
            DisappearCross();
        }
    }

    public void AppearCross()
    {
        if (CrossFadeOut != null)
            StopCoroutine(CrossFadeOut);

        CrossFadeIn = FadeManager.FadeOnly(gameObject, true, _speed);
        StartCoroutine(CrossFadeIn);
    }

    public void DisappearCross()
    {
        if (CrossFadeIn != null)
            StopCoroutine(CrossFadeIn);

        CrossFadeOut = FadeManager.FadeOnly(gameObject, false, _speed);
        StartCoroutine(CrossFadeOut);
    }

    private IEnumerator IClick()
    {
        if (!SaveManagerStatic.ClickBlocker)
        {
            SaveManagerStatic.ClickBlocker = true;
            SaveManagerStatic.UiBloker = true;

            yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>()
            {
                CoroutineUtils.WaitForSequence(new List<IEnumerator>()
                {
                    ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime),
                    ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime)
                }),
                saveChoiceAnimator.DeleteAction()
            }));
        }
    }

    public override void ResetButtonState()
    {
        base.ResetButtonState();

        UnlockButton();
    }
}
