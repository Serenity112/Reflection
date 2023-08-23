using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Fungus;

public enum Settings
{
    FullScreenMode,
    BlackFramesMode,
    DialogSkipMode,
    Resolution,

    masterVolume,
    musicVolume,
    ambientVolume,
    soundVolume,

    TextSpeed,
    SpriteExpand,
    GuiAnimation,
    Language,
}

public enum SettingsOptions
{
    FullScreen,
    WindowsScreen,

    BlackLines,
    Zoom,

    SkipEveryting,
    SkipRead,

    r2560x1440,
    r1920x1080,
    r1280x720,
    r832x480,
    rAutomatic,

    // Заглушки
    Volume,
    Speed,
    Expand,
    GuiAnimation,

    // Язык
    Russian,
    English,
}

public struct SettingsOptionsData
{
    public SettingsOptionsData(SettingsOptions settingsOption, float data)
    {
        this.settingsOption = settingsOption;
        this.data = data;
    }

    public SettingsOptions settingsOption;

    public float data;
}

public class SettingsConfig
{
    public static Dictionary<Settings, SettingsOptionsData> chosenOptions = new Dictionary<Settings, SettingsOptionsData>();

    public static List<ISettingsOption> options = new List<ISettingsOption>();

    public static ISettingsManager currentManager { get; set; }

    public static float spacingSpeed = 5f;

    public static float sliderGuiSpeed = 5f;

    public static bool skipEverything
    {
        get
        {
            return chosenOptions[Settings.DialogSkipMode].settingsOption == SettingsOptions.SkipEveryting;
        }

        set
        {
            skipEverything = value;
        }
    }

    private static string GlobalSettings;

    public static void LoadSettingsFromMemory()
    {
        GlobalSettings = SaveSystemUtils.GlobalSettings;

        LoadDefaultSettings();

        LoadUserSettings();
    }

    public static SettingsOptions GetChosenOption(Settings setting)
    {
        return chosenOptions[setting].settingsOption;
    }

    private static void LoadDefaultSettings()
    {
        chosenOptions.Clear();

        //Общие
        chosenOptions.Add(Settings.FullScreenMode, new SettingsOptionsData(SettingsOptions.FullScreen, 0));
        chosenOptions.Add(Settings.BlackFramesMode, new SettingsOptionsData(SettingsOptions.BlackLines, 0));
        chosenOptions.Add(Settings.DialogSkipMode, new SettingsOptionsData(SettingsOptions.SkipEveryting, 0));
        chosenOptions.Add(Settings.Resolution, new SettingsOptionsData(SettingsOptions.rAutomatic, 0));

        // Миксер - глобальные параметры (0 - 100) Изменяются вручную из настроек игры
        chosenOptions.Add(Settings.masterVolume, new SettingsOptionsData(SettingsOptions.Volume, 100));
        chosenOptions.Add(Settings.musicVolume, new SettingsOptionsData(SettingsOptions.Volume, 100));
        chosenOptions.Add(Settings.soundVolume, new SettingsOptionsData(SettingsOptions.Volume, 100));
        chosenOptions.Add(Settings.ambientVolume, new SettingsOptionsData(SettingsOptions.Volume, 100));

        // Скорость текста
        chosenOptions.Add(Settings.TextSpeed, new SettingsOptionsData(SettingsOptions.Speed, 100));

        // Увеличениек спрайтов
        chosenOptions.Add(Settings.SpriteExpand, new SettingsOptionsData(SettingsOptions.Expand, 1)); // 1 - true

        // Анимация фонов
        chosenOptions.Add(Settings.GuiAnimation, new SettingsOptionsData(SettingsOptions.GuiAnimation, 1)); // 1 - true

        // Язык
        chosenOptions.Add(Settings.Language, new SettingsOptionsData(SettingsOptions.Russian, 1));
    }

    private static void LoadUserSettings()
    {
        try
        {
            foreach (Settings setting in (Settings[])Enum.GetValues(typeof(Settings)))
            {
                if (ES3.FileExists(GlobalSettings) && ES3.KeyExists(setting.ToString(), GlobalSettings))
                {
                    SettingsOptionsData optiondata = ES3.Load<SettingsOptionsData>(setting.ToString(), GlobalSettings);

                    chosenOptions[setting] = optiondata;
                }
            }
        }
        catch (Exception)
        {
            WarningPanel.instance.CreateWarningPanel(WarningPanel.SavingErrorMessage);
        }
    }

