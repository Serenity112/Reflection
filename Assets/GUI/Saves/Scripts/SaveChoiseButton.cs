using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveChoiseButton : ISaveSystemButton
{
    [SerializeField] private Side side;
    [SerializeField] private SaveFileFields Fields;
    [SerializeField] private SaveOption option;

    private DeleteCrossButton DeleteCross;
    private SaveChoiseIconAnimator saveChoiseIconAnimator;
    private SaveChoiseAnimator saveChoiseAnimator;

    public override void Awake()
    {
        base.Awake();

        saveChoiseIconAnimator = Fields._SaveChoiseIconAnimator;
        saveChoiseAnimator = Fields._SaveChoiseAnimator;
        DeleteCross = saveChoiseAnimator.DeleteCross;

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
            DeleteCross.AppearCross();
            saveChoiseIconAnimator.AppearSide(side);
        }
    }

    public override void ExitAction()
    {
        if (!SaveManagerStatic.UiBloker)
        {
            saveChoiseIconAnimator.RemoveSide(side);
            DeleteCross.DisappearCross();
        }
    }

    private IEnumerator IClick()
    {
        if (!SaveManagerStatic.ClickBlocker)
        {
            SaveManagerStatic.ClickBlocker = true;
            SaveManagerStatic.UiBloker = true;

            yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentShrinkScale, expandTime));
            yield return StartCoroutine(ExpandManager.ExpandObject(buttonParent, parentOrigScale, expandTime));

            saveChoiseAnimator.SaveLoadDeleteAction(option);
            StartRotation();
        }

        yield return null;
    }

    public void StartRotation()
    {
        animator.Play("SL_Rotate");
    }

    public void EndRotation()
    {
        animator.Play("SL_Idle");
    }

    public override void ResetButtonState()
    {
        base.ResetButtonState();

        EndRotation();
        UnlockButton();
    }
}
