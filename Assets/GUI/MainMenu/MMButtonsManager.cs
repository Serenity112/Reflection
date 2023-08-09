using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MMButtonsManager : MonoBehaviour
{
    public static MMButtonsManager instance = null;

    private List<GameObject> MainMenuOptionButtons;

    [HideInInspector] public bool ButtonSelected = false;

    public enum MainMenuOption
    {
        Continue,
        NewGame,
        Saves,
        Settings,
        AboutUs,
        Quit
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        MainMenuOptionButtons = new List<GameObject>();
    }

    private void Start()
    {
        EnableButtons();
        UnlineButtons();
    }

    public void SubscribeButton(GameObject button)
    {
        MainMenuOptionButtons.Add(button);
    }

    public void UnlineButtons()
    {
        foreach (GameObject button in MainMenuOptionButtons)
        {
            MMOptionButton underlineButton = button.GetComponent<MMOptionButton>();
            underlineButton.StopAllCoroutines();

            GameObject spacing = underlineButton.spacing;
            spacing.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    public void AppearActualButton()
    {
        foreach (GameObject button_obj in MainMenuOptionButtons)
        {
            MMOptionButton button = button_obj.GetComponent<MMOptionButton>();
            button.StopAllCoroutines();
            button.AppearIfEntered();
        }
    }

    public void EnableButtons()
    {
        foreach (GameObject button_obj in MainMenuOptionButtons)
        {
            button_obj.GetComponent<BoxCollider>().enabled = true;
            button_obj.GetComponent<Button>().enabled = true;
        }
    }

    public void DisableButtons()
    {
        foreach (GameObject button_obj in MainMenuOptionButtons)
        {
            button_obj.GetComponent<BoxCollider>().enabled = false;
            button_obj.GetComponent<Button>().enabled = false;
        }
    }

    public void ExecuteOption(MainMenuOption option)
    {
        switch (option)
        {
            case MainMenuOption.NewGame:
                MMPanelsManager.instance.StartNewGame();
                break;
            case MainMenuOption.Continue:
                MMPanelsManager.instance.ContinueGame();
                break;
            case MainMenuOption.Saves:
                MMPanelsManager.instance.OpenSaveMenu();
                break;
            case MainMenuOption.Settings:
                MMSettingsManager.instance.OpenSettings();
                break;
            case MainMenuOption.AboutUs:
                MMPanelsManager.instance.OpenInfoMenu();
                break;
            case MainMenuOption.Quit:
                MMPanelsManager.instance.QuitGame();
                break;
        }
    }
}
