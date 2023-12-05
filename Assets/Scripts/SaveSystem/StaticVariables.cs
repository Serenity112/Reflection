public static class StaticVariables
{
    // Статусы
    public static bool PAUSED = false;
    public static bool WARNING_PANEL = false;
    public static bool CONFIRM_PANEL = false;
    public static bool OVER_UI = false;

    // Дауснкие статусы
    public static bool PAUSED_ALLOW_BUTTON = false;
    public static bool IN_SETTINGS_MENU = false;

    // В главном ли меню игра
    public static bool ifInMainMenu { get; set; } = true;

    // Подгрузка UI системы из-за сейвов
    public static bool UIsystemDown { get; set; } = false;

    // Для переноса данных о загрузке между главным меню и игрой
    public static int StartingLoadSaveFile { get; set; } = -1; // -1 = Начать новую игру

    // Наличие оверлей панелький
    public static bool OverlayPanelActive { get; set; } = false;

    // ...
    public static bool GameIsStarting { get; set; } = true;

    // Анимации кнопки "Продолжить"
    public enum MMContinueButtonState
    {
        ButtonDeleted,
        HideAnimation,
        AppearAnimation,
        ButtonAdded,
    }

    public static MMContinueButtonState MainMenuContinueButtonAnimationTrigger { get; set; } = MMContinueButtonState.ButtonDeleted;
}
