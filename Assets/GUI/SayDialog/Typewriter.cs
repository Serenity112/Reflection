using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct TypewriterSaveData
{
    public TypewriterSaveData(int i)
    {
        SavedDialogs = new();
    }

    public Dictionary<string, int> SavedDialogs;
}

public class Typewriter : MonoBehaviour
{
    public static Typewriter Instance;

    private Flowchart _flowchart;

    [SerializeField] private GameObject StoryText;
    [SerializeField] private SkipButton skipButton;

    private Text _text;

    private Coroutine _say;

    private bool extern_button_click_flag = false;

    // Задержка между буквами, контролируется из настроек
    private float defaultDelay;

    // Флаг скорости для опции "бесконечно" в настройках
    private bool instantSpeed = false;

    private Dictionary<string, int> DialogReadSaves = new Dictionary<string, int>();

    // Зависимый скип от загрузок, паузы
    private bool _SKIP_ENABLE = false;

    //Независимый скип, по сути просто индикато нажатия Tab
    public bool SkipIsActive { get; private set; } = false;

    // Клик игровой кнопки / Enter / Space
    private bool _CLICK_ENABLE = false;

    // Флаг для того, чтобы нельзя было зажать ЛКМ для скипа
    public bool continueClickedFlag = true;

    // Максимальная скорость, с которой она считается бесконечной
    private double MAX_TEXT_SPEED = 120;

    private void Awake()
    {
        Instance = this;

        _text = StoryText.GetComponent<Text>();

        ResetTypewritterFlags();
    }

    private void Update()
    {
        UpdateClick();
        UpdateSkip();
    }

    private void Start()
    {
        _flowchart = PanelsManager.instance.flowchart;

        LoadDialogReadSaves();
    }

    public void ResetTypewritterFlags()
    {
        SkipIsActive = false;
        extern_button_click_flag = false;
        _SKIP_ENABLE = false;
        _CLICK_ENABLE = false;
        continueClickedFlag = true;
    }

    private bool GetDenyStatus()
    {
        return (
            PauseButtonsManager.GAME_IS_PAUSED ||
            StaticVariables.OVERLAY_ACTIVE ||
            ChoiceManager.CHOICE_IS_ACTIVE
            );
    }

    // Нажимается кнопкой в игре
    public void ContinueButtonInput()
    {
        extern_button_click_flag = true;
    }

    // Обновления статуса нажимания мыши
    private void UpdateClick()
    {
        bool externalInputState = extern_button_click_flag;
        bool keyboardState = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return);

