using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static WarningPanelMessages;

public static class WarningPanelMessages
{
    public enum WarningTemplate
    {
        PatchRelease,
        MemoryLeak,
        SaveSystemCorrupt,
        AssetLoading
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
        };
    }

    public static string PatchRelease =
      "В связи с выходом обновления игры, некоторые сохранения могут быть\n" +
      "Удалены или отброшены по прогрессу\n" +
      "Приносим извинения.";

    public static string MemoryLeak =
      "Произошла утечка памяти!\n" +
      "Игра может работать нестабильно. Стабильное решение - перезагрузка игры.\n" +
      "Приносим извинения.";

    public static string SaveSystemCorrupt =
      "В системе сохранений возникла ошибка!\n" +
      "Некоторые элементы игры могут быть сброшены и/или работать нестабильно.\n" +
      "Если вы не взаимодействовали с файлами игры, просим вас сообщить об ошибке разработчикам.\n" +
      "Приносим извинения за доставленные неудобства! (нам похуй если честно)";

    public static string AssetLoading =
     "Ошибка загрузки внутриигрового ассета.\n" +
     "Проверьте целостность файлов игры";
}

public class WarningPanel : MonoBehaviour
{
    public static WarningPanel instance = null;
    public static bool WARNING_PANEL_ACTIVE { get; set; } = false;

    private AsyncOperationHandle<GameObject> handler;

    private Queue<(WarningTemplate, string)> WarningMessagesQueue = new();

    private GameObject Panel;

    private WarningPanelButton Button;

    private void Awake()
    {
        instance = this;

        Panel = transform.GetChild(0).gameObject;
        Button = Panel.transform.GetChild(0).GetChild(0).GetComponent<WarningPanelButton>();
    }

    public void ResetPanelState()
    {
        SetText("");
        Button.ResetButtonState();
    }

    private void SetText(string text)
    {
        Panel.transform.GetChild(1).GetComponent<Text>().text = text;
    }

    public void CreateWarningMessage(WarningTemplate template, string errorCode)
    {
        FadeManager.FadeObject(Panel, true);

        WarningMessagesQueue.Enqueue((template, errorCode));

        if (!WARNING_PANEL_ACTIVE)
        {
            ProcessQueue();
        }

        WARNING_PANEL_ACTIVE = true;
    }

    private void ProcessQueue()
    {
        if (WarningMessagesQueue.Count > 0)
        {
            var warning = WarningMessagesQueue.Dequeue();
            SetText($"{Warnings[warning.Item1]}\n\nКоды ошибки: {warning.Item2}");
        }
    }

    public void CloseWarningPanel()
    {
        SetText("");

        if (WarningMessagesQueue.Count > 0)
        {
            ProcessQueue();
        }
        else
        {
            ResetPanelState();
            WARNING_PANEL_ACTIVE = false;
            FadeManager.FadeObject(Panel, false);
        }
    }

    public void ReleasePanel()
    {
        Addressables.ReleaseInstance(handler);

        Resources.UnloadUnusedAssets();
    }
}
