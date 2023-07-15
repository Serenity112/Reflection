public interface ISettingsManager
{
    void OpenSettings();
    void CloseSettings();

    // ������ ��������� �������� ������ �� ������� ��������� ���������. �����-�� ������������, �����-�� ��������� � ������ ��������.
    void InstantApplySpecificSetting(Settings setting, SettingsOptions value, float data);
}
