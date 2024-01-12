using System;
using System.Collections;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SettingsManager : MonoBehaviour, ISettingsManager
{
    public static SettingsManager instance = null;

    private GameObject GameCanvas;

    private GameObject PanelsCanvas;

    private GameObject OverlaysCanvas;

    private GameObject ActivePanels;

    private GameObject blackPanelGame;

    private GameObject blackPanelPanels;

    [SerializeField]
    private AudioMixer audioMixer;

    private Camera PanelsCamera;

    private float speed = 5f;
    [SerializeField]
    private GameObject SettingsPanel;

    private void Awake()
    {
        instance = this;

        
    }

    void Start()
    {
        GameCanvas = PanelsManager.instance.GameCanvas;
        PanelsCanvas = PanelsManager.instance.PanelsCanvas;
        OverlaysCanvas = PanelsManager.instance.OverlaysCanvas;
        ActivePanels = PanelsManager.instance.ActivePanels;
        blackPanelGame = PanelsManager.instance.BlackPanel;
        blackPanelPanels = PanelsManager.instance.blackPanelPanels;
        PanelsCamera = PanelsManager.instance.PanelsCamera;
        SettingsPanel = ActivePanels.transform.GetChild(0).gameObject;

        SettingsConfig.LoadSettingsFromMemory();

        ApplySettingsOnStart();
    }

    public void OpenSettings()
    {
        StartCoroutine(IOpenSettings());
    }

    public void CloseSettings()
    {
        StartCoroutine(ICloseSettings());
    }
    private IEnumerator IOpenSettings()
    {
        SettingsConfig.currentManager = GetComponent<SettingsManager>();

        FadeManager.FadeObject(blackPanelPanels, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelGame, true, speed));

        PanelsCamera.enabled = true;
        FadeManager.FadeObject(SettingsPanel, true);
        SettingsPanel.GetComponent<SettingsController>().InitialReset();

        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, false, speed));
        FadeManager.FadeObject(blackPanelGame, false);
        StaticVariables.IN_SETTINGS_MENU = true;
    }

    private IEnumerator ICloseSettings()
    {
        FadeManager.FadeObject(blackPanelGame, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, speed));

        PanelsCamera.enabled = false;
        FadeManager.FadeObject(SettingsPanel, false);

        PauseButtonsManager.instance.ResetManager();

        yield return StartCoroutine(FadeManager.FadeObject(blackPanelGame, false, speed));
        FadeManager.FadeObject(blackPanelPanels, false);

        StaticVariables.IN_SETTINGS_MENU = false;
    }

    // Не все настрйоки нужно просто включить. Например музыку надо плавно вцвести
    private void ApplySettingsOnStart()
    {
        foreach (SettingsList setting in (SettingsList[])Enum.GetValues(typeof(SettingsList)))
        {
            SettingsOptions value = SettingsConfig.chosenOptions[setting].settingsOption;
            float data = SettingsConfig.chosenOptions[setting].data;

            switch (setting)
            {
                case SettingsList.masterVolume:
                    //audioMixer.SetFloat("MasterVol", data / 100);
                    SmoothMusicOnStart("MasterVol", data / 100); // Деление на 100 т.к. метод принимает числа от 0 до 1
                    break;
                case SettingsList.musicVolume:
                    SmoothMusicOnStart("MusicVol", data / 100);
                    break;
                case SettingsList.ambientVolume:
                    SmoothMusicOnStart("AmbientVol", data / 100);
                    break;
                case SettingsList.soundVolume:
                    SmoothMusicOnStart("SoundVol", data / 100);
                    break;
                case SettingsList.FullScreenMode:
                    // Не применять, переносится с мейн меню
                    break;
                case SettingsList.Resolution:
                    // Не применять, переносится с мейн меню
                    break;
                default:
                    InstantApplySpecificSetting(setting, value, data);
                    break;
            }
        }
    }


    public void InstantApplySpecificSetting(SettingsList setting, SettingsOptions value, float data)
    {
        switch (setting)
        {
            case SettingsList.FullScreenMode:
                SettingsConfig.ChangeFullscreeenMode(value);
                break;
            case SettingsList.BlackFramesMode:
                SettingsConfig.ChangeBlackFramesMode(GameCanvas, value);
                SettingsConfig.ChangeBlackFramesMode(PanelsCanvas, value);
                SettingsConfig.ChangeBlackFramesMode(OverlaysCanvas, value);
                break;
            case SettingsList.Resolution:
                SettingsConfig.ChangeResoulution(value);
                break;
            case SettingsList.masterVolume:
                SettingsConfig.SetVolume(audioMixer, "MasterVol", data / 100); // Деление на 100 т.к. метод принимает числа от 0 до 1
                break;
            case SettingsList.musicVolume:
                SettingsConfig.SetVolume(audioMixer, "MusicVol", data / 100);
                break;
            case SettingsList.soundVolume:
                SettingsConfig.SetVolume(audioMixer, "SoundVol", data / 100);
                break;
            case SettingsList.ambientVolume:
                SettingsConfig.SetVolume(audioMixer, "AmbientVol", data / 100);
                break;
            case SettingsList.TextSpeed:
                SettingsConfig.ChangeTextSpeed(Typewriter.Instance, data);
                break;
            case SettingsList.Language:
                break;
            case SettingsList.SpriteExpand:
                if (data == 0) // Запретить
                {
                    SpriteController.instance.UnExpandAllSprites();
                }
                else // Разрешить
                {
                    SpriteController.instance.LoadSpritesExpandingInfo(false);
                }
                break;
        }
    }

    private void SmoothMusicOnStart(string exposedParam, float volume_linear)
    {
        float fadeInTime = 3f;
        audioMixer.SetFloat(exposedParam, 0);
        float targetVolume_db = Mathf.Log10(volume_linear) * 20;
        StartCoroutine(FadeVolume(audioMixer, exposedParam, fadeInTime, targetVolume_db));
    }

    private IEnumerator FadeVolume(AudioMixer mixer, string exposedParam, float duration, float targetVolume_db)
    {
        float currentTime = 0;
        mixer.GetFloat(exposedParam, out float currentVol);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetVolume_db, currentTime / duration);
            mixer.SetFloat(exposedParam, newVol);
            yield return null;
        }
        yield break;
    }
}
