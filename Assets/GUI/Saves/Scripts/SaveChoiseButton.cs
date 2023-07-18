using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveChoiseButton : MonoBehaviour
{
    [SerializeField] private Side side;
    [SerializeField] private GameObject File;
    [SerializeField] private SaveOption option;

    private SaveChoiseIconAnimator saveChoiseIconAnimator;
    private SaveChoiseAnimator saveChoiseAnimator;
    private GameObject DeleteCross;

    private void Start()
    {
        saveChoiseIconAnimator = File.GetComponent<SaveChoiseIconAnimator>();
        saveChoiseAnimator = File.GetComponent<SaveChoiseAnimator>();
        DeleteCross = saveChoiseAnimator.DeleteCross;

        if (!File.GetComponent<SaveFileFields>().OnMouseEnter)
        {
            DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross(2);
        }
    }

    private void OnMouseEnter()
    {
        if (!StaticVariables.OverlayPanelActive)
        {
            DeleteCross.GetComponent<DeleteCrossButton>().AppearCross();
            saveChoiseIconAnimator.AppearSide(side);
        }
    }

    private void OnMouseExit()
    {
        if (!StaticVariables.OverlayPanelActive)
        {
            saveChoiseIconAnimator.RemoveSide(side);
            DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross(2);
        }
    }

    public void SaveAction()
    {
        saveChoiseAnimator.SaveAction(option);
    }
}
