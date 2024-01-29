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
        if (!SaveManagerStatic.UIsystemDown)
        {
            StartCoroutine(firstSaveAnimator.IDisappearCassette());
        }
    }

    public override void ResetButtonState()
    {
        base.ResetButtonState();

        GetComponent<Button>().interactable = true;
        animator.Play("FC_Anim_Idle");
        animator.Play("FC_Fade_Idle");
    }

    public IEnumerator IClick()
    {
        if (!SaveManagerStatic.UIsystemDown)
        {
            SaveManagerStatic.UIsystemDown = true;
            yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
            yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

            animator.Play("FC_Anim_Rotate");
            GetComponent<Button>().interactable = false;
            firstSaveAnimator.FirstSaveIconClick();
        }

        yield return null;
    }
}
