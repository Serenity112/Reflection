public static class StaticVariables
{
    public static bool ifInMainMenu { get; set; } = true;

    public static bool UIsystemDown { get; set; } = false;

    public static int StartingLoadSaveFile { get; set; } = -1; // -1 = ������ ����� ����

    public static bool OverlayPanelActive { get; set; } = false;

    public static bool GameIsStarting { get; set; } = true;

    public enum MMContinueButtonState
    {
        ButtonDeleted,
        HideAnimation,
        AppearAnimation,
        ButtonAdded,
    }

    public static MMContinueButtonState MainMenuContinueButtonAnimationTrigger { get; set; } = MMContinueButtonState.ButtonDeleted;
}
