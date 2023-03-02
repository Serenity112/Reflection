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

    private void Start()
    {
        saveChoiseIconAnimator = File.GetComponent<SaveChoiseIconAnimator>();
        saveChoiseAnimator = File.GetComponent<SaveChoiseAnimator>();
    }
    private void OnMouseEnter()
    {
        saveChoiseIconAnimator.appearSide(side);
    }

    private void OnMouseExit()
    {
        saveChoiseIconAnimator.removeSide(side);
    }

    public void SaveAction()
    {
        saveChoiseAnimator.SaveAction(option);
    }
}
