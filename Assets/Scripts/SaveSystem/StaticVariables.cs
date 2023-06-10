public static class StaticVariables
{
    public static bool ifInMainMenu { get; set; } = false;

    public static bool UIsystemDown { get; set; } = false;

    public static int StartingLoadSaveFile { get; set; } = -1; // -1 = Начать новую игру

    public static bool OverlayPanelActive { get; set; } = false;

    public enum MMContinueButtonState
    {
        ButtonDeleted,
        HideAnimation,
        AppearAnimation,
        ButtonAdded,
    }
    public static MMContinueButtonState MainMenuContinueButtonAnimationTrigger { get; set; } = MMContinueButtonState.ButtonDeleted;
}
