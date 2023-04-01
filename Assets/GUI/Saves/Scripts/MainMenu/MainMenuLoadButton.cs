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
        mainMenuLoad.AppearCassette();
        DeleteCross.GetComponent<DeleteCrossButton>().AppearCross();
    }

    private void OnMouseExit()
    {
        mainMenuLoad.DisappearCassette();
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
    }
}
