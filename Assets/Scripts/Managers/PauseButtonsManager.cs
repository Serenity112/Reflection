using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonsManager : IButtonManager
{
    public static PauseButtonsManager instance = null;

    private float speed = 5f;

    public bool FreezeButtons { get; private set; } = false;

    public enum PauseOptions
    {
        Continue,
        Saves,
        Settings,
        Quit
    }

    private void Awake()
    {
        instance = this;

        GameButtons = new List<GameObject>();
    }

    public override void AppearActualButton()
    {
        foreach (GameObject button_obj in GameButtons)
        {
            PauseOptionButton button = button_obj.GetComponent<PauseOptionButton>();
            button.AppearIfEntered();
        }
    }

    public override void DisableButtons()
    {
        FreezeButtons = true;
        foreach (GameObject button_obj in GameButtons)
        {
            button_obj.GetComponent<BoxCollider>().enabled = false;
            button_obj.GetComponent<Button>().interactable = false;
        }
    }

    public override void EnableButtons()
    {
        FreezeButtons = false;
        foreach (GameObject button_obj in GameButtons)
        {
            button_obj.GetComponent<BoxCollider>().enabled = true;
            button_obj.GetComponent<Button>().interactable = true;
        }
    }

    public override void UnSelectButtons()
    {
        foreach (GameObject button in GameButtons)
        {
            PauseOptionButton underlineButton = button.GetComponent<PauseOptionButton>();
            underlineButton.ResetFlags();
            GameObject spacing = underlineButton.spacing;
            spacing.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    public void ExecuteOption(PauseOptions option)
    {
        switch (option)
        {
            case PauseOptions.Continue:
                StartCoroutine(IContinue());
                break;
            case PauseOptions.Saves:
                PanelsManager.instance.OpenSaveMenu();
                break;
            case PauseOptions.Settings:
                SettingsManager.instance.OpenSettings();
                break;
            case PauseOptions.Quit:
                PanelsManager.instance.QuitToMainMenu();
                break;
        }
    }

    private IEnumerator IContinue()
    {
        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, speed * 0.5f),
            FadeManager.FadeObject(PanelsManager.instance.PausePanel, false, speed)
        }));

        UnSelectButtons();

        Typewriter.Instance.AllowSkip();
    }
}
