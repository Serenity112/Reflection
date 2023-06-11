using System.Collections;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static StaticVariables;
using System.Collections.Generic;

public enum SavePageScroll
{
    Left,
    Right,
}
public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;

    [SerializeField] private float pagesScrollSpeed;

    public float optionsGradientSpeed;

    [SerializeField] private int captureWidth;

    [SerializeField] private int captureHeight;

    public const int savesPerPage = 6;

    public const int maxPages = 12;

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

        if (!StaticVariables.ifInMainMenu)
        {
            GameCamera = PanelsManager.instance.GameCamera;
        }

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

        FirstLoad();
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

    public void FirstLoad()
    {
        for (int saveNum = 0; saveNum < savesPerPage; saveNum++)
        {
            SaveFileFields saveFileFields = allFiles[saveNum].GetComponent<SaveFileFields>();

            GameObject savedPanel = saveFileFields.SavedPanel;
            GameObject unsavedPanel = saveFileFields.UnSavedPanel;
            GameObject MainMenuPanel = saveFileFields.MainMenuPanel;
            GameObject screenshot = allScreenshots[saveNum];

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

                saveFileFields.overPanel.GetComponent<CanvasGroup>().alpha = 1f; // Overpanel

                var texture = ES3.LoadImage($"{ScreenshotsFolder}/screenshot{saveNum}.png");
                texture.name = "screenshot" + saveNum;
                currentTextures[saveNum] = texture;

                screenshot.GetComponent<RawImage>().texture = texture;

                screenshot.GetComponent<CanvasGroup>().alpha = 1f;
                saveFileFields.AllowSaveLoad = true;
                saveFileFields.AllowOverPanel = true;
            }
            else
            {
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
    public IEnumerator LoadPictures(int prevPage, int pageToLoad)
    {
        // Выцветание скринов
        int firstActiveSave = -1;

        for (int i = 0; i < savesPerPage; i++)
        {
            if (savesTaken[prevPage * savesPerPage + i])
            {
                firstActiveSave = i;
                break;
            }
        }

        for (int i = 0; i < savesPerPage; i++)
        {
            SaveFileFields saveFileFields = allFiles[i].GetComponent<SaveFileFields>();
            GameObject savedPanel = saveFileFields.SavedPanel;
            GameObject unsavedPanel = saveFileFields.UnSavedPanel;
            GameObject overPanel = saveFileFields.overPanel;

            StartCoroutine(FadeManager.FadeOnly(saveFileFields.datetime, false, pagesScrollSpeed));
            StartCoroutine(FadeManager.FadeObject(overPanel, false, pagesScrollSpeed)); // Overpanel
            StartCoroutine(FadeManager.FadeObject(savedPanel, false, pagesScrollSpeed));
            FadeManager.FadeObject(unsavedPanel, false);

            if (i != firstActiveSave)
                StartCoroutine(FadeManager.FadeObject(allScreenshots[i], false, pagesScrollSpeed));
        }

        if (firstActiveSave != -1)
        {
            yield return StartCoroutine(FadeManager.FadeObject(allScreenshots[firstActiveSave], false, pagesScrollSpeed));
        }


        // Очистка текущих скринов
        ClearCurrent();

        // Загрузка новых скринов
        for (int i = 0; i < savesPerPage; i++)
        {
            int actualSaveNum = pageToLoad * savesPerPage + i;
            SaveFileFields saveFileFields = allFiles[i].GetComponent<SaveFileFields>();

            if (savesTaken[actualSaveNum])
            {
                saveFileFields.datetime.GetComponent<Text>().text = saveDataTimes[actualSaveNum];
                StartCoroutine(FadeManager.FadeOnly(saveFileFields.datetime, true, pagesScrollSpeed));

                var texture = ES3.LoadImage($"{ScreenshotsFolder}/screenshot{actualSaveNum}.png");
                texture.name = "screenshot" + actualSaveNum;
                currentTextures[i] = texture;
                allScreenshots[i].GetComponent<RawImage>().texture = texture;
            }
            else
            {
                StartCoroutine(FadeManager.FadeOnly(saveFileFields.datetime, false, pagesScrollSpeed));

                allScreenshots[i].GetComponent<RawImage>().texture = null;
                currentTextures[i] = null;
            }

            yield return null;
        }

        List<IEnumerator> enumerators = new List<IEnumerator>();

        // Вцветание новых скринов
        for (int i = 0; i < savesPerPage; i++)
        {
            int saveNum = pageToLoad * savesPerPage + i;

            SaveFileFields saveFileFields = allFiles[i].GetComponent<SaveFileFields>();

            GameObject savedPanel = saveFileFields.SavedPanel;
            GameObject unsavedPanel = saveFileFields.UnSavedPanel;
            GameObject MainMenuPanel = saveFileFields.MainMenuPanel;

            saveFileFields.exitLeft = true;
            saveFileFields.exitRight = true;

            if (savesTaken[saveNum])
            {
                enumerators.Add(FadeManager.FadeOnly(allScreenshots[i], true, pagesScrollSpeed));

                if (StaticVariables.ifInMainMenu)
                {
                    StartCoroutine(FadeManager.FadeObject(MainMenuPanel, true, pagesScrollSpeed));
                }
                else
                {
                    StartCoroutine(FadeManager.FadeObject(savedPanel, true, pagesScrollSpeed));
                }

                FadeManager.FadeObject(unsavedPanel, false);

                StartCoroutine(FadeManager.FadeObject(saveFileFields.overPanel, true, pagesScrollSpeed)); // Overpanel
                saveFileFields.AllowSaveLoad = true;
                saveFileFields.AllowOverPanel = true;
            }
            else
            {
                allScreenshots[i].GetComponent<CanvasGroup>().alpha = 0f;
                if (StaticVariables.ifInMainMenu)
                {
                    FadeManager.FadeObject(MainMenuPanel, false);
                }
                else
                {
                    FadeManager.FadeObject(savedPanel, false);
                    FadeManager.FadeObject(unsavedPanel, true);
                }

                saveFileFields.overPanel.GetComponent<CanvasGroup>().alpha = 0f; // Overpanel
                saveFileFields.AllowSaveLoad = false;
                saveFileFields.AllowOverPanel = false;
            }
        }

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(enumerators));
        //yield return new WaitForSeconds(0.5f);

        StaticVariables.UIsystemDown = false;
    }

    public IEnumerator OverrideScreenshot(int saveNum, GameObject screenshot, GameObject overscreenshot, float speed)
    {
        int actualSaveNum = currentPage * savesPerPage + saveNum;
        yield return new WaitForEndOfFrame();

        var bufferTexture = currentTextures[saveNum];
        var currentTexture = RenderTexture.active;

        var overRenderTexture = RenderTexture.GetTemporary(captureWidth, captureHeight, 24);
        overRenderTexture.name = "OverTemprender" + saveNum;

        RenderTexture.active = overRenderTexture;
        GameCamera.targetTexture = overRenderTexture;
        GameCamera.Render();

        var texture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        texture.name = "OverScreenshotTex" + saveNum;
        texture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
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

        savesTaken[currentPage * savesPerPage + saveNum] = true;
        ES3.Save<bool[]>("saveTaken", savesTaken, SaveFilesData);


        screenshot.GetComponent<RawImage>().texture = texture;
        FadeManager.FadeObject(overscreenshot, false);
        overscreenshot.GetComponent<RawImage>().texture = null;


        StaticVariables.UIsystemDown = false;
    }

    public IEnumerator SetScreenshot(int saveNum, GameObject screenshot)
    {
        int actualSaveNum = currentPage * savesPerPage + saveNum;
        yield return new WaitForEndOfFrame();

        var currentTexture = RenderTexture.active;

        var renderTexture = RenderTexture.GetTemporary(captureWidth, captureHeight, 24);
        renderTexture.name = "Temprender" + actualSaveNum;

        RenderTexture.active = renderTexture;
        GameCamera.targetTexture = renderTexture;
        GameCamera.Render();

        var texture = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        texture.name = "ScreenshotTex" + actualSaveNum;
        texture.ReadPixels(new Rect(0, 0, captureWidth, captureHeight), 0, 0);
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

        savesTaken[currentPage * savesPerPage + saveNum] = true;
        ES3.Save<bool[]>("saveTaken", savesTaken, SaveFilesData);

        StaticVariables.UIsystemDown = false;
    }

    public void DeleteSave(int pageNum)
    {
        int saveNum = currentPage * savesPerPage + pageNum;

        string screenshotFileName = $"{ScreenshotsFolder}/screenshot{saveNum}.png";
        string fileName = $"{SaveFilesFolder}/{SaveFileName}{saveNum}.es3";

        if (ES3.FileExists(fileName) && ES3.FileExists(screenshotFileName))
        {
            ES3.DeleteFile(screenshotFileName);
            savesTaken[saveNum] = false;
            ES3.Save<bool[]>("saveTaken", savesTaken, SaveFilesData);

            // Если после УДАЛЕНИЯ не осталось true сейвов, значит удалён последний => триггер 0
            if (!savesTaken.Contains(true))
            {
                ES3.Save<MMContinueButtonState>("ContinueTrigger", MMContinueButtonState.HideAnimation, SaveFilesData);
                StaticVariables.MainMenuContinueButtonAnimationTrigger = MMContinueButtonState.HideAnimation;
            }

            ES3.DeleteKey("SaveFile" + saveNum, fileName);

            // Добавить удаление спешл ивентов!
        }
    }

    public void SaveDateTime(int saveNum, string datetime)
    {
        saveDataTimes[currentPage * savesPerPage + saveNum] = datetime;
        ES3.Save<string[]>("saveDataTimes", saveDataTimes, SaveFilesData);
    }

    public void RemoveDateTime(int saveNum)
    {
        saveDataTimes[currentPage * savesPerPage + saveNum] = string.Empty;
        ES3.Save<string[]>("saveDataTimes", saveDataTimes, SaveFilesData);
    }

    public void CloseSave()
    {
        PanelsConfig.CurrentManager.CloseSaveMenu();
    }
}
