using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static WarningPanelMessages;

public static class WarningPanelMessages
{
    public enum WarningTemplate
    {
        PatchRelease,
        MemoryLeak,
        SaveSystemCorrupt,
        AssetLoading,
        VersionMissing
    }

    public static Dictionary<WarningTemplate, string> Warnings;

    static WarningPanelMessages()
    {
        Warnings = new()
        {
            { WarningTemplate.PatchRelease, PatchRelease },
            { WarningTemplate.MemoryLeak, MemoryLeak },
            { WarningTemplate.SaveSystemCorrupt, SaveSystemCorrupt },
            { WarningTemplate.AssetLoading, AssetLoading },
            { WarningTemplate.VersionMissing, VersionMissing },
        };
    }

    public static string PatchRelease =
      "В связи с выходом обновления игры, некоторые сохранения могут быть\n" +
      "Удалены или отброшены по прогрессу\n" +
      "Приносим извинения.";

    public static string MemoryLeak =
      "Произошла утечка памяти!\n" +
      "Игра может работать нестабильно. Попробуйте перезагрузить игру.\n" +
      "Приносим извинения.";

    public static string SaveSystemCorrupt =
      "В системе сохранений возникла ошибка!\n" +
      "Некоторые элементы игры могут быть сброшены и/или работать нестабильно.\n" +
      "Если вы не взаимодействовали с файлами игры, просим вас сообщить об ошибке разработчикам.\n" +
      "Приносим извинения за доставленные неудобства! (нам похуй)";

    public static string AssetLoading =
     "Ошибка загрузки внутриигрового ассета.\n" +
     "Проверьте целостность файлов игры";

    public static string VersionMissing =
     "Не найден файл версии (или он повреждён)\n" +
     "Если вы его удалили/редактировали - верните. Если нет - сообщие о баге в поддержку.";
}

public class WarningPanel : MonoBehaviour
{
    public static WarningPanel instance = null;

    public static bool WARNING_PANEL_ACTIVE { get; set; } = false;

    private GameObject _panel;
    [SerializeField] private Text text;
    [SerializeField] private WarningPanelButton button;

    private void Awake()
    {
        instance = this;

        _panel = transform.GetChild(0).gameObject;
    }

    private void SetText(string text)
    {
        this.text.text = text;
    }

    private void AddText(string text)
    {
        if (!string.IsNullOrEmpty(this.text.text))
        {
            this.text.text += "\n\n";
        }
        this.text.text += text;
    }

    public void CreateWarningMessage(WarningTemplate template, string errorCode)
    {
        FadeManager.FadeObject(_panel, true);

        if (!WARNING_PANEL_ACTIVE)
        {
            ResetPanelState();
        }

        WARNING_PANEL_ACTIVE = true;  

        AddText($"{Warnings[template]}\n\nКод ошибки: {errorCode}");
    }

    public void CloseWarningPanel()
    {
        SetText("");
        WARNING_PANEL_ACTIVE = false; 
        ResetPanelState();
        FadeManager.FadeObject(_panel, false);    
    }

    private void ResetPanelState()
    {
        SetText("");
        button.ResetButtonState();
    }
}
