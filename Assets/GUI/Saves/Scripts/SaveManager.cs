using System.Collections;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
public enum SavePageScroll
{
    Left,
    Right,
}

public struct SaveManagerData
{
    public SaveManagerData(int i)
    {
        SavesTaken = new Dictionary<int, string>();
    }

    public Dictionary<int, string> SavesTaken;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance = null;

    public float speed { get; set; } = 4f;

    private (int width, int height) CaptureRes = (1280, 720);

    [HideInInspector]
    public const int savesPerPage = 2;

    [HideInInspector]
    public const int maxPages = 12;

    private BottomPages bottomPages;

    private GameObject[] allFiles = new GameObject[savesPerPage];
    private GameObject[] allScreenshots = new GameObject[savesPerPage];

    private Camera ScreenshotCamera;

    private Texture2D[] currentTextures = new Texture2D[savesPerPage];

    [HideInInspector]
    public Dictionary<int, string> SavesTaken = new Dictionary<int, string>();

    public int currentPage = 0;

    private static string SaveSystemDataFile;
    private static string SaveFileName;

    private static string ScreenshotsFolder;
    private static string SaveFilesFolder;

    public float frameAplhaOff { get; private set; } = 1f;
    public float frameAplhaOn { get; private set; } = 0.4f;

    [SerializeField]
    private SaveBackButton backButton;

    private IEnumerator _update;

    private void Awake()
    {
        instance = this;

        SaveSystemDataFile = "SaveSystemData.es3";
        SaveFilesFolder = SaveSystemUtils.SaveFilesFolder;
        SaveFileName = SaveSystemUtils.SaveFileName;
        ScreenshotsFolder = SaveSystemUtils.ScreenshotsFolder;

        bottomPages = gameObject.transform.GetChild(2).GetComponent<BottomPages>();

        InitFiles();
    }

    private void InitFiles()
    {
        GameObject Files = transform.GetChild(1).gameObject;

        for (int i = 0; i < savesPerPage; i++)
        {
            GameObject file = Files.transform.GetChild(i).gameObject;
            allFiles[i] = file;
            allScreenshots[i] = file.GetComponent<SaveFileFields>().Screenshot;
        }

        if (ES3.FileExists(SaveSystemDataFile) && ES3.KeyExists("SavesTaken", SaveSystemDataFile))
        {
            SavesTaken = ES3.Load<SaveManagerData>("SavesTaken", SaveSystemDataFile).SavesTaken;
        }
    }

    private void Start()
    {
        ScreenshotCamera = PanelsConfig.CurrentManager.GetGameCamera();
    }

    private IEnumerator IUpdate()
    {
        while (true)
        {
            if (StaticVariables.IN_SAVE_MENU &&
                !StaticVariables.GAME_IS_LOADING &&
                !StaticVariables.OVERLAY_ACTIVE)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    ScrollPage(SavePageScroll.Right);
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    ScrollPage(SavePageScroll.Left);
                }
            }

