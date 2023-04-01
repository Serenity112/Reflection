public interface ISettingsManager
{
    void OpenSettings();
    void CloseSettings();

    // Каждый меденджер настроек должен по разному применять настройки. Какие-то игнорировать, какие-то применять к многим объектам.
    void ApplySetting(Settings setting, SettingsOptions value, float data);
}
