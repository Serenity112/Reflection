public static class StaticVariables
{
    // �������
    public static bool PAUSED = false;
    public static bool WARNING_PANEL = false;
    public static bool CONFIRM_PANEL = false;
    public static bool OVER_UI = false;

    // �������� �������
    public static bool PAUSED_ALLOW_BUTTON = false;
    public static bool IN_SETTINGS_MENU = false;

    // � ������� �� ���� ����
    public static bool ifInMainMenu { get; set; } = true;

    // ��������� UI ������� ��-�� ������
    public static bool UIsystemDown { get; set; } = false;

    // ��� �������� ������ � �������� ����� ������� ���� � �����
    public static int StartingLoadSaveFile { get; set; } = -1; // -1 = ������ ����� ����

    // ������� ������� ���������
    public static bool OverlayPanelActive { get; set; } = false;

    // ...
    public static bool GameIsStarting { get; set; } = true;

    // �������� ������ "����������"
    public enum MMContinueButtonState
    {
        ButtonDeleted,
        HideAnimation,
        AppearAnimation,
        ButtonAdded,
    }

    public static MMContinueButtonState MainMenuContinueButtonAnimationTrigger { get; set; } = MMContinueButtonState.ButtonDeleted;
}
