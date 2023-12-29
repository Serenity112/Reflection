public static class StaticVariables
{
    // Игра на паузе
    public static bool PAUSED { get; set; } = false;

    //
    public static bool WARNING_PANEL { get; set; } = false;

    //
    public static bool CONFIRM_PANEL { get; set; } = false;

    // Открыто какое-то визуальное UI меню, пока оно не закроается, другие открывать нельзя
    public static bool OVERLAY_UI_OPENED { get; set; } = false;

    //
    public static bool SPRITE_LOADING { get; set; } = false;

    //
    public static bool SPRITE_MOVING { get; set; } = false;

    // Дауснкие статусы
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
