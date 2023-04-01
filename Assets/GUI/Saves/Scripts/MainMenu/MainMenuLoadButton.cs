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
        if (!StaticVariables.ConfirmationPanelActive)
        {
            DeleteCross.GetComponent<DeleteCrossButton>().AppearCross();
            mainMenuLoad.AppearCassette();
        }
    }

    private void OnMouseExit()
    {
        if (!StaticVariables.ConfirmationPanelActive)
        {
            DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
            mainMenuLoad.DisappearCassette();
        }
    }
}
