using Fungus;
using System;
using System.IO;
using UnityEngine;

public class SaveSystemUtils : MonoBehaviour
{
    // Alpha - �������� ���� ������ �����
    // Demo - ������ ����� �����
    // Release - �����-�� ����� ���� ���
    public const string GAME_VERSION = "demo:1";

    #region SaveFiles

    // ���� ��������
    public static string GlobalSettings { get; set; } = "GlobalSettings.es3";

    // �������������� ��������� ����������. ����������� �������, ���
    public static string GameData { get; set; } = "GameData.es3";

    // ��� ���������� ���� �����
    public static string SaveFileSingleName { get; set; } = "SaveFile";

    // ����� �� �����������
    public static string ScreenshotsFolder { get; set; } = "ScreenShots";

    //
    public static string SaveFilesFolder { get; set; } = "SaveFiles";

    #endregion


    private void Start()
    {
        GameVersionIsOk();
    }

    private void GameVersionIsOk()
    {
        string actual_ver_name = GAME_VERSION.Split(":")[0];
        string actual_ver_num = GAME_VERSION.Split(":")[1];

        // ���� ����� - ������� ��� �����. ��� ����� ��� ������, ��� ������ ����� ������ ������ ���� �������
        if (actual_ver_name == "alpha")
        {
            DumpAllData();
        }

        if (ES3.FileExists("version.es3"))
        {
            try
            {
                string version = ES3.Load<string>("ver", "version.es3");

                string file_ver_name = version.Split(":")[0];
                string file_ver_num = version.Split(":")[1];

                ES3.Save<string>("ver", GAME_VERSION, "version.es3");


                // �������� ��������� ����, ���� ������ ���������� � ������ ����
            }
            catch
            {
                RaiseVersionError();
            }
        }
        // ���� ������ �����������
        else
        {
            // ���� ����� ������ ���, �� ������ ����� ����
            if (ES3.GetFiles().Length > 0 || ES3.GetDirectories().Length > 0)
            {
                RaiseVersionError();
            }

            ES3.Save<string>("ver", GAME_VERSION, "version.es3");
        }

        void RaiseVersionError()
        {
            WarningPanel.instance.CreateWarningMessage(WarningPanelMessages.WarningTemplate.VersionMissing, $"���������� ������ {GAME_VERSION}");
        }
    }

    // �������� ������
    public static void DumpFile(string fileName)
    {
        if (ES3.FileExists(fileName))
        {
            ES3.DeleteFile(fileName);
        }
    }

    public static void DumpFolder(string folderName)
    {
        if (ES3.FileExists(folderName))
        {
            ES3.DeleteFile(folderName);
        }
    }

    public static void DumpAllData()
    {
        foreach (var file in ES3.GetFiles())
        {
            ES3.DeleteFile(file);
        }

        foreach (var dir in ES3.GetDirectories())
        {
            ES3.DeleteDirectory(dir);
        }
    }

    // ��������������, � ������� ������
    public static void FileRecovery(string fileName, string path)
    {
        // �������� ��������� ������ ������. ���� ������� �������������� ���������, ����� ������� ��� ����� ��� ����
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
