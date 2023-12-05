public interface ISettingsManager
{
    void OpenSettings();
    void CloseSettings();

    // Каждый меденджер настроек должен по разному применять настройки. Какие-то игнорировать, какие-то применять к многим объектам.
    void InstantApplySpecificSetting(SettingsList setting, SettingsOptions value, float data);
}
