public static class StaticVariables
{
    // ������������� �������
    public static bool PAUSED =>
        PauseButtonsManager.GAME_IS_PAUSED;

    public static bool IN_SETTINGS_MENU { get; set; } = false;
    public static bool IN_SAVE_MENU { get; set; } = false;


    public static bool SPRITE_MOVING { get; set; } = false;

    // ��������������� ������� (�.�.�. ��������)


    // ��� �����-�� ���������
    public static bool GAME_IS_LOADING =>
        SpriteApearer.SPRITE_LOADING ||
        SpritesSwapper.SPRITE_LOADING ||
        PanelsManager.GAME_LOADING;

    // ������������ ����� �������
    public static bool OVERLAY_ACTIVE =>
       WarningPanel.WARNING_PANEL_ACTIVE ||
       ConfirmationPanel.CONFIRM_PANEL_ACTIVE ||
       HideButton.UI_HIDDEN ||
       LogManager.LOG_PANEL_ACTIVE;




    //___________________________________________________________________________________________


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
