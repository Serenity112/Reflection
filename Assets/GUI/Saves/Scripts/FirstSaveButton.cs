using UnityEngine;

public class FirstSaveButton : MonoBehaviour
{
    [SerializeField] private GameObject File;
    private FirstSaveAnimator firstSaveAnimator;
    private SaveFileFields saveFileFields;

    private void Start()
    {
        firstSaveAnimator = File.GetComponent<FirstSaveAnimator>();
        saveFileFields = File.GetComponent<SaveFileFields>();
    }

    private void OnMouseEnter()
    {
        saveFileFields.OnMouseEnter = true;
        firstSaveAnimator.AppearCassette();
    }

    private void OnMouseExit()
    {
        saveFileFields.OnMouseEnter = false;
        firstSaveAnimator.DisappearCassette();
    }
}
