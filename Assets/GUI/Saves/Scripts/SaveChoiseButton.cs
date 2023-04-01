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
    }
    private void OnMouseEnter()
    {
        saveChoiseIconAnimator.appearSide(side);
        DeleteCross.GetComponent<DeleteCrossButton>().AppearCross();
    }

    private void OnMouseExit()
    {
        saveChoiseIconAnimator.removeSide(side);
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
    }

    public void SaveAction()
    {
        saveChoiseAnimator.SaveAction(option);
    }
}
