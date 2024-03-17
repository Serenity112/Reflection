public static class SaveManagerStatic
{
    // В главном ли меню игра
    public static bool ifInMainMenu { get; set; } = true;

    // Подгрузка UI системы из-за сейвов
    public static bool ClickBlocker { get; set; } = false;

    // Для блока каких-то триггеров, при выполнении анимаций
    public static bool UiBloker { get; set; } = false;

    // Некоторые действия выполняются с задержкой, чтобы не ломался ui поток
    public static bool DelayedSaveAction { get; set; } = false;

    // Для переноса данных о загрузке между главным меню и игрой
    public static int StartingLoadSaveFile { get; set; } = -1; // -1 = Начать новую игру
}
