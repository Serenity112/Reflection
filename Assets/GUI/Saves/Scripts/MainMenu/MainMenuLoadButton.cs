using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLoadButton : MonoBehaviour
{
    [SerializeField] private GameObject File;
    private MainMenuLoad mainMenuLoad;

    private void Start()
    {
        mainMenuLoad = File.GetComponent<MainMenuLoad>();
    }
    private void OnMouseEnter()
    {
        mainMenuLoad.AppearCassette();
    }

    private void OnMouseExit()
    {
        mainMenuLoad.DisappearCassette();
    }
}
