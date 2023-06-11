using System.IO;
using UnityEngine;

public static class SaveSystemUtils
{
    public static string GlobalSettings = "GlobalSettings.es3";
    public static string SaveFilesData = "SaveFilesData.es3";

    public static string SaveFileName = "SaveFile";
    public static string ScreenshotsFolder = "ScreenShots";
    public static string SaveFilesFolder = "SaveFiles";

    public static void InitialFilesCheck()
    {

    }

    public static void FileRecovery(string fileName, string path)
    {
        // �������� ��������� ������ ������. ���� ������� �������������� ���������, ����� ������� ��� ����� ��� ����
    }

    // ����� ������ ���� ����������. ���� ��� ���������� ��� ��� ����� �������� �����
    public static void DumpAllData()
    {
        DirectoryInfo dataDir = new DirectoryInfo(Application.persistentDataPath);
        dataDir.Delete(true);
    }

    public static void CreateBackup()
    {
        ES3.CreateBackup(GlobalSettings);
    }

    public static void RestoreBackup()
    {
        ES3.RestoreBackup(GlobalSettings);
    }
}
