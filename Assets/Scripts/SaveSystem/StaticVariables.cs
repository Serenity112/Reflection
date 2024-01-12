public static class StaticVariables
{
    // Навигационные статусы
    public static bool PAUSED =>
        PauseButtonsManager.GAME_IS_PAUSED;

    public static bool IN_SETTINGS_MENU { get; set; } = false;
    public static bool IN_SAVE_MENU { get; set; } = false;


    public static bool SPRITE_MOVING { get; set; } = false;

    // Вспомогательные статусы (а.к.а. анимации)


    // Идёт какая-то прогрузка
    public static bool GAME_IS_LOADING =>
        SpriteApearer.SPRITE_LOADING ||
        SpritesSwapper.SPRITE_LOADING ||
        PanelsManager.GAME_LOADING;

    // Присутствует любой оверлей
    public static bool OVERLAY_ACTIVE =>
       WarningPanel.WARNING_PANEL_ACTIVE ||
       ConfirmationPanel.CONFIRM_PANEL_ACTIVE ||
       HideButton.UI_HIDDEN ||
       LogManager.LOG_PANEL_ACTIVE;




    //___________________________________________________________________________________________


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