            yield return null;
        }
    }

    public void ClearCurrenTextures()
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
        if (!SaveManagerStatic.UIsystemDown)
        {
            SaveManagerStatic.UIsystemDown = true;
            switch (side)
            {
                case SavePageScroll.Left:
                    if (currentPage > 0)
                    {
                        LoadPage(currentPage - 1);
                    }
                    break;
                case SavePageScroll.Right:
                    if (currentPage < (maxPages - 1))
                    {
                        LoadPage(currentPage + 1);
                    }
                    break;
            }
        }
    }

    public void LoadPage(int pageToLoad)
    {
        int oldPage = currentPage;
        currentPage = pageToLoad;

        bottomPages.DisappearNumbers(oldPage);
        bottomPages.HidePage(oldPage);

        StartCoroutine(LoadPictures(oldPage, pageToLoad));

        bottomPages.AppearNumbers(pageToLoad);
        bottomPages.ShowPage(pageToLoad);
    }

    public void LoadFirstPage()
    {
        for (int i = 0; i < savesPerPage; i++)
        {
            SaveFileFields saveFileFields = allFiles[i].GetComponent<SaveFileFields>();

            /*GameObject savedPanel = saveFileFields._SaveChoiseAnimator;
            GameObject unsavedPanel = saveFileFields._FirstSaveAnimator;
            GameObject MainMenuPanel = saveFileFields._MainMenuLoad;*/

            GameObject screenshot = allScreenshots[i];
            GameObject noImage = saveFileFields.NoImage;
            GameObject overPanel = saveFileFields.OverPanel;

            if (SavesTaken.ContainsKey(i))
            {
                if (SaveManagerStatic.ifInMainMenu)
                {
                    //FadeManager.FadeObject(MainMenuPanel, true);
                }
                else
                {
                    //FadeManager.FadeObject(savedPanel, true);
                }

                saveFileFields.Datetime.GetComponent<Text>().text = SavesTaken[i];
                saveFileFields.Datetime.GetComponent<CanvasGroup>().alpha = 1f;

                var texture = ES3.LoadImage($"{ScreenshotsFolder}/screenshot{i}.png");
                texture.name = "screenshot" + i;
                currentTextures[i] = texture;

                screenshot.GetComponent<RawImage>().texture = texture;

                screenshot.GetComponent<CanvasGroup>().alpha = 1f;

                FadeManager.FadeObject(overPanel, true);
                noImage.GetComponent<CanvasGroup>().alpha = frameAplhaOff;
                //FadeManager.FadeObject(unsavedPanel, false);
            }
            else
            {
                FadeManager.FadeObject(overPanel, false);
                FadeManager.FadeObject(noImage, true);
                screenshot.GetComponent<CanvasGroup>().alpha = 0f;
                screenshot.GetComponent<RawImage>().texture = null;
                currentTextures[i] = null;
                saveFileFields.Datetime.HideText();

                if (!SaveManagerStatic.ifInMainMenu)
                {
                    FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, false);
                    FadeManager.FadeObject(saveFileFields._MainMenuLoadObject, false);

                    FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, true);
                    saveFileFields._FirstSaveAnimator.ResetPanel();
                }
            }
        }

        bottomPages.StartingLoad();
    }

    private IEnumerator LoadPictures(int currentPage, int pageToLoad)
    {
        // Выцветание скринов
        List<IEnumerator> enumerators_prev = new List<IEnumerator>();
        List<IEnumerator> enumerators_next = new List<IEnumerator>();

        List<Action> action_prev = new List<Action>();
        List<Action> action_next = new List<Action>();

        for (int i = 0; i < savesPerPage; i++)
        {
            int a_prev = currentPage * savesPerPage + i;
            int a_next = pageToLoad * savesPerPage + i;

            SaveFileFields saveFileFields = allFiles[i].GetComponent<SaveFileFields>();

            /*GameObject savedPanel = saveFileFields._SaveChoiseAnimator;
            GameObject unsavedPanel = saveFileFields._FirstSaveAnimator;
            GameObject mainMenuPanel = saveFileFields._MainMenuLoad;*/

            GameObject noImage = saveFileFields.NoImage;
            GameObject overPanel = saveFileFields.OverPanel;

            // [До]
            if (SaveManagerStatic.ifInMainMenu)
            {
                allFiles[i].GetComponent<MainMenuLoad>().InstantHideCassette();
            }

            if (SavesTaken.ContainsKey(a_prev))
            {
                if (SaveManagerStatic.ifInMainMenu)
                {
                    //FadeManager.FadeObject(mainMenuPanel, false);
                    //enumerators_prev.Add(FadeManager.FadeObject(mainMenuPanel, false, optionsGradientSpeed));
                }
                else
                {
                    allFiles[i].GetComponent<SaveChoiseIconAnimator>().ResetPanel();
                    //FadeManager.FadeObject(savedPanel, false);
                    //enumerators_prev.Add(FadeManager.FadeObject(savedPanel, false, optionsGradientSpeed));
                }
            }

            if (!SavesTaken.ContainsKey(a_prev))
            {
                if (SaveManagerStatic.ifInMainMenu)
                {

                }
                else
                {
                    //FadeManager.FadeObject(unsavedPanel, false);
                }
            }

            // [После]
            if (SavesTaken.ContainsKey(a_next))
            {
                if (SaveManagerStatic.ifInMainMenu)
                {
                    //enumerators_next.Add(FadeManager.FadeObject(mainMenuPanel, true, speed));
                }
                else
                {
                    //enumerators_next.Add(FadeManager.FadeObject(savedPanel, true, speed));
                }
            }

            if (!SavesTaken.ContainsKey(a_next))
            {
                if (SaveManagerStatic.ifInMainMenu)
                {
                }
                else
                {
                    //FadeManager.FadeObject(unsavedPanel, false);
                    //enumerators_next.Add(FadeManager.FadeObject(unsavedPanel, true, speed));
                }
            }

            // Переход [Занятый] => [Занятый]
            if (SavesTaken.ContainsKey(a_prev) && SavesTaken.ContainsKey(a_next))
            {
                enumerators_prev.Add(FadeManager.FadeOnly(allScreenshots[i], false, speed));
                //enumerators_prev.Add(FadeManager.FadeOnly(saveFileFields.Datetime, false, speed));
                enumerators_prev.Add(FadeManager.FadeOnly(noImage, true, speed));
                action_prev.Add(delegate { StartCoroutine(saveFileFields.CloseOverPanel(1)); });

                enumerators_next.Add(SetDateTime(saveFileFields.Datetime.GetComponent<Text>(), SavesTaken[a_next]));
                //enumerators_next.Add(FadeManager.FadeOnly(saveFileFields.Datetime, true, speed));
                enumerators_next.Add(FadeManager.FadeOnly(allScreenshots[i], true, speed));
                enumerators_next.Add(FadeManager.FadeToTargetAlpha(noImage, frameAplhaOff, speed));
                action_next.Add(delegate { StartCoroutine(saveFileFields.OpenOverPanel(1)); });
            }

            // Переход [Занятый] => [Пустой]
            if (SavesTaken.ContainsKey(a_prev) && !SavesTaken.ContainsKey(a_next))
            {
                //enumerators_prev.Add(FadeManager.FadeOnly(saveFileFields.Datetime, false, speed));
                enumerators_prev.Add(FadeManager.FadeOnly(allScreenshots[i], false, speed));
                enumerators_prev.Add(FadeManager.FadeOnly(noImage, true, speed));
                action_prev.Add(delegate { StartCoroutine(saveFileFields.CloseOverPanel(1)); });
            }

            // Переход [Пустой] => [Занятый]
            if (!SavesTaken.ContainsKey(a_prev) && SavesTaken.ContainsKey(a_next))
            {
                enumerators_next.Add(SetDateTime(saveFileFields.Datetime.GetComponent<Text>(), SavesTaken[a_next]));
                //enumerators_next.Add(FadeManager.FadeOnly(saveFileFields.Datetime, true, speed));
                enumerators_next.Add(FadeManager.FadeToTargetAlpha(noImage, frameAplhaOff, speed));
                enumerators_next.Add(FadeManager.FadeOnly(allScreenshots[i], true, speed));

                action_next.Add(delegate { StartCoroutine(saveFileFields.OpenOverPanel(1)); });
            }

            // Переход [Пустой] => [Пустой]
            if (!SavesTaken.ContainsKey(a_prev) && !SavesTaken.ContainsKey(a_next))
            {
                // Ничего
            }
        }

        foreach (var action in action_prev)
        {
            action.Invoke();
        }

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(enumerators_prev));

        // Очистка текущих скринов
        ClearCurrenTextures();
        for (int i = 0; i < savesPerPage; i++)
        {
            SaveFileFields saveFileFields = allFiles[i].GetComponent<SaveFileFields>();
            saveFileFields.ResetAll();

            var fs = allFiles[i].GetComponent<FirstSaveAnimator>();
            fs.DisappearCassetteInstant();

            var sca = allFiles[i].GetComponent<SaveChoiseIconAnimator>();
            sca.ResetPanel();

            var sca2 = allFiles[i].GetComponent<SaveChoiseAnimator>();
            sca2.HideCross();
        }

        // Загрузка текстур новых скринов
        for (int i = 0; i < savesPerPage; i++)
        {
            int actualSaveNum = pageToLoad * savesPerPage + i;

            if (SavesTaken.ContainsKey(actualSaveNum))
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

        foreach (var action in action_next)
        {
            action.Invoke();
        }

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(enumerators_next));

        SaveManagerStatic.UIsystemDown = false;
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
        ScreenshotCamera.targetTexture = overRenderTexture;
        ScreenshotCamera.Render();

        var texture = new Texture2D(CaptureRes.width, CaptureRes.height, TextureFormat.RGB24, false);
        texture.name = "OverScreenshotTex" + saveNum;
        texture.ReadPixels(new Rect(0, 0, CaptureRes.width, CaptureRes.height), 0, 0);
        texture.Apply();

        overscreenshot.GetComponent<RawImage>().texture = texture;

        ScreenshotCamera.targetTexture = currentTexture;
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

        SavesTaken.Add(actualSaveNum, "");
        SaveCurrentData();

        screenshot.GetComponent<RawImage>().texture = texture;
        FadeManager.FadeObject(overscreenshot, false);
        overscreenshot.GetComponent<RawImage>().texture = null;


        SaveManagerStatic.UIsystemDown = false;
    }

    public IEnumerator SetScreenshot(int localSaveNum, GameObject screenshot)
    {
        int actualSaveNum = currentPage * savesPerPage + localSaveNum;
        yield return new WaitForEndOfFrame();

        var currentTexture = RenderTexture.active;

        var renderTexture = RenderTexture.GetTemporary(CaptureRes.width, CaptureRes.height, 24);
        renderTexture.name = "Temprender" + localSaveNum;

        RenderTexture.active = renderTexture;
        ScreenshotCamera.targetTexture = renderTexture;
        ScreenshotCamera.Render();

        Texture2D texture = new Texture2D(CaptureRes.width, CaptureRes.height, TextureFormat.RGB24, false);
        texture.name = "ScreenshotTex" + localSaveNum;
        texture.ReadPixels(new Rect(0, 0, CaptureRes.width, CaptureRes.height), 0, 0);
        texture.Apply();

        screenshot.GetComponent<RawImage>().texture = texture;

        ScreenshotCamera.targetTexture = currentTexture;
        RenderTexture.active = currentTexture;

        RenderTexture.ReleaseTemporary(renderTexture);



        // SetScreenshot вызывается только во время 1го сейва. Если до 1го сейва не было сейвов => триггер AppearAnimation
        /*if (!savesTaken.Contains(true))
        {
            StaticVariables.MainMenuContinueButtonAnimationTrigger = MMContinueButtonState.AppearAnimation;
            ES3.Save<MMContinueButtonState>("ContinueTrigger", MMContinueButtonState.AppearAnimation, SaveFilesData);
        }*/
        yield return new WaitForSeconds(0.5f);

        currentTextures[localSaveNum] = texture;
        string fileName = $"{ScreenshotsFolder}/screenshot{actualSaveNum}.png";
        ES3.SaveImage(texture, fileName);
    }

    public void DeleteSave(int saveNum)
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;

        string screenshotFileName = $"{ScreenshotsFolder}/screenshot{actualSaveNum}.png";
        string fileName = $"{SaveFilesFolder}/{SaveFileName}{actualSaveNum}.es3";

        if (ES3.FileExists(fileName) && ES3.FileExists(screenshotFileName))
        {
            ES3.DeleteFile(screenshotFileName);
            SavesTaken.Remove(actualSaveNum);
            SaveCurrentData();

            // Если после УДАЛЕНИЯ не осталось true сейвов, значит удалён последний => триггер 0
            /*if (!savesTaken.Contains(true))
            {
                ES3.Save<MMContinueButtonState>("ContinueTrigger", MMContinueButtonState.HideAnimation, SaveFilesData);
                StaticVariables.MainMenuContinueButtonAnimationTrigger = MMContinueButtonState.HideAnimation;
            }*/

            ES3.DeleteFile(fileName);
        }
    }

    public void AddSaveData(int actualSaveNum, string datetime)
    {
        if (!SavesTaken.ContainsKey(actualSaveNum))
        {
            SavesTaken.Add(actualSaveNum, datetime);
        }
    }

    public void RemoveDateTime(int saveNum)
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;
        SavesTaken[actualSaveNum] = string.Empty;
        SaveCurrentData();
    }

    public int GetActualSaveNum(int localSaveNum)
    {
        return currentPage * savesPerPage + localSaveNum;
    }

    public void SaveCurrentData()
    {
        ES3.Save<SaveManagerData>("SavesTaken", new SaveManagerData() { SavesTaken = SavesTaken }, SaveSystemDataFile);
    }

    public void CloseSave()
    {
        PanelsConfig.CurrentManager.CloseSaveMenu();
    }

    public void ResetManager()
    {
        currentPage = 0;

        backButton.ResetButtonState();
        bottomPages.InitialReset();
        LoadFirstPage();

        _update = IUpdate();
        StartCoroutine(_update);
    }


    public void OnSaveClose()
    {
        if (_update != null)
        {
            StopCoroutine(_update);
        }
    }


}
