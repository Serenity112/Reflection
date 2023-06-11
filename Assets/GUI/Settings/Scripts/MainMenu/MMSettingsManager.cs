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
    private AudioMixer AudioMixer;

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

    private void OnEnable()
    {
        ActivePanels = PanelsConfig.CurrentManager.GetActivePanelsParent();

        BlackPanel = PanelsConfig.CurrentManager.GetBlackPanel();
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
        StartCoroutine(ICloseSettings());
    }

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

        Addressables.ReleaseInstance(handler);

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));

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
                SettingsConfig.ChangeBlackFramesMode(MainCanvas, value);
                break;
            case Settings.Resolution:
                SettingsConfig.ChangeResoulution(value);
                break;
            case Settings.masterVolume:
                SettingsConfig.SetVolume(AudioMixer, "MasterVol", data / 100); // Деление на 100 т.к. метод принимает числа от 0 до 1, а сейвах идут проценты от 0 до 100
                break;
            case Settings.musicVolume:
                SettingsConfig.SetVolume(AudioMixer, "MusicVol", data / 100);
                break;
            case Settings.soundVolume:
                SettingsConfig.SetVolume(AudioMixer, "SoundVol", data / 100);
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
                AudioMixer.SetFloat("MasterVol", 0);
                float targetVolume_db = Mathf.Log10(data / 100) * 20;
                StartCoroutine(FadeVolume(AudioMixer, "MasterVol", 3f, targetVolume_db));
            }
            else
            {
                ApplySetting(setting, value, data);
            }
        }
    }

    private static IEnumerator FadeVolume(AudioMixer mixer, string exposedParam, float duration, float targetVolumeDB)
    {
        float currentTime = 0;
        mixer.GetFloat(exposedParam, out float currentVol);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetVolumeDB, currentTime / duration);
            mixer.SetFloat(exposedParam, newVol);
            yield return null;
        }
        yield break;
    }
}
