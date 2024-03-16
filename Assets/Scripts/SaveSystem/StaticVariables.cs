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


    

    // Игра запущена в 1й раз в рамках текущей сессии
    public static bool GameLaunchedFirstTime { get; set; } = true;
}
