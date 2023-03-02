using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Fungus;

public class SettingsManager : MonoBehaviour, ISettingsManager
{
    public static SettingsManager instance = null;

    [SerializeField]
    private GameObject GameCanvas;

    [SerializeField]
    private GameObject PanelsCanvas;

    [SerializeField]
    private GameObject ActivePanels;

    [SerializeField]
    private GameObject blackPanelGame;

    [SerializeField]
    private GameObject blackPanelPanels;

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private float introMusicFadeSpeed = 3f;

    [SerializeField]
    private Camera PanelsCamera;

    [SerializeField]
    private float settingsSpeed;

    private AsyncOperationHandle<GameObject> settingsPanelHandler;


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
    }
    void Start()
    {
        SettingsConfig.LoadSettingsFromMemory();

        InitSettingsOnStart();
    }

    public void OpenSettings()
    {
        StartCoroutine(IOpenSettings());
    }
    public void CloseSettings()
    {
        ButtonsManager.unlinePauseButtons();

        StartCoroutine(ICloseSettings());
    }
    private IEnumerator IOpenSettings()
    {
        SettingsConfig.currentManager = GetComponent<SettingsManager>();

        FadeManager.FadeObject(blackPanelPanels, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelGame, true, settingsSpeed));

        settingsPanelHandler = Addressables.InstantiateAsync("SettingsGuiPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return settingsPanelHandler;

        if (settingsPanelHandler.Status == AsyncOperationStatus.Succeeded)
        {
            settingsPanelHandler.Result.name = "SettingsGui";
            PanelsCamera.enabled = true;

            yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, false, settingsSpeed));
            FadeManager.FadeObject(blackPanelGame, false);
        }
        else
        {
            Debug.Log("Error loading");
        }
    }
    private IEnumerator ICloseSettings()
    {
        FadeManager.FadeObject(blackPanelGame, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, settingsSpeed));

        Addressables.ReleaseInstance(settingsPanelHandler);
        PanelsCamera.enabled = false;

        yield return StartCoroutine(FadeManager.FadeObject(blackPanelGame, false, settingsSpeed));
        FadeManager.FadeObject(blackPanelPanels, false);

        Resources.UnloadUnusedAssets();
    }

    public void ApplySetting(Settings setting, SettingsOptions value, float data)
    {
        switch (setting)
        {
            case Settings.FullScreenMode:
                SettingsConfig.ChangeFullscreeenMode(value);
                break;
            case Settings.BlackFramesMode:
                SettingsConfig.ChangeBlackFramesMode(GameCanvas, value);
                SettingsConfig.ChangeBlackFramesMode(PanelsCanvas, value);
                break;
            case Settings.Resolution:
                SettingsConfig.ChangeResoulution(value);
                break;
            case Settings.masterVolume:
                SettingsConfig.SetVolume(audioMixer, "MasterVol", data);
                break;
            case Settings.musicVolume:
                SettingsConfig.SetVolume(audioMixer, "MusicVol", data);
                break;
            case Settings.soundVolume:
                SettingsConfig.SetVolume(audioMixer, "SoundVol", data);
                break;
            case Settings.ambientVolume:
                SettingsConfig.SetVolume(audioMixer, "AmbientVol", data);
                break;
            case Settings.textSpeed:
                SettingsConfig.changeTextSpeed(PanelsCanvas.GetComponent<Writer>(), data);
                break;
        }
    }

    private void InitSettingsOnStart()
    {
        foreach (Settings setting in (Settings[])Enum.GetValues(typeof(Settings)))
        {
            SettingsOptions value = SettingsConfig.chosenOptions[setting].settingsOption;

            float data = SettingsConfig.chosenOptions[setting].data;

            if (setting == Settings.masterVolume)
            {
                SettingsConfig.SetVolume(audioMixer, "MasterVol", 0);
                float db_vol = (1 - data / 100) * (-80);
                StartCoroutine(FadeVolume(audioMixer, "MasterVol", introMusicFadeSpeed, db_vol));
            }
            else
            {
                ApplySetting(setting, value, data);
            }
        }
    }

    private static IEnumerator FadeVolume(AudioMixer mixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        mixer.GetFloat(exposedParam, out currentVol);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetVolume, currentTime / duration);
            mixer.SetFloat(exposedParam, newVol);
            yield return null;
        }
        yield break;
    }
}