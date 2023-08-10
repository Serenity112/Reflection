using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
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

        ApplySettingsOnStart();
    }

    public void OpenSettings()
    {
        StartCoroutine(IOpenSettings());
    }

    public void CloseSettings()
    {
        PauseButtonsManager.instance.UnSelectButtons();

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

        PauseButtonsManager.instance.EnableButtons();
        PauseButtonsManager.instance.UnSelectButtons();

        yield return StartCoroutine(FadeManager.FadeObject(blackPanelGame, false, settingsSpeed));
        FadeManager.FadeObject(blackPanelPanels, false);

        Resources.UnloadUnusedAssets();
    }

    // �� ��� ��������� ����� ������ ��������. �������� ������ ���� ������ �������
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
                    // �� ���������, ����������� � ���� ����
                    break;
                case Settings.Resolution:
                    // �� ���������, ����������� � ���� ����
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
                SettingsConfig.SetVolume(audioMixer, "MasterVol", data / 100); // ������� �� 100 �.�. ����� ��������� ����� �� 0 �� 1
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
