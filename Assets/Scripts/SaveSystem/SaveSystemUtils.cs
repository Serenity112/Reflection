using Fungus;
using System;
using System.IO;
using UnityEngine;

public class SaveSystemUtils : MonoBehaviour
{
    // Alpha - удаление всех сейвов нахуй
    // Demo - просто ветка демки
    // Release - Когда-то через пару лет
    public const string GAME_VERSION = "demo:1";

    #region SaveFiles

    // Файл настроек
    public static string GlobalSettings { get; set; } = "GlobalSettings.es3";

    // Дополнительная рандомная информация. Прочитанные диалоги, итд
    public static string GameData { get; set; } = "GameData.es3";

    // Имя одиночного сейв файла
    public static string SaveFileSingleName { get; set; } = "SaveFile";

    // Папка со скриншотами
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

        // Если альва - очищаем все сейвы. Это нужно для версий, где каждая новая версия меняет сейв систему
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


                // Добавить обработку того, если версия устаревшая в случае чего
            }
            catch
            {
                RaiseVersionError();
            }
        }
        // Файл версии отсутствует
        else
        {
            // Если файла версий нет, но другие файлы есть
            if (ES3.GetFiles().Length > 0 || ES3.GetDirectories().Length > 0)
            {
                RaiseVersionError();
            }

            ES3.Save<string>("ver", GAME_VERSION, "version.es3");
        }

        void RaiseVersionError()
        {
            WarningPanel.instance.CreateWarningMessage(WarningPanelMessages.WarningTemplate.VersionMissing, $"Актуальная версия {GAME_VERSION}");
        }
    }

    // Удаление данных
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

    // Восстановление, в будущем сделаю
    public static void FileRecovery(string fileName, string path)
    {
        // Добавить обработку ошибок файлов. Либо сделать индивидуальную обработку, либло сносить все сейвы при сбое
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
