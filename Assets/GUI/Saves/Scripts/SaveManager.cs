using System.Collections;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static StaticVariables;
using System.Collections.Generic;
using System.Threading;

public enum SavePageScroll
{
    Left,
    Right,
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;

    [HideInInspector] public float pagesScrollSpeed = 4f;

    [HideInInspector] public float optionsGradientSpeed = 4f;

    private (int width, int height) CaptureRes = (1280, 720);

    [HideInInspector] public const int savesPerPage = 6;

    [HideInInspector] public const int maxPages = 12;

    [SerializeField] private GameObject bottomPagesObj;
    private BottomPages bottomPages;

    private GameObject[] allFiles = new GameObject[savesPerPage];
    private GameObject[] allScreenshots = new GameObject[savesPerPage];

    private Camera GameCamera;

    private Texture2D[] currentTextures = new Texture2D[savesPerPage];

    [HideInInspector] public bool[] savesTaken = new bool[maxPages * savesPerPage];

    [HideInInspector] public string[] saveDataTimes = new string[maxPages * savesPerPage];

    public int currentPage = 0;

    private static string SaveFilesData;
    private static string SaveFileName;

    private static string ScreenshotsFolder;
    private static string SaveFilesFolder;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        SaveFilesData = SaveSystemUtils.SaveFilesData;
        SaveFilesFolder = SaveSystemUtils.SaveFilesFolder;
        SaveFileName = SaveSystemUtils.SaveFileName;
        ScreenshotsFolder = SaveSystemUtils.ScreenshotsFolder;

        GameObject Files = transform.GetChild(0).gameObject;

        for (int i = 0; i < savesPerPage; i++)
        {
            GameObject file = Files.transform.GetChild(i).gameObject;
            allFiles[i] = file;
            allScreenshots[i] = file.GetComponent<SaveFileFields>().screenshot;
        }

        GameCamera = PanelsConfig.CurrentManager.GetGameCamera();

        if (ES3.FileExists(SaveFilesData) && ES3.KeyExists("saveTaken", SaveFilesData))
        {
            savesTaken = ES3.Load<bool[]>("saveTaken", SaveFilesData);
        }

        if (ES3.FileExists(SaveFilesData) && ES3.KeyExists("saveDataTimes", SaveFilesData))
        {
            saveDataTimes = ES3.Load<string[]>("saveDataTimes", SaveFilesData);
        }
        else // Стартовая инициализация
        {
            for (int i = 0; i < saveDataTimes.Length; i++)
            {
                saveDataTimes[i] = string.Empty;
            }
            ES3.Save<string[]>("saveDataTimes", saveDataTimes, SaveFilesData);
        }

        bottomPages = bottomPagesObj.GetComponent<BottomPages>();

