using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SettingsManager : MonoBehaviour, ISettingsManager
{
    public static SettingsManager instance = null;

    private GameObject GameCanvas;

    private GameObject PanelsCanvas;

    private GameObject ActivePanels;

    private GameObject blackPanelGame;

    private GameObject blackPanelPanels;

    [SerializeField]
    private AudioMixer audioMixer;

    private Camera PanelsCamera;

    private float speed = 5f;

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
        GameCanvas = PanelsManager.instance.GameCanvas;
        PanelsCanvas = PanelsManager.instance.PanelsCanvas;
        ActivePanels = PanelsManager.instance.ActivePanels;
        blackPanelGame = PanelsManager.instance.BlackPanel;
        blackPanelPanels = PanelsManager.instance.blackPanelPanels;
        PanelsCamera = PanelsManager.instance.PanelsCamera;

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

        settingsPanelHandler = Addressables.InstantiateAsync("SettingsGuiPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return settingsPanelHandler;

        if (settingsPanelHandler.Status == AsyncOperationStatus.Succeeded)
        {
            settingsPanelHandler.Result.name = "SettingsGui";
            PanelsCamera.enabled = true;
        }
        else
        {
            Debug.Log("Error loading");
        }

        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, false, speed));
        FadeManager.FadeObject(blackPanelGame, false);
    }
    private IEnumerator ICloseSettings()
    {
        FadeManager.FadeObject(blackPanelGame, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, speed));

        Addressables.ReleaseInstance(settingsPanelHandler);
        PanelsCamera.enabled = false;

        PauseButtonsManager.instance.EnableButtons();
        PauseButtonsManager.instance.UnSelectButtons();

        yield return StartCoroutine(FadeManager.FadeObject(blackPanelGame, false, speed));
        FadeManager.FadeObject(blackPanelPanels, false);

        Resources.UnloadUnusedAssets();
    }

    // Не все настрйоки нужно просто включить. Например музыку надо плавно вцвести
    private void ApplySettingsOnStart()
    {
        audioMixer.SetFloat("MasterVol", 0);
        audioMixer.SetFloat("MusicVol", 0);
        audioMixer.SetFloat("SoundVol", 0);

        foreach (Settings setting in (Settings[])Enum.GetValues(typeof(Settings)))
        {
            SettingsOptions value = SettingsConfig.chosenOptions[setting].settingsOption;
            float data = SettingsConfig.chosenOptions[setting].data;

            switch (setting)
            {
                case Settings.masterVolume:
                    SmoothMusicOnStart("MasterVol", data);
                    break;
                case Settings.musicVolume:
                    SmoothMusicOnStart("MusicVol", data);
                    break;
                case Settings.soundVolume:
                    SmoothMusicOnStart("SoundVol", data);
                    break;
                case Settings.FullScreenMode:
                    // Не применять, переносится с мейн меню
                    break;
                case Settings.Resolution:
                    // Не применять, переносится с мейн меню
                    break;
                default:
                    InstantApplySpecificSetting(setting, value, data);
                    break;
            }
        }
    }


    public void InstantApplySpecificSetting(Settings setting, SettingsOptions value, float data)
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
                SettingsConfig.SetVolume(audioMixer, "MasterVol", data / 100); // Деление на 100 т.к. метод принимает числа от 0 до 1
                break;
            case Settings.musicVolume:
                SettingsConfig.SetVolume(audioMixer, "MusicVol", data / 100);
                break;
            case Settings.soundVolume:
                SettingsConfig.SetVolume(audioMixer, "SoundVol", data / 100);
                break;
            case Settings.ambientVolume:
                SettingsConfig.SetVolume(audioMixer, "AmbientVol", data / 100);
                break;
            case Settings.TextSpeed:
                SettingsConfig.ChangeTextSpeed(Typewriter.Instance, data);
                break;
            case Settings.Language:
                break;
            case Settings.SpriteExpand:
                if (data == 1)
                {
                    //SpriteController.instance.UnExpandAllSprites();
                }
                else
                {

                }
                break;
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
