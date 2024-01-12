using Krivodeling.UI.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PauseOptions
{
    Continue,
    Saves,
    Settings,
    Quit
}

// По сути весь менеджер над меню паузы
public class PauseButtonsManager : IButtonManager
{
    public static PauseButtonsManager instance = null;

    public static bool GAME_IS_PAUSED { get; set; } = false;

    public static bool PAUSE_ANIMATION_ENDED { get; set; } = false;

    private float _speed = 5f;

    private bool _buttonClicked = false;

    public UIBlur uIBlur;

    public GameObject PausePanel;

    public void Awake()
    {
        instance = this;
    }

    private bool GetAllowStatus()
    {
        return  PAUSE_ANIMATION_ENDED &&
                !StaticVariables.OVERLAY_ACTIVE &&
                !_buttonClicked &&
                !StaticVariables.GAME_IS_LOADING;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GetAllowStatus())
            {
                ExecuteOption(PauseOptions.Continue);
            }
        }
    }

    public override void ResetAllButtonsState()
    {
        foreach (var button in GameButtons)
        {
            button.ResetButtonState();
        }
    }

    public void ExecuteOption(PauseOptions option)
    {
        // Избежать нажатия на другие кнопки при уже нажатой одной
        if (!GetAllowStatus())
        {
            return;
        }

        _buttonClicked = true;

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
        uIBlur.EndBlur(_speed);

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeOnly(PanelsManager.instance.GameGuiPanel, true, _speed),
            FadeManager.FadeOnly(PanelsManager.instance.GameButtons, true, _speed * 0.5f),
            FadeManager.FadeObject(PausePanel, false, _speed)
        }));

        ResetAllButtonsState();
        PausePanel.SetActive(false);

        GAME_IS_PAUSED = false;
        PAUSE_ANIMATION_ENDED = false;
    }

    public override void ResetManager()
    {
        _buttonClicked = false;
        ResetAllButtonsState();
    }
}
