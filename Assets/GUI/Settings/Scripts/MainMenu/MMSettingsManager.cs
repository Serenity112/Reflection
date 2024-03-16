using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MMSettingsManager : MonoBehaviour, ISettingsManager
{
    public static MMSettingsManager instance = null;

    [SerializeField]
    private GameObject Canvas1;
    [SerializeField]
    private GameObject Canvas2;
    [SerializeField]
    private GameObject Canvas3;

    [SerializeField]
    private GameObject SettingsGuiPanel;

    [SerializeField]
    private AudioMixer audioMixer;

    private GameObject BlackPanel;

    [SerializeField]
    private float FadingSpeed = 5f;

    private void Awake()
    {
        instance = this;

        SettingsConfig.currentManager = this;
    }

    void Start()
    {
        BlackPanel = PanelsConfig.CurrentManager.GetBlackPanel();

        SettingsConfig.LoadSettingsFromMemory();

        ApplySettingsOnStart();
        StaticVariables.GameLaunchedFirstTime = false;
    }

    public void OpenSettings() => StartCoroutine(IOpenSettings());

    public void CloseSettings() => StartCoroutine(ICloseSettings());

    private IEnumerator IOpenSettings()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));
        FadeManager.FadeObject(SettingsGuiPanel, true);
        SettingsGuiPanel.GetComponent<SettingsController>().InitialReset();
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));
        StaticVariables.IN_SETTINGS_MENU = true;
    }
    private IEnumerator ICloseSettings()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));
        FadeManager.FadeObject(SettingsGuiPanel, false);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));
        StaticVariables.IN_SETTINGS_MENU = false;
    }

    public void InstantApplySpecificSetting(SettingsList setting, SettingsOptions value, float data)
    {
        switch (setting)
        {
            case SettingsList.FullScreenMode:
                SettingsConfig.ChangeFullscreeenMode(value);
                break;
            case SettingsList.BlackFramesMode:
                SettingsConfig.ChangeBlackFramesMode(Canvas1, value);
                SettingsConfig.ChangeBlackFramesMode(Canvas2, value);
                SettingsConfig.ChangeBlackFramesMode(Canvas3, value);
                break;
            case SettingsList.Resolution:
                SettingsConfig.ChangeResoulution(value);
                break;
            case SettingsList.masterVolume:
                SettingsConfig.SetVolume(audioMixer, "MasterVol", data / 100); // Деление на 100 т.к. метод принимает числа от 0 до 1, а сейвах идут проценты от 0 до 100
                break;
            case SettingsList.musicVolume:
                SettingsConfig.SetVolume(audioMixer, "MusicVol", data / 100);
                break;
            case SettingsList.soundVolume:
                SettingsConfig.SetVolume(audioMixer, "SoundVol", data / 100);
                break;
            case SettingsList.Language:
                break;
        }
    }

    private void ApplySettingsOnStart()
    {
        foreach (SettingsList setting in (SettingsList[])Enum.GetValues(typeof(SettingsList)))
        {
            SettingsOptions value = SettingsConfig.chosenOptions[setting].settingsOption;
            float data = SettingsConfig.chosenOptions[setting].data;

            switch (setting)
            {
                case SettingsList.masterVolume:
                    SmoothMusicOnStart("MasterVol", data / 100);
                    break;
                case SettingsList.musicVolume:
                    SmoothMusicOnStart("MusicVol", data / 100);
                    break;
                case SettingsList.soundVolume:
                    SmoothMusicOnStart("SoundVol", data / 100);
                    break;
                case SettingsList.FullScreenMode:
                    if (StaticVariables.GameLaunchedFirstTime)
                    {
                        goto default;
                    }
                    break;
                case SettingsList.Resolution:
                    if (StaticVariables.GameLaunchedFirstTime)
                    {
                        goto default;
                    }
                    break;
                default:
                    InstantApplySpecificSetting(setting, value, data);
                    break;
            }
        }
    }

    private void SmoothMusicOnStart(string exposedParam, float data)
    {
        audioMixer.SetFloat(exposedParam, -80);
        float targetVolume_db = Mathf.Log10(data) * 20;
        StartCoroutine(FadeVolume(audioMixer, exposedParam, 3f, targetVolume_db));
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
