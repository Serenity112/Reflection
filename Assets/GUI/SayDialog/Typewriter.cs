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

    public bool buttonAutoSkip = false;
    public bool wasCurrentDialogRead = false;
    public bool isSkipping = false;

    // Флаг для кнопок Space и Return
    public bool continueClickedFlag = true;

    // Глобальный флаг для запрета скипа кнопкой Tab
    public bool denySkip { get; private set; } = false;

    // Локальный флаг для запрета скипа кнопкой Tab, сохрнаяет изменение режима isSkipping и анимации skip кнопки
    public bool denyNextDialog { get; private set; } = false;

    private Coroutine _say;

    private string currentText;

    private bool clickFlag = false;

    // Задержка между буквами, контролируется из настроек
    private float defaultDelay;

    // Флаг скорости для опции "бесконечно" в настройках
    private bool instantSpeed = false;

    public float TextSpeed { get; private set; }

    private string DialogSavesFile;

    private Dictionary<string, List<int>> DialogSaves = new Dictionary<string, List<int>>();

    private void Awake()
    {
        Instance = this;

        _text = StoryText.GetComponent<Text>();

        DialogSavesFile = SaveSystemUtils.DialogSavesFile;
        LoadDialogSaves();
    }

    private void Update()
    {
        bool tabSkip = Input.GetKey(KeyCode.Tab);

        if (tabSkip && buttonAutoSkip && isSkipping)
        {
            CancelButtonAutoSkip();
        }

        if ((tabSkip || buttonAutoSkip) && (SettingsConfig.skipEverything || wasCurrentDialogRead) && !denySkip)
        {
            if (!isSkipping)
            {
                skipButton.EnableSkip();
                isSkipping = true;
            }

            if (!denyNextDialog)
            {
                SetClickFlag();
            }
        }
        else
        {
            // Отжатие авто-скипа
            if (isSkipping)
            {
                CancelButtonAutoSkip();
                skipButton.DisableSkip();
                isSkipping = false;
            }
        }

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return))
        {
            CancelButtonAutoSkip();

            if (!denySkip && !denyNextDialog && continueClickedFlag)
            {
                continueClickedFlag = false;
                SetClickFlag();
            }
        }
        else
        {
            continueClickedFlag = true;
        }
    }

    public void AllowSkip()
    {
        //Debug.Log("Skip allowed");
        denySkip = false;
        denyNextDialog = false;
    }

    public void DenySkip()
    {
        //Debug.Log("Skip denied");
        denySkip = true;
        denyNextDialog = true;
        buttonAutoSkip = false;
    }

    public void ForceUpdateSkippingState()
    {
        Update();
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

    private void CancelButtonAutoSkip()
    {
        if (buttonAutoSkip)
        {
            buttonAutoSkip = false;
        }
    }

    // Метод вызываемый кнопкой в игре
    public void ContinueButtonInput()
    {
        CancelButtonAutoSkip();
        SetClickFlag();
    }

    // Флаг о том, что надо скипать диалог
    public void SetClickFlag()
    {
        clickFlag = true;
    }

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

    // Методы для вывода текста общего / экстенда
    public IEnumerator ISayDialog(string storyText, string speaker)
    {
        _text.text = "";
        currentText = storyText;

        LogManager.instance.NewMessage(storyText, speaker);

        yield return StartCoroutine(ISayGeneric(storyText, speaker));
    }

    public IEnumerator ISayExtend(string prevText, string extendedText, string speaker)
    {
        _text.text = prevText + " ";
        currentText = prevText + " " + extendedText;

        LogManager.instance.NewMessageExtended(extendedText, speaker);

        yield return StartCoroutine(ISayGeneric(extendedText, speaker));
    }

    // Обобщение вывода текста
    private IEnumerator ISayGeneric(string textToWrite, string speaker)
    {
        SpriteExpand.instance.StopPrev();
        //SpriteExpand.instance.SetExpanding(speaker, Typewriter.Instance.isSkipping);

        NameChanger.instance.SetName(speaker);

        string curr_block = UserData.instance.CurrentBlock;
        int curr_ind = UserData.instance.CurrentCommandIndex;

        if (!DialogSaves.ContainsKey(curr_block))
        {
            DialogSaves.Add(curr_block, new List<int>());
        }

        if (DialogSaves[curr_block].Contains(curr_ind))
        {
            wasCurrentDialogRead = true;
        }
        else
        {
            wasCurrentDialogRead = false;
            DialogSaves[curr_block].Add(curr_ind);
        }

        _say = StartCoroutine(IDoTypewriting(textToWrite));
        yield return _say;
    }

    // Посимвольный вывод текста
    private IEnumerator IDoTypewriting(string storyText)
    {
        float timeElapsed = 0f;

        if (instantSpeed)
        {
            _text.text = storyText;
        }
        else
        {
            foreach (char c in currentText)
            {
                _text.text += c;

                while (timeElapsed < defaultDelay)
                {
                    if (clickFlag)
                    {
                        break;
                    }

                    timeElapsed += Time.deltaTime;
                    yield return null;
                }

                timeElapsed -= defaultDelay;

                if (clickFlag)
                {
                    clickFlag = false;
                    _text.text = currentText;
                    break;
                }
            }
        }

        while (!clickFlag)
        {
            yield return null;
        }

        clickFlag = false;
    }

    #endregion

    #region SaveSystem

    public void SaveDialogSaves()
    {
        ES3.Save<Dictionary<string, List<int>>>(DialogSavesFile, DialogSaves, $"{DialogSavesFile}.es3");
    }

    public void LoadDialogSaves()
    {
        if (ES3.KeyExists(DialogSavesFile, $"{DialogSavesFile}.es3"))
        {
            var loaded = ES3.Load<Dictionary<string, List<int>>>(DialogSavesFile, $"{DialogSavesFile}.es3");
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
