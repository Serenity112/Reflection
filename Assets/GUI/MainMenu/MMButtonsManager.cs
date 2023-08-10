using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MMButtonsManager : IButtonManager
{
    public static MMButtonsManager instance = null;

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
        instance = this;

        GameButtons = new List<GameObject>();
    }

    private void Start()
    {
        EnableButtons();
        UnSelectButtons();
    }

    public override void UnSelectButtons()
    {
        foreach (GameObject button in GameButtons)
        {
            MMOptionButton underlineButton = button.GetComponent<MMOptionButton>();
            underlineButton.ResetFlags();
            GameObject spacing = underlineButton.spacing;
            spacing.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    public override void AppearActualButton()
    {
        foreach (GameObject button_obj in GameButtons)
        {
            MMOptionButton button = button_obj.GetComponent<MMOptionButton>();
            button.AppearIfEntered();
        }
    }

    public override void EnableButtons()
    {
        foreach (GameObject button_obj in GameButtons)
        {
            button_obj.GetComponent<BoxCollider>().enabled = true;
            button_obj.GetComponent<Button>().interactable = true;
        }
    }

    public override void DisableButtons()
    {
        foreach (GameObject button_obj in GameButtons)
        {
            button_obj.GetComponent<BoxCollider>().enabled = false;
            button_obj.GetComponent<Button>().interactable = false;
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
