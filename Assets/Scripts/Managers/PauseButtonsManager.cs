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

public class PauseButtonsManager : IButtonManager
{
    public static PauseButtonsManager instance = null;

    private float _speed = 5f;

    private bool _buttonClicked = false;

    public UIBlur uIBlur;

    public GameObject PausePanel;

    public void Awake()
    {
        instance = this;

        PausePanel = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (StaticVariables.PAUSE_ANIM_ENDED &&
                !StaticVariables.OVERLAY_UI_OPENED &&
                !_buttonClicked &&
                !StaticVariables.GAME_LOADING)
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
        if (_buttonClicked)
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
            FadeManager.FadeOnly(PausePanel, false, _speed)
        }));

        ResetAllButtonsState();
        PausePanel.SetActive(false);

        StaticVariables.PAUSED = false;
        StaticVariables.PAUSE_ANIM_ENDED = false;
    }

    public override void ResetManager()
    {
        _buttonClicked = false;
        ResetAllButtonsState();
    }
}