        if (externalInputState || keyboardState)
        {
            if (skipButton.IfButtonClicked())
            {
                skipButton.DisableSkipState();
                skipButton.DisableSkipAnimation();
            }

            if (externalInputState)
            {
                extern_button_click_flag = false;
            }

            if (!GetDenyStatus() && continueClickedFlag)
            {
                continueClickedFlag = false;
                _CLICK_ENABLE = true;
            }
            else
            {
                _CLICK_ENABLE = false;
            }
        }
        else
        {
            continueClickedFlag = true;
            _CLICK_ENABLE = false;
        }
    }

    private void UpdateSkip()
    {
        bool tabSkip = Input.GetKey(KeyCode.LeftControl);
        bool skipButtonClicked = skipButton.IfButtonClicked();

        if (tabSkip && skipButtonClicked)
        {
            skipButton.DisableSkipState();
        }

        if (tabSkip || skipButtonClicked)
        {
            if (!GetDenyStatus())
            {
                _SKIP_ENABLE = true;
            }
            else
            {
                _SKIP_ENABLE = false;
                // Если был скип и случилась, например, пауза - отменяем скип кнопкой
                skipButton.DisableSkipState();
                skipButton.DisableSkipAnimation();
            }

            if (!SkipIsActive)
            {
                SkipIsActive = true;
                OnSkipStart();
            }
        }
        else
        {
            if (SkipIsActive)
            {
                SkipIsActive = false;
                OnSkipEnd();
            }

            _SKIP_ENABLE = false;
        }
    }

    private void OnSkipStart()
    {
        skipButton.EnableSkipAnimation();
    }

    private void OnPostSkipEnd()
    {
        if (SettingsConfig.IfAllowExpandings())
        {
            SpriteController.instance.LoadSpritesExpandingInfo(true);
        }
    }

    private void OnSkipEnd()
    {
        skipButton.DisableSkipAnimation();


        if (!SpriteExpand.instance.isExecuting)
        {
            //SpriteExpand.instance.StopPrev(false);
        }
    }

    public void SetTextSpeed(float value)
    {
        if (value == MAX_TEXT_SPEED)
        {
            instantSpeed = true;
        }
        else
        {
            instantSpeed = false;
            defaultDelay = 1 / value;
        }
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    #region Typewriting

    private bool CalculateWasRead(string block, int index)
    {
        if (DialogReadSaves.ContainsKey(block))
        {
            if (index < DialogReadSaves[block])
            {
                return true;
            }
            else
            {
                DialogReadSaves[block] = index;
                return false;
            }
        }
        else
        {
            DialogReadSaves.Add(block, index);
            return false;
        }
    }

    // Методы для вывода текста общего / экстенда
    public IEnumerator ISayDialog(string storyText, Character speaker)
    {
        _text.text = "";

        LogManager.instance.CreateMessage(speaker, storyText);

        if (_say != null)
        {
            StopCoroutine(_say);
        }
        _say = StartCoroutine(ISayGeneric(storyText, speaker));
        yield return _say;
    }

    public IEnumerator ISayExtend(string prevText, string extendedText, Character speaker)
    {
        _text.text = prevText + " ";

        LogManager.instance.CreateMessage(speaker, extendedText);

        if (_say != null)
        {
            StopCoroutine(_say);
        }
        _say = StartCoroutine(ISayGeneric(extendedText, speaker));
        yield return _say;
    }

    // Флаг для быстрого скипа, нужно ресетать при тёмных экранах
    private bool auto_complete_next = false;

    public void ResetInstantSkip()
    {
        auto_complete_next = false;
    }

    // Обобщение вывода текста
    private IEnumerator ISayGeneric(string textToWrite, Character speaker)
    {
        SpriteExpand.instance.SetExpanding(speaker, SkipIsActive);

        NameChanger.instance.SetName(speaker);

        // Typewriting
        bool was_read = CalculateWasRead(_flowchart.ActiveBlock.BlockName, _flowchart.ActiveBlock.GetCurrentIndex());
        bool allow_skip_flag = (SettingsConfig.skipEverything || was_read);

        float timeElapsed = 0f;
        bool interrupt_flag = false;

        yield return null;

        if (_SKIP_ENABLE && allow_skip_flag)
        {
            auto_complete_next = true;
            _text.text = textToWrite;
            yield return null;
            yield break;
        }
        else if (auto_complete_next || instantSpeed)
        {
            OnPostSkipEnd();

            auto_complete_next = false;
            _text.text = textToWrite;
            yield return null;
        }
        else
        {
            auto_complete_next = false;

            foreach (char c in textToWrite)
            {
                _text.text += c;

                while (timeElapsed < defaultDelay)
                {
                    if (_SKIP_ENABLE && allow_skip_flag)
                    {
                        auto_complete_next = true;
                        interrupt_flag = true;
                        break;
                    }
                    if (_CLICK_ENABLE)
                    {
                        interrupt_flag = true;
                        break;
                    }

                    timeElapsed += Time.deltaTime;
                    yield return null;
                }

                timeElapsed -= defaultDelay;

                if (interrupt_flag)
                {
                    _text.text = textToWrite;
                    break;
                }
            }
        }

        yield return null;

        // Ожидание ввода Space / Tab / Нажатия игровой кнопки
        while (true)
        {
            if ((_SKIP_ENABLE && allow_skip_flag) || _CLICK_ENABLE)
            {
                if (_SKIP_ENABLE && allow_skip_flag)
                {
                    auto_complete_next = true;
                }
                yield return null;
                yield break;

            }
            yield return null;
        }
    }

    #endregion

    #region SaveSystem

    public void SaveDialogReadSaves()
    {
        ES3.Save<TypewriterSaveData>("ReadDialogs",
            new TypewriterSaveData(0) { SavedDialogs = DialogReadSaves },
            $"{SaveSystemUtils.SaveFilesData}");
    }

    public void LoadDialogReadSaves()
    {
        try
        {
            if (ES3.KeyExists("ReadDialogs", $"{SaveSystemUtils.SaveFilesData}"))
            {
                TypewriterSaveData loaded = ES3.Load<TypewriterSaveData>("ReadDialogs", $"{SaveSystemUtils.SaveFilesData}");
                if (loaded.SavedDialogs != null)
                {
                    DialogReadSaves = loaded.SavedDialogs;
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    #endregion
}
