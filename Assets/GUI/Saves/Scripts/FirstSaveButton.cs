using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirstSaveButton : ISaveSystemButton
{
    [SerializeField] private GameObject File;

    public GameObject CassetteImg;
    private FirstSaveAnimator firstSaveAnimator;

    public override void Awake()
    {
        base.Awake();

        CassetteImg = transform.GetChild(0).gameObject;
        firstSaveAnimator = File.GetComponent<FirstSaveAnimator>();

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(delegate
        {
            StartCoroutine(IClick());
        });
    }

    public override void EnterAction()
    {
        StartCoroutine(firstSaveAnimator.IAppearCassette());
    }

    public override void ExitAction()
    {
        StartCoroutine(firstSaveAnimator.IDisappearCassette());
    }

    public IEnumerator IClick()
    {
        if (!SaveManagerStatic.ClickBlocker)
        {
            SaveManagerStatic.ClickBlocker = true;
            firstSaveAnimator.Animating = true;

            yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
            yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

            animator.Play("FC_Anim_Rotate");

            StartCoroutine(firstSaveAnimator.IFirstSaveIconClick());
        }

        yield return null;
    }

    public override void ResetButtonState()
    {
        base.ResetButtonState();

        UnlockButton();
        animator.Play("FC_Anim_Idle");
        animator.Play("FC_Fade_Idle");
    }
}
