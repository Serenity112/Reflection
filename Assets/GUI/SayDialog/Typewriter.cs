using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour
{
    public static Typewriter Instance;

    [SerializeField] private GameObject StoryText;
    [SerializeField] private SkipButton skipButton;

    private Text _text;

    public bool SkipIsActive { get; set; } = false;

    // Глобальный флаг для запрета скипа кнопкой Tab
    public bool denySkip { get; private set; } = false;

    // Локальный флаг для запрета скипа кнопкой Tab, сохрнаяет изменение режима isSkipping и анимации skip кнопки
    public bool denyNextDialog { get; private set; } = false;

    private Coroutine _say;

    private string currentText;

    private bool extern_button_click_flag = false;

    // Задержка между буквами, контролируется из настроек
    private float defaultDelay;

    // Флаг скорости для опции "бесконечно" в настройках
    private bool instantSpeed = false;

    public float TextSpeed { get; private set; }

    private string DialogSavesFile;

    private Dictionary<string, int> DialogSaves = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;

        _text = StoryText.GetComponent<Text>();

        DialogSavesFile = SaveSystemUtils.DialogSavesFile;
        LoadDialogSaves();
    }

    public bool continueClickedFlag = true;

    private void ResetContinueFlag()
    {
        continueClickedFlag = true;
    }

    public void ContinueButtonInput()
    {
        extern_button_click_flag = true;
    }

    private bool GetInputClickState()
    {
        return extern_button_click_flag;
    }

    private bool GetKeyboardClickState()
    {
        return (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return));
    }

    private bool RecalculateClickState()
    {
        bool inputState = GetInputClickState();
        bool keyboardState = GetKeyboardClickState();

        if (inputState || keyboardState)
        {
            if (skipButton.IfButtonClicked())
            {
                skipButton.DisableSkipState();
                skipButton.DisableSkipAnimation();
            }

            if (inputState)
            {
                extern_button_click_flag = false;
            }


            if (!denySkip && continueClickedFlag)
            {
                continueClickedFlag = false;
                return true;
            }
        }

        return false;
    }

    public bool GetRawSkipState()
    {
        return (Input.GetKey(KeyCode.Tab) || skipButton.IfButtonClicked());
    }

    private bool RecalculateSkipState()
    {
        bool tabSkip = Input.GetKey(KeyCode.Tab);
        bool skipButtonClicked = skipButton.IfButtonClicked();

        if (tabSkip && skipButtonClicked)
        {
            skipButton.DisableSkipState();
        }

        if (tabSkip || skipButtonClicked)
        {
            if (!SkipIsActive)
            {
                SkipIsActive = true;
                OnSkipStart();
            }

            if (!denySkip)
            {
                return true;
            }
        }
        else
        {
            if (SkipIsActive)
            {
                SkipIsActive = false;
                OnSkipEnd();
            }

        }

        return false;
    }

    private void OnSkipStart()
    {
        skipButton.EnableSkipAnimation();
        //SpriteController.instance.UnExpandAllSprites();
    }

    private void OnSkipEnd()
    {
        skipButton.DisableSkipAnimation();
        //CancelButtonAutoSkip();
        //SkipJustEnded = true;
    }


    public void RecalculateSkippingMode()
    {
        bool tabSkip = Input.GetKey(KeyCode.Tab);

        /*if ((tabSkip || AutoSkipButtonClickedFlag) && (SettingsConfig.skipEverything || wasCurrentDialogRead) && !denySkip)
        {
            if (!isSkipping)
            {
                isSkipping = true;
            }
        }
        else
        {
            if (isSkipping)
            {
                isSkipping = false;
            }
        }*/
    }

    public void AllowSkip()
    {
        denySkip = false;
        denyNextDialog = false;
    }

    public void DenySkip()
    {
        denySkip = true;
        denyNextDialog = true;
    }

    public void SetTextSpeed(float value)
    {
        if (value == 120)
        {
            instantSpeed = true;
        }
        else
        {
            instantSpeed = false;
            defaultDelay = 1 / value;
        }
    }

    // Метод вызываемый кнопкой в игре
    public void StopTypewriter()
    {
        if (_say != null)
        {
            StopCoroutine(_say);
        }
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    #region Typewriting

    private bool CalculateWasRead(string block, int index)
    {
        if (!DialogSaves.ContainsKey(block))
        {
            DialogSaves.Add(block, 0);
        }

        if (index <= DialogSaves[block])
        {
            return true;
        }
        else
        {
            DialogSaves[block] = index;
            return false;
        }
    }

    // Методы для вывода текста общего / экстенда
    public IEnumerator ISayDialog(string storyText, string speaker)
    {
        _text.text = "";
        currentText = storyText;

        LogManager.instance.NewMessage(storyText, speaker);

        if (_say != null)
        {
            StopCoroutine(_say);
        }
        _say = StartCoroutine(ISayGeneric(storyText, speaker));
        yield return _say;
    }

    public IEnumerator ISayExtend(string prevText, string extendedText, string speaker)
    {
        _text.text = prevText + " ";
        currentText = prevText + " " + extendedText;

        LogManager.instance.NewMessageExtended(extendedText, speaker);

        if (_say != null)
        {
            StopCoroutine(_say);
        }
        _say = StartCoroutine(ISayGeneric(extendedText, speaker));
        yield return _say;
    }

    private bool auto_complete_next = false;

    // Обобщение вывода текста
    private IEnumerator ISayGeneric(string textToWrite, string speaker)
    {
        //SpriteExpand.instance.SetExpanding(speaker, isSkipping);

        NameChanger.instance.SetName(speaker);

        bool allow_skip_flag = (SettingsConfig.skipEverything ||
            CalculateWasRead(UserData.instance.CurrentBlock, UserData.instance.CurrentCommandIndex));

        // Typewriting
        float timeElapsed = 0f;
        bool interrupt_flag = false;

        RecalculateClickState();
        bool instant_skip = RecalculateSkipState();
        
        if (instant_skip)
        {
            auto_complete_next = true;
            _text.text = textToWrite;
            ResetContinueFlag();
            yield return null;
            yield break;
        }
        else if (auto_complete_next || instantSpeed)
        {
            auto_complete_next = false;
            _text.text = textToWrite;
            yield return null;
        }
        else
        {
            foreach (char c in textToWrite)
            {
                _text.text += c;

                while (timeElapsed < defaultDelay)
                {
                    if (!GetKeyboardClickState())
                    {
                        ResetContinueFlag();
                    }

                    bool click_flag = RecalculateClickState();
                    bool skip_flag = RecalculateSkipState();

                    if (allow_skip_flag)
                    {
                        if (skip_flag)
                        {
                            auto_complete_next = true;
                            interrupt_flag = true;
                            break;
                        }
                        if (click_flag)
                        {
                            interrupt_flag = true;
                            break;
                        }
                    }

                    timeElapsed += Time.deltaTime;
                    yield return null;
                }

                timeElapsed -= defaultDelay;

                if (interrupt_flag)
                {
                    _text.text = currentText;
                    break;
                }              
            }
        }

        // Против зажатий кнопок Space / Enter
        while (true)
        {
            bool keyboardState = GetKeyboardClickState();

            if (!keyboardState)
            {
                ResetContinueFlag();
                break;
            }
            yield return null;
        }

        // Ожидание ввода Space / Tab / Нажатия игровой кнопки
        while (true)
        {
            bool click_flag = RecalculateClickState();
            bool skip_flag = RecalculateSkipState();

            if (click_flag || skip_flag)
            {
                yield break;
            }
            yield return null;
        }
    }

    #endregion

    #region SaveSystem

    public void SaveDialogSaves()
    {
        ES3.Save<Dictionary<string, int>>(DialogSavesFile, DialogSaves, $"{DialogSavesFile}.es3");
    }

    public void LoadDialogSaves()
    {
        if (ES3.KeyExists(DialogSavesFile, $"{DialogSavesFile}.es3"))
        {
            var loaded = ES3.Load<Dictionary<string, int>>(DialogSavesFile, $"{DialogSavesFile}.es3");
            if (loaded != null)
            {
                DialogSaves = loaded;
            }
        }
    }

    void OnApplicationQuit()
    {
        SaveDialogSaves();
    }

    #endregion
}
