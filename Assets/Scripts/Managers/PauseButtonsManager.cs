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
                !StaticVariables.OVER_UI &&
                !_buttonClicked &&
                !StaticVariables.GAME_LOADING)
            {
                ExecuteOption(PauseOptions.Continue);
            }
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

    public override void EnableButtons()
    {
        foreach (GameObject button_obj in GameButtons)
        {
            button_obj.GetComponent<BoxCollider>().enabled = true;
            button_obj.GetComponent<Button>().interactable = true;
        }
    }

    public override void ResetAllButtonsState()
    {
        foreach (GameObject button in GameButtons)
        {
            PauseOptionButton pauseOptionButton = button.GetComponent<PauseOptionButton>();
            pauseOptionButton.ResetButtonState();
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
        EnableButtons();
    }

    public IEnumerator HideManager(float speed)
    {
        yield return StartCoroutine(FadeManager.FadeObject(PausePanel, false, speed));
    }

    public IEnumerator ShowManager(float speed)
    {
        yield return StartCoroutine(FadeManager.FadeObject(PausePanel, true, speed));
    }

    public void SetActive(bool active)
    {
        PausePanel.SetActive(active);
    }

    public void SetAlpha(float alpha)
    {
        PausePanel.GetComponent<CanvasGroup>().alpha = alpha;
    }
}