    public static void SaveOptionToFile(Settings setting, SettingsOptions value)
    {
        SaveOptionToFile(setting, value, 0);
    }

    public static void SaveOptionToFile(Settings setting, SettingsOptions value, float data)
    {
        try
        {
            ES3.Save<SettingsOptionsData>(setting.ToString(), new SettingsOptionsData(value, data), GlobalSettings);

            if (chosenOptions.ContainsKey(setting))
            {
                chosenOptions[setting] = new SettingsOptionsData(value, data);
            }
        }
        catch (Exception)
        {
            WarningPanel.instance.CreateWarningPanel(WarningPanel.SavingErrorMessage);
        }
    }

    public static void FilterAndApplySpecificSetting(Settings setting, SettingsOptions value, float data = 0)
    {
        if (setting == Settings.FullScreenMode)
        {
            if (value == SettingsOptions.FullScreen)
            {
                SaveOptionToFile(Settings.Resolution, SettingsOptions.rAutomatic);
            }
        }

        if (setting == Settings.Resolution)
        {
            if (value != chosenOptions[setting].settingsOption)
            {
                SaveOptionToFile(Settings.FullScreenMode, SettingsOptions.WindowsScreen);
            }
        }

        currentManager.InstantApplySpecificSetting(setting, value, data);
    }

    public static bool isOptionEnabled(Settings setting, SettingsOptions option)
    {
        return chosenOptions[setting].settingsOption == option;
    }

    public static void subscribeOption(ISettingsOption option)
    {
        options.Add(option);
    }

    public static void unSubscribeAllOptions()
    {
        options.Clear();
    }

    public static void UpdateAllVisuals()
    {
        foreach (ISettingsOption setting in options)
        {
            setting.UpdateVisuals();
        }
    }

    public static void InitialUpdateAllVisuals()
    {
        foreach (ISettingsOption setting in options)
        {
            setting.InitialUpdateVisuals();
        }
    }

    // Универсальное применение настроек из разных менеджеров
    public static void ChangeFullscreeenMode(SettingsOptions value)
    {
        switch (value)
        {
            case SettingsOptions.FullScreen:
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                break;
            case SettingsOptions.WindowsScreen:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }

    public static void ChangeBlackFramesMode(GameObject canvas, SettingsOptions value)
    {
        switch (value)
        {
            case SettingsOptions.BlackLines:
                canvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

                foreach (Transform child in canvas.transform)
                {
                    RectTransform rect = child.gameObject.GetComponent<RectTransform>();

                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.pivot = new Vector2(0.5f, 0.5f);

                    rect.sizeDelta = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
                }
                break;
            case SettingsOptions.Zoom:
                canvas.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;

                foreach (Transform child in canvas.transform)
                {
                    RectTransform rect = child.gameObject.GetComponent<RectTransform>();

                    rect.anchorMin = new Vector2(0f, 0f);
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.pivot = new Vector2(0.5f, 0.5f);

                    rect.offsetMin = new Vector2(0f, 0f);
                    rect.offsetMax = new Vector2(0f, 0f);

                    rect.anchoredPosition = new Vector2(0f, 0f);
                }
                break;
        }
    }

    [Obsolete]
    public static void ChangeResoulution(SettingsOptions value)
    {
        switch (value)
        {
            case SettingsOptions.r2560x1440:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(2560, 1440, FullScreenMode.Windowed, 144);
                break;
            case SettingsOptions.r1920x1080:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed, 60);
                break;
            case SettingsOptions.r1280x720:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed, 60);
                break;
            case SettingsOptions.r832x480:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Screen.SetResolution(832, 480, FullScreenMode.Windowed, 60);
                break;
            case SettingsOptions.rAutomatic:
                if (GetChosenOption(Settings.FullScreenMode) != SettingsOptions.FullScreen)
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.Windowed, 60);
                }
                break;
        }
    }

    // Принимает громкость от 0 до 1, ставит громоксть миксера в децибельной шкале
    public static void SetVolume(AudioMixer mixer, string exposedparam, float linearVolume)
    {
        float dbVolume = Mathf.Log10(linearVolume) * 20;

        if (linearVolume == 0.0f)
        {
            dbVolume = -80.0f;
        }

        mixer.SetFloat(exposedparam, dbVolume);
    }

    public static void ChangeTextSpeed(Typewriter typewriter, float speed)
    {
        typewriter.SetTextSpeed(speed);
    }
}
