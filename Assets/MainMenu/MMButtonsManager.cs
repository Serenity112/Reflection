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

        GameButtons = new List<IDraggableButton>();
    }

    private void Start()
    {
        ResetAllButtonsState();
    }

    public override void ResetAllButtonsState()
    {
        foreach (var button in GameButtons)
        {
            button.ResetButtonState();
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

    public override void ResetManager()
    {
        ResetAllButtonsState();
    }
}
