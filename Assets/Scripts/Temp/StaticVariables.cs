public static class StaticVariables
{
    public static bool ifInMainMenu { get; set; } = false;

    public static bool UIsystemDown { get; set; } = false;

    public static int StartingLoadSaveFile { get; set; } = -1;

    public static int MainMenuContinueButtonAnimationTrigger { get; set; } = -1; // -1 ������ ����� ����, 0 - �������� �������� ����������, 1 �������� ��������� ����������, 2 - ��������� ����������
}
