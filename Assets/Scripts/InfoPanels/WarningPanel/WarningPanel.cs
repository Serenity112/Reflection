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
      "� ����� � ������� ���������� ����, ��������� ���������� ����� ����\n" +
      "������� ��� ��������� �� ���������\n" +
      "�������� ���������.";

    public static string MemoryLeak =
      "��������� ������ ������!\n" +
      "���� ����� �������� �����������. ���������� ������������� ����.\n" +
      "�������� ���������.";

    public static string SaveSystemCorrupt =
      "� ������� ���������� �������� ������!\n" +
      "��������� �������� ���� ����� ���� �������� �/��� �������� �����������.\n" +
      "���� �� �� ����������������� � ������� ����, ������ ��� �������� �� ������ �������������.\n" +
      "�������� ��������� �� ������������ ����������! (��� �����)";

    public static string AssetLoading =
     "������ �������� �������������� ������.\n" +
     "��������� ����������� ������ ����";

    public static string VersionMissing =
     "�� ������ ���� ������ (��� �� ��������)\n" +
     "���� �� ��� �������/������������� - �������. ���� ��� - ������� � ���� � ���������.";
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

        AddText($"{Warnings[template]}\n\n��� ������: {errorCode}");
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
