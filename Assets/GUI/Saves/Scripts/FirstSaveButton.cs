using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSaveButton : MonoBehaviour
{
    [SerializeField] private GameObject File;
    private FirstSaveAnimator firstSaveAnimator;

    private void Start()
    {
        firstSaveAnimator = File.GetComponent<FirstSaveAnimator>();
    }
    private void OnMouseEnter()
    {
        firstSaveAnimator.AppearCassette();
    }

    private void OnMouseExit()
    {
        firstSaveAnimator.DisappearCassette();
    }
}
