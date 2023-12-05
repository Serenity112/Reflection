public interface ISettingsManager
{
    void OpenSettings();
    void CloseSettings();

    // ������ ��������� �������� ������ �� ������� ��������� ���������. �����-�� ������������, �����-�� ��������� � ������ ��������.
    void InstantApplySpecificSetting(SettingsList setting, SettingsOptions value, float data);
}
