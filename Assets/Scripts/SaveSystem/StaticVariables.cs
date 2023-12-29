public static class StaticVariables
{
    // ���� �� �����
    public static bool PAUSED { get; set; } = false;

    //
    public static bool WARNING_PANEL { get; set; } = false;

    //
    public static bool CONFIRM_PANEL { get; set; } = false;

    // ������� �����-�� ���������� UI ����, ���� ��� �� ����������, ������ ��������� ������
    public static bool OVERLAY_UI_OPENED { get; set; } = false;

    //
    public static bool SPRITE_LOADING { get; set; } = false;

    //
    public static bool SPRITE_MOVING { get; set; } = false;

    // �������� �������
    public static bool PAUSE_ANIM_ENDED { get; set; } = false;
    public static bool IN_SETTINGS_MENU { get; set; } = false;
    public static bool IN_SAVE_MENU { get; set; } = false;
    public static bool GAME_LOADING { get; set; } = false;

    public static void ResetFlags()
    {
        PAUSED = false;
        WARNING_PANEL = false;
        CONFIRM_PANEL = false;
        OVERLAY_UI_OPENED = false;
        PAUSE_ANIM_ENDED = false;
        IN_SETTINGS_MENU = false;
        IN_SAVE_MENU = false;
    }

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
