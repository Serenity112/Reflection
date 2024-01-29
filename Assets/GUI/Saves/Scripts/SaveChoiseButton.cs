using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveChoiseButton : IExpandableButton
{
    [SerializeField] private Side side;
    [SerializeField] private GameObject File;
    [SerializeField] private SaveOption option;

    private DeleteCrossButton DeleteCross;
    //private FirstSaveAnimator firstSaveAnimator;
    private SaveChoiseIconAnimator saveChoiseIconAnimator;
    private SaveChoiseAnimator saveChoiseAnimator;


    public override void Awake()
    {
        base.Awake();

        //firstSaveAnimator = File.GetComponent<FirstSaveAnimator>();
        saveChoiseIconAnimator = File.GetComponent<SaveChoiseIconAnimator>();
        saveChoiseAnimator = File.GetComponent<SaveChoiseAnimator>();
        DeleteCross = saveChoiseAnimator.DeleteCross;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public override void EnterAction()
    {
        DeleteCross.AppearCross();
        saveChoiseIconAnimator.AppearSide(side);

        if (!SaveManagerStatic.OverlayPanelActive)
        {

        }
    }

    public override void ExitAction()
    {
        saveChoiseIconAnimator.RemoveSide(side);
        DeleteCross.DisappearCross();

        if (!SaveManagerStatic.OverlayPanelActive)
        {

        }
    }

    public override IEnumerator IClick()
    {
        if (!SaveManagerStatic.UIsystemDown)
        {
            SaveManagerStatic.UIsystemDown = true;

            saveChoiseAnimator.SaveAction(option);
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
        GetComponent<Button>().interactable = true;
    }
}
