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
    private GameObject MainCanvas;

    [SerializeField]
    private AudioMixer audioMixer;

    private GameObject ActivePanels;

    private GameObject BlackPanel;

    [SerializeField]
    private float FadingSpeed = 5f;

    private AsyncOperationHandle<GameObject> handler;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        SettingsConfig.currentManager = this;
    }

    void Start()
    {
        ActivePanels = PanelsConfig.CurrentManager.GetActivePanelsParent();

        BlackPanel = PanelsConfig.CurrentManager.GetBlackPanel();

        SettingsConfig.LoadSettingsFromMemory();

        ApplySettingsOnStart();

        StaticVariables.GameIsStarting = false;
    }

    public void OpenSettings() => StartCoroutine(IOpenSettings());

    public void CloseSettings() => StartCoroutine(ICloseSettings());

    private IEnumerator IOpenSettings()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));

        handler = Addressables.InstantiateAsync("SettingsGuiPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return handler;

        if (handler.Status == AsyncOperationStatus.Succeeded)
        {
            handler.Result.name = "SettingsGuiPanel";
            yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));
        }
        else
        {
            Debug.Log("Error loading");
        }
    }
    private IEnumerator ICloseSettings()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));

        MMButtonsManager.instance.EnableButtons();

        Addressables.ReleaseInstance(handler);

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));

        Resources.UnloadUnusedAssets();
    }

    public void InstantApplySpecificSetting(SettingsList setting, SettingsOptions value, float data)
    {
        switch (setting)
        {
            case SettingsList.FullScreenMode:
                SettingsConfig.ChangeFullscreeenMode(value);
                break;
            case SettingsList.BlackFramesMode:
                SettingsConfig.ChangeBlackFramesMode(MainCanvas, value);
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
        audioMixer.SetFloat("MasterVol", 0);
        audioMixer.SetFloat("MusicVol", 0);
        audioMixer.SetFloat("SoundVol", 0);

        foreach (SettingsList setting in (SettingsList[])Enum.GetValues(typeof(SettingsList)))
        {
            SettingsOptions value = SettingsConfig.chosenOptions[setting].settingsOption;
            float data = SettingsConfig.chosenOptions[setting].data;

            switch (setting)
            {
                case SettingsList.masterVolume:
                    SmoothMusicOnStart("MasterVol", data);
                    break;
                case SettingsList.musicVolume:
                    SmoothMusicOnStart("MusicVol", data);
                    break;
                case SettingsList.soundVolume:
                    SmoothMusicOnStart("SoundVol", data);
                    break;
                case SettingsList.FullScreenMode:
                    if (StaticVariables.GameIsStarting)
                    {
                        InstantApplySpecificSetting(setting, value, data);
                    }
                    break;
                case SettingsList.Resolution:
                    if (StaticVariables.GameIsStarting)
                    {
                        InstantApplySpecificSetting(setting, value, data);
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
        audioMixer.SetFloat(exposedParam, 0);
        float targetVolume_db = Mathf.Log10(data / 100) * 20;
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
