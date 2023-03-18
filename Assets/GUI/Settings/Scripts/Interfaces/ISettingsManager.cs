public interface ISettingsManager
{
    void OpenSettings();
    void CloseSettings();
    void ApplySetting(Settings setting, SettingsOptions value, float data);
}