        LoadFirstPage();
    }

    public void ClearCurrent()
    {
        for (int i = 0; i < savesPerPage; i++)
        {
            if (currentTextures[i] != null)
            {
                Destroy(currentTextures[i]);
                currentTextures[i] = null;
                allScreenshots[i].GetComponent<CanvasGroup>().alpha = 0f;
            }
        }
    }

    // Листание главных страниц по левой/правой кнопке по бокам
    public void ScrollPage(SavePageScroll side)
    {
        if (!StaticVariables.UIsystemDown)
        {
            switch (side)
            {
                case SavePageScroll.Left:
                    if (currentPage > 0)
                    {
                        StaticVariables.UIsystemDown = true;
                        LoadPage(currentPage - 1);
                    }
                    break;
                case SavePageScroll.Right:
                    if (currentPage < (maxPages - 1))
                    {
                        StaticVariables.UIsystemDown = true;
                        LoadPage(currentPage + 1);
                    }
                    break;
            }
        }
    }

    public void LoadPage(int pageToLoad)
    {
        bottomPages.DisappearNumbers(currentPage);
        bottomPages.Disactivate(currentPage);

        StartCoroutine(LoadPictures(currentPage, pageToLoad));
        currentPage = pageToLoad;

        bottomPages.AppearNumbers(pageToLoad);
        bottomPages.Activate(pageToLoad);
    }

    public void LoadFirstPage()
    {
        for (int saveNum = 0; saveNum < savesPerPage; saveNum++)
        {
            SaveFileFields saveFileFields = allFiles[saveNum].GetComponent<SaveFileFields>();

            GameObject savedPanel = saveFileFields.SavedPanel;
            GameObject unsavedPanel = saveFileFields.UnSavedPanel;
            GameObject MainMenuPanel = saveFileFields.MainMenuPanel;

            GameObject screenshot = allScreenshots[saveNum];

            GameObject noImage = saveFileFields.NoImage;
            GameObject frame = saveFileFields.Frame;
            GameObject overPanel = saveFileFields.overPanel;

            if (savesTaken[saveNum])
            {
                if (StaticVariables.ifInMainMenu)
                {
                    FadeManager.FadeObject(MainMenuPanel, true);
                }
                else
                {
                    FadeManager.FadeObject(savedPanel, true);
                }

                saveFileFields.datetime.GetComponent<Text>().text = saveDataTimes[saveNum];
                saveFileFields.datetime.GetComponent<CanvasGroup>().alpha = 1f;

                var texture = ES3.LoadImage($"{ScreenshotsFolder}/screenshot{saveNum}.png");
                texture.name = "screenshot" + saveNum;
                currentTextures[saveNum] = texture;

                screenshot.GetComponent<RawImage>().texture = texture;
                screenshot.GetComponent<CanvasGroup>().alpha = 1f;

                FadeManager.FadeObject(overPanel, true);
                FadeManager.FadeObject(frame, true);
                FadeManager.FadeObject(noImage, false);
                FadeManager.FadeObject(unsavedPanel, false);

                saveFileFields.AllowSaveLoad = true;
                saveFileFields.AllowOverPanel = true;
            }
            else
            {
                FadeManager.FadeObject(overPanel, false);
                FadeManager.FadeObject(frame, false);
                FadeManager.FadeObject(noImage, true);

                FadeManager.FadeObject(MainMenuPanel, false);
                FadeManager.FadeObject(savedPanel, false);

                saveFileFields.datetime.GetComponent<CanvasGroup>().alpha = 0f;

                screenshot.GetComponent<RawImage>().texture = null;
                currentTextures[saveNum] = null;

                screenshot.GetComponent<CanvasGroup>().alpha = 0f;
                if (!StaticVariables.ifInMainMenu)
                {
                    FadeManager.FadeObject(unsavedPanel, true);
                }

                saveFileFields.AllowSaveLoad = false;
                saveFileFields.AllowOverPanel = false;
            }
        }

        bottomPagesObj.GetComponent<BottomPages>().StartingLoad();
    }

    public IEnumerator LoadPictures(int currentPage, int pageToLoad)
    {
        // Выцветание скринов
        List<IEnumerator> enumerators_prev = new List<IEnumerator>();
        List<IEnumerator> enumerators_next = new List<IEnumerator>();

        for (int i = 0; i < savesPerPage; i++)
        {
            int a_prev = currentPage * savesPerPage + i;
            int a_next = pageToLoad * savesPerPage + i;

            SaveFileFields saveFileFields = allFiles[i].GetComponent<SaveFileFields>();
            GameObject savedPanel = saveFileFields.SavedPanel;
            GameObject unsavedPanel = saveFileFields.UnSavedPanel;
            GameObject MainMenuPanel = saveFileFields.MainMenuPanel;

            GameObject noImage = saveFileFields.NoImage;
            GameObject frame = saveFileFields.Frame;
            GameObject overPanel = saveFileFields.overPanel;

            //
            if (savesTaken[a_next])
            {
                if (StaticVariables.ifInMainMenu)
                {
                    FadeManager.FadeObject(MainMenuPanel, true);
                }
                else
                {
                    FadeManager.FadeObject(savedPanel, true);
                }

                FadeManager.FadeObject(unsavedPanel, false);
            }

            //
            if (!savesTaken[a_next])
            {
                if (!StaticVariables.ifInMainMenu)
                {
                    FadeManager.FadeObject(unsavedPanel, true);
                }

                FadeManager.FadeObject(MainMenuPanel, false);
                FadeManager.FadeObject(savedPanel, false);
            }


            // Переход [Занятый] => [Занятый]
            if (savesTaken[a_prev] && savesTaken[a_next])
            {
                enumerators_prev.Add(FadeManager.FadeOnly(allScreenshots[i], false, pagesScrollSpeed));
                enumerators_prev.Add(FadeManager.FadeOnly(saveFileFields.datetime, false, pagesScrollSpeed));

                enumerators_next.Add(SetDateTime(saveFileFields.datetime.GetComponent<Text>(), saveDataTimes[a_next]));
                enumerators_next.Add(FadeManager.FadeOnly(saveFileFields.datetime, true, pagesScrollSpeed));
                enumerators_next.Add(FadeManager.FadeOnly(allScreenshots[i], true, pagesScrollSpeed));
            }

            // Переход [Занятый] => [Пустой]
            if (savesTaken[a_prev] && !savesTaken[a_next])
            {
                enumerators_prev.Add(FadeManager.FadeOnly(saveFileFields.datetime, false, pagesScrollSpeed));
                enumerators_prev.Add(FadeManager.FadeOnly(allScreenshots[i], false, pagesScrollSpeed));
                enumerators_prev.Add(FadeManager.FadeOnly(overPanel, false, pagesScrollSpeed));
                enumerators_prev.Add(FadeManager.FadeOnly(noImage, true, pagesScrollSpeed));

                enumerators_next.Add(FadeManager.FadeOnly(frame, false, pagesScrollSpeed));
            }

            // Переход [Пустой] => [Занятый]
            if (!savesTaken[a_prev] && savesTaken[a_next])
            {
                enumerators_prev.Add(FadeManager.FadeOnly(allScreenshots[i], false, pagesScrollSpeed));

                enumerators_next.Add(SetDateTime(saveFileFields.datetime.GetComponent<Text>(), saveDataTimes[a_next]));
                enumerators_next.Add(FadeManager.FadeOnly(saveFileFields.datetime, true, pagesScrollSpeed));
                enumerators_next.Add(FadeManager.FadeOnly(overPanel, true, pagesScrollSpeed));
                enumerators_next.Add(FadeManager.FadeOnly(noImage, false, pagesScrollSpeed));
                enumerators_next.Add(FadeManager.FadeOnly(frame, true, pagesScrollSpeed));
                enumerators_next.Add(FadeManager.FadeOnly(allScreenshots[i], true, pagesScrollSpeed));
            }

            // Переход [Пустой] => [Пустой]
            if (!savesTaken[a_prev] && !savesTaken[a_next])
            {
                // Ничего
            }
        }

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(enumerators_prev));

        // Очистка текущих скринов
        ClearCurrent();

        // Загрузка текстур новых скринов
        for (int i = 0; i < savesPerPage; i++)
        {
            int actualSaveNum = pageToLoad * savesPerPage + i;

            if (savesTaken[actualSaveNum])
            {
                var texture = ES3.LoadImage($"{ScreenshotsFolder}/screenshot{actualSaveNum}.png");
                texture.name = "screenshot" + actualSaveNum;
                currentTextures[i] = texture;
                allScreenshots[i].GetComponent<RawImage>().texture = texture;
            }
            else
            {
                allScreenshots[i].GetComponent<RawImage>().texture = null;
                currentTextures[i] = null;
            }

            yield return null;
        }

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(enumerators_next));

        StaticVariables.UIsystemDown = false;
    }

    private IEnumerator SetDateTime(Text text, string datetime)
    {
        text.text = datetime;
        yield return null;
    }

    public IEnumerator OverrideScreenshot(int saveNum, GameObject screenshot, GameObject overscreenshot, float speed)
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;
        yield return new WaitForEndOfFrame();

        var bufferTexture = currentTextures[saveNum];
        var currentTexture = RenderTexture.active;

        var overRenderTexture = RenderTexture.GetTemporary(CaptureRes.width, CaptureRes.height, 24);
        overRenderTexture.name = "OverTemprender" + saveNum;

        RenderTexture.active = overRenderTexture;
        GameCamera.targetTexture = overRenderTexture;
        GameCamera.Render();

        var texture = new Texture2D(CaptureRes.width, CaptureRes.height, TextureFormat.RGB24, false);
        texture.name = "OverScreenshotTex" + saveNum;
        texture.ReadPixels(new Rect(0, 0, CaptureRes.width, CaptureRes.height), 0, 0);
        texture.Apply();

        overscreenshot.GetComponent<RawImage>().texture = texture;

        GameCamera.targetTexture = currentTexture;
        RenderTexture.active = currentTexture;

        RenderTexture.ReleaseTemporary(overRenderTexture);

        yield return StartCoroutine(FadeManager.FadeObject(overscreenshot, true, speed));

        if (bufferTexture != null)
            Destroy(bufferTexture);

        string fileName = $"{ScreenshotsFolder}/screenshot{actualSaveNum}.png";
        if (ES3.FileExists(fileName))
        {
            ES3.DeleteFile(fileName);
        }

        currentTextures[saveNum] = texture;

        ES3.SaveImage(texture, fileName);

        savesTaken[actualSaveNum] = true;
        ES3.Save<bool[]>("saveTaken", savesTaken, SaveFilesData);


        screenshot.GetComponent<RawImage>().texture = texture;
        FadeManager.FadeObject(overscreenshot, false);
        overscreenshot.GetComponent<RawImage>().texture = null;


        StaticVariables.UIsystemDown = false;
    }

    public IEnumerator SetScreenshot(int saveNum, GameObject screenshot)
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;
        yield return new WaitForEndOfFrame();

        var currentTexture = RenderTexture.active;

        var renderTexture = RenderTexture.GetTemporary(CaptureRes.width, CaptureRes.height, 24);
        renderTexture.name = "Temprender" + saveNum;

        RenderTexture.active = renderTexture;
        GameCamera.targetTexture = renderTexture;
        GameCamera.Render();

        var texture = new Texture2D(CaptureRes.width, CaptureRes.height, TextureFormat.RGB24, false);
        texture.name = "ScreenshotTex" + saveNum;
        texture.ReadPixels(new Rect(0, 0, CaptureRes.width, CaptureRes.height), 0, 0);
        texture.Apply();

        screenshot.GetComponent<RawImage>().texture = texture;

        GameCamera.targetTexture = currentTexture;
        RenderTexture.active = currentTexture;

        RenderTexture.ReleaseTemporary(renderTexture);

        yield return new WaitForSeconds(0.5f);

        currentTextures[saveNum] = texture;

        string fileName = $"{ScreenshotsFolder}/screenshot{actualSaveNum}.png";

        ES3.SaveImage(texture, fileName);

        // SetScreenshot вызывается только во время 1го сейва. Если до 1го сейва не было сейвов => триггер AppearAnimation
        if (!savesTaken.Contains(true))
        {
            StaticVariables.MainMenuContinueButtonAnimationTrigger = MMContinueButtonState.AppearAnimation;
            ES3.Save<MMContinueButtonState>("ContinueTrigger", MMContinueButtonState.AppearAnimation, SaveFilesData);
        }

        savesTaken[actualSaveNum] = true;
        new Thread(() =>
        {
            ES3.Save<bool[]>("saveTaken", savesTaken, SaveFilesData);
        }).Start();

        StaticVariables.UIsystemDown = false;
    }

    public void DeleteSave(int saveNum)
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;

        string screenshotFileName = $"{ScreenshotsFolder}/screenshot{actualSaveNum}.png";
        string fileName = $"{SaveFilesFolder}/{SaveFileName}{actualSaveNum}.es3";

        if (ES3.FileExists(fileName) && ES3.FileExists(screenshotFileName))
        {
            ES3.DeleteFile(screenshotFileName);
            savesTaken[actualSaveNum] = false;
            ES3.Save<bool[]>("saveTaken", savesTaken, SaveFilesData);

            // Если после УДАЛЕНИЯ не осталось true сейвов, значит удалён последний => триггер 0
            if (!savesTaken.Contains(true))
            {
                ES3.Save<MMContinueButtonState>("ContinueTrigger", MMContinueButtonState.HideAnimation, SaveFilesData);
                StaticVariables.MainMenuContinueButtonAnimationTrigger = MMContinueButtonState.HideAnimation;
            }

            ES3.DeleteKey("SaveFile" + actualSaveNum, fileName);

            // Добавить удаление спешл ивентов!
        }
    }

    public void SaveDateTime(int saveNum, string datetime)
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;
        saveDataTimes[actualSaveNum] = datetime;
        ES3.Save<string[]>("saveDataTimes", saveDataTimes, SaveFilesData);
    }

    public void RemoveDateTime(int saveNum)
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;
        saveDataTimes[actualSaveNum] = string.Empty;
        ES3.Save<string[]>("saveDataTimes", saveDataTimes, SaveFilesData);
    }

    public void CloseSave()
    {
        PanelsConfig.CurrentManager.CloseSaveMenu();
    }
}
