using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManagerStatic
{
    // В главном ли меню игра
    public static bool ifInMainMenu { get; set; } = true;

    // Подгрузка UI системы из-за сейвов
    public static bool UIsystemDown { get; set; } = false;

    // Для переноса данных о загрузке между главным меню и игрой
    public static int StartingLoadSaveFile { get; set; } = -1; // -1 = Начать новую игру

    // Наличие оверлей панелький
    public static bool OverlayPanelActive { get; set; } = false;
}
