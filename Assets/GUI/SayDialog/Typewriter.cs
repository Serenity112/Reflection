using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Typewriter : MonoBehaviour
{
    public static Typewriter Instance;

    [SerializeField] private GameObject StoryText;

    private Text _text;

    public bool buttonAutoSkip = false;
    public bool wasCurrentDialogRead = false;
    public bool isSkipping = false;

    // Флаг для кнопок Space и Return
    public bool continueClickedFlag = true;

    // Глобальный флаг для запрета скипа кнопкой Tab
    public bool denySkip { get; set; } = false;

    // Локальный флаг для запрета скипа кнопкой Tab, сохрнаяет изменение режима isSkipping и анимации skip кнопки
    public bool denyNextDialog { get; set; } = false;

    [SerializeField] private SkipButton skipButton;

    private Coroutine _say;

    private string currentText;

    private bool clickFlag = false;

    private float defaultDelay;

    private bool instantSpeed = false;

    public float TextSpeed { get; private set; }

    private void Awake()
    {
        Instance = this;

        _text = StoryText.GetComponent<Text>();
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

    public IEnumerator SayExtend(string extendedText, string prevText)
    {
        _text.text = prevText + " ";
        currentText = prevText + " " + extendedText;

        _say = StartCoroutine(ISay(extendedText));
        yield return _say;
    }

    public IEnumerator Say(string storyText)
    {
        _text.text = "";
        currentText = storyText;

        _say = StartCoroutine(ISay(storyText));
        yield return _say;
    }

    private IEnumerator ISay(string storyText)
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
}
