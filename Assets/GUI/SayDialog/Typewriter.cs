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

    private Coroutine _say;

    private bool extern_button_click_flag = false;

    // �������� ����� �������, �������������� �� ��������
    private float defaultDelay;

    // ���� �������� ��� ����� "����������" � ����������
    private bool instantSpeed = false;

    private string DialogSavesFile;

    private Dictionary<string, int> DialogSaves = new Dictionary<string, int>();

    // ��������� ���� �� ��������, �����
    private bool _SKIP_ENABLE = false;

    //����������� ����, �� ���� ������ �������� ������� Tab
    public bool SkipIsActive { get; private set; } = false;

    private bool _POST_SKIP = false;

    // ���� ������� ������ / Enter / Space
    private bool _CLICK_ENABLE = false;

    // ���� ��� ����, ����� ������ ���� ������ ��� ��� �����
    public bool continueClickedFlag = true;

    // ������������ ��������, � ������� ��� ��������� �����������
    private double MAX_TEXT_SPEED = 120;

    private void Awake()
    {
        Instance = this;

        _text = StoryText.GetComponent<Text>();

        DialogSavesFile = SaveSystemUtils.DialogSavesFile;

        ResetTypewritterFlags();
    }

    private void Update()
    {
        UpdateClick();
        UpdateSkip();
    }

    private void Start()
    {
        LoadDialogSaves();
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
            StaticVariables.OVERLAY_ACTIVE
            );
    }

    // ���������� ������� � ����
    public void ContinueButtonInput()
    {
        extern_button_click_flag = true;
    }

    // ���������� ������� ��������� ����
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
        bool tabSkip = Input.GetKey(KeyCode.Tab);
        bool skipButtonClicked = skipButton.IfButtonClicked();

        if (tabSkip && skipButtonClicked)
        {
            skipButton.DisableSkipState();
        }

        if (tabSkip || skipButtonClicked)
        {
            if (GetDenyStatus())
            {
                _SKIP_ENABLE = false;
                // ���� ��� ���� � ���������, ��������, ����� - �������� ���� �������
                skipButton.DisableSkipState();
                skipButton.DisableSkipAnimation();
            }
            else
            {
                if (!SkipIsActive)
                {
                    SkipIsActive = true;
                    OnSkipStart();
                }

                _SKIP_ENABLE = true;
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


        AudioManager.instance.OnSkipEnd();

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

    // ������ ��� ������ ������ ������ / ��������
    public IEnumerator ISayDialog(string storyText, Character speaker)
    {
        _text.text = "";

        //LogManager.instance.NewMessage(storyText, speaker);

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

        //LogManager.instance.NewMessageExtended(extendedText, speaker);

        if (_say != null)
        {
            StopCoroutine(_say);
        }
        _say = StartCoroutine(ISayGeneric(extendedText, speaker));
        yield return _say;
    }

    // ���� ��� �������� �����, ����� �������� ��� ����� �������
    private bool auto_complete_next = false;

    public void ResetInstantSkip()
    {
        auto_complete_next = false;
    }

    // ��������� ������ ������
    private IEnumerator ISayGeneric(string textToWrite, Character speaker)
    {
        SpriteExpand.instance.SetExpanding(speaker, SkipIsActive || _POST_SKIP);

        NameChanger.instance.SetName(speaker);

        bool allow_skip_flag = (SettingsConfig.skipEverything ||
            CalculateWasRead(UserData.instance.CurrentBlock, UserData.instance.CurrentCommandIndex));

        // Typewriting
        float timeElapsed = 0f;
        bool interrupt_flag = false;

        yield return null;

        if (_SKIP_ENABLE)
        {
            _POST_SKIP = true;
            auto_complete_next = true;
            _text.text = textToWrite;
            yield return null;
            yield break;
        }
        else if (auto_complete_next || instantSpeed)
        {
            if (_POST_SKIP)
            {
                _POST_SKIP = false;
                OnPostSkipEnd();
            }
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
                    if (allow_skip_flag)
                    {
                        if (_SKIP_ENABLE)
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

        // �������� ����� Space / Tab / ������� ������� ������
        while (true)
        {
            if (_SKIP_ENABLE || _CLICK_ENABLE)
            {
                if (_SKIP_ENABLE)
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

    public void SaveDialogSaves()
    {
        ES3.Save<Dictionary<string, int>>(DialogSavesFile, DialogSaves, $"{DialogSavesFile}.es3");
    }

    public void LoadDialogSaves()
    {
        try
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
        catch (Exception ex)
        {
            //WarningPanel.instance.CreateWarningPanel(WarningPanel.SavingErrorMessage, PanelsManager.instance.ActivePanels.transform);
        }
    }

    void OnApplicationQuit()
    {
        SaveDialogSaves();
    }

    #endregion
}
