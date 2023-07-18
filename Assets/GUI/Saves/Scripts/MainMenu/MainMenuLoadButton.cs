using UnityEngine;

public class MainMenuLoadButton : MonoBehaviour
{
    [SerializeField] private GameObject File;
    private GameObject DeleteCross;
    private MainMenuLoad mainMenuLoad;

    private void Start()
    {
        mainMenuLoad = File.GetComponent<MainMenuLoad>();
        DeleteCross = mainMenuLoad.DeleteCross;
    }

    private void OnMouseEnter()
    {
        if (!StaticVariables.OverlayPanelActive)
        {
            DeleteCross.GetComponent<DeleteCrossButton>().AppearCross();
            StartCoroutine(mainMenuLoad.AppearCassette());
            StartCoroutine(mainMenuLoad.saveFileFields.CloseOverPanel());
        }
    }

    private void OnMouseExit()
    {
        if (!StaticVariables.OverlayPanelActive)
        {
            DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
            StartCoroutine(mainMenuLoad.DisappearCassette());
            StartCoroutine(mainMenuLoad.saveFileFields.OpenOverPanel());
        }
    }
}
