using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum SettingsList
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

    None,
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

    // Заглушки
    None,
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
    public static Dictionary<SettingsList, SettingsOptionsData> chosenOptions = new Dictionary<SettingsList, SettingsOptionsData>();

    public static List<(SettingsList, ISettingsOptions)> options = new List<(SettingsList, ISettingsOptions)>();

    public static ISettingsManager currentManager { get; set; }

    public static float spacingSpeed { get; set; } = 5f;

    public static float sliderGuiSpeed { get; set; } = 5f;

    public static bool skipEverything
    {
        get
        {
            return chosenOptions[SettingsList.DialogSkipMode].settingsOption == SettingsOptions.SkipEveryting;
        }

        set
        {
            skipEverything = value;
        }
    }

    public static void LoadSettingsFromMemory()
    {
        LoadDefaultSettings();

        LoadUserSettings();
    }

    public static SettingsOptions GetChosenOption(SettingsList setting)
    {
        return chosenOptions[setting].settingsOption;
    }

    // Установка настроек по умолчанию
    private static void LoadDefaultSettings()
    {
        chosenOptions.Clear();

        //Общие
        chosenOptions.Add(SettingsList.FullScreenMode, new SettingsOptionsData(SettingsOptions.FullScreen, 0));
        chosenOptions.Add(SettingsList.BlackFramesMode, new SettingsOptionsData(SettingsOptions.BlackLines, 0));
        chosenOptions.Add(SettingsList.DialogSkipMode, new SettingsOptionsData(SettingsOptions.SkipEveryting, 0));
        chosenOptions.Add(SettingsList.Resolution, new SettingsOptionsData(SettingsOptions.rAutomatic, 0));

        // Миксер - глобальные параметры (0 - 100) Изменяются вручную из настроек игры
        chosenOptions.Add(SettingsList.masterVolume, new SettingsOptionsData(SettingsOptions.Volume, 50));
        chosenOptions.Add(SettingsList.musicVolume, new SettingsOptionsData(SettingsOptions.Volume, 50));
        chosenOptions.Add(SettingsList.soundVolume, new SettingsOptionsData(SettingsOptions.Volume, 50));
        chosenOptions.Add(SettingsList.ambientVolume, new SettingsOptionsData(SettingsOptions.Volume, 50));

        // Скорость текста
        chosenOptions.Add(SettingsList.TextSpeed, new SettingsOptionsData(SettingsOptions.Speed, 100));

        // Увеличениек спрайтов
        chosenOptions.Add(SettingsList.SpriteExpand, new SettingsOptionsData(SettingsOptions.Expand, 1)); // 1 - true

        // Анимация фонов
        chosenOptions.Add(SettingsList.GuiAnimation, new SettingsOptionsData(SettingsOptions.GuiAnimation, 1)); // 1 - true

        // Язык
        chosenOptions.Add(SettingsList.Language, new SettingsOptionsData(SettingsOptions.Russian, 1));

        // Заглушки
        chosenOptions.Add(SettingsList.None, new SettingsOptionsData(SettingsOptions.None, 1));
    }

    // Загрузка настроек пользователя поверх стандартных
    private static void LoadUserSettings()
    {
        try
        {
            foreach (SettingsList setting in (SettingsList[])Enum.GetValues(typeof(SettingsList)))
            {
                if (ES3.FileExists(SaveSystemUtils.GlobalSettings) && ES3.KeyExists(setting.ToString(), SaveSystemUtils.GlobalSettings))
                {
                    SettingsOptionsData optiondata = ES3.Load<SettingsOptionsData>(setting.ToString(), SaveSystemUtils.GlobalSettings);
                    chosenOptions[setting] = optiondata;
                }
            }
        }
        catch (Exception ex)
        {
            WarningPanel.instance.CreateWarningMessage(WarningPanelMessages.WarningTemplate.SaveSystemCorrupt, $"key: {ex.Message} folder: {SaveSystemUtils.GlobalSettings}");
            //SaveSystemUtils.DumpFile(SaveSystemUtils.GlobalSettings);
        }
    }

    // Сохранение настроек в список и файл сохранений
    public static void SaveOption(SettingsList setting, SettingsOptions value)
    {
        SaveOption(setting, value, 0);
    }

    public static void SaveOption(SettingsList setting, SettingsOptions value, float data)
    {
        try
        {
            ES3.Save<SettingsOptionsData>(setting.ToString(), new SettingsOptionsData(value, data), SaveSystemUtils.GlobalSettings);

            if (chosenOptions.ContainsKey(setting))
            {
                chosenOptions[setting] = new SettingsOptionsData(value, data);
            }
        }
        catch (Exception)
        {

        }
    }

    public static void CheckLinkedOptions(SettingsList setting, SettingsOptions value)
    {
        if (setting == SettingsList.FullScreenMode && value == SettingsOptions.FullScreen)
        {
            SaveOption(SettingsList.Resolution, SettingsOptions.rAutomatic);
            UpdateGroupVisuals(SettingsList.Resolution);
        }

        if (setting == SettingsList.Resolution && value != chosenOptions[SettingsList.Resolution].settingsOption)
        {
            SaveOption(SettingsList.FullScreenMode, SettingsOptions.WindowsScreen);
            UpdateGroupVisuals(SettingsList.FullScreenMode);
        }
    }

    public static void ApplySetting(SettingsList setting, SettingsOptions value, float data = 0)
    {
        currentManager.InstantApplySpecificSetting(setting, value, data);
    }

    public static bool isOptionEnabled(SettingsList setting, SettingsOptions option)
    {
        return chosenOptions[setting].settingsOption == option;
    }

    public static void subscribeOption(SettingsList key, ISettingsOptions option)
    {
        options.Add((key, option));
    }

    public static void unSubscribeAllOptions()
    {
        options.Clear();
    }

    public static void UpdateGroupVisuals(SettingsList settingGroup)
    {
        foreach (var setting in options)
        {
            if (setting.Item1 == settingGroup)
            {
                setting.Item2.UpdateVisuals();
            }
        }
    }

    public static void InitialUpdateAllVisuals()
    {
        foreach (var setting in options)
        {
            setting.Item2.InitialUpdateVisuals();
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
                if (GetChosenOption(SettingsList.FullScreenMode) != SettingsOptions.FullScreen)
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
        int coef = 5;
        float dbVolume = coef * Mathf.Log10(linearVolume);

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

    public static bool IfAllowExpandings()
    {
        return chosenOptions[SettingsList.SpriteExpand].data == 1;
    }
}
