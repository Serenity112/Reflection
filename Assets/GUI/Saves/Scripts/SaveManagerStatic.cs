public static class SaveManagerStatic
{
    // � ������� �� ���� ����
    public static bool ifInMainMenu { get; set; } = true;

    // ��������� UI ������� ��-�� ������
    public static bool ClickBlocker { get; set; } = false;

    public static bool UiBloker { get; set; } = false;

    // ��� �������� ������ � �������� ����� ������� ���� � �����
    public static int StartingLoadSaveFile { get; set; } = -1; // -1 = ������ ����� ����

    // ������� ������� ���������
    public static bool OverlayPanelActive { get; set; } = false;
}
