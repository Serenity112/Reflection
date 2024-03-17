using System.Collections;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    public const int savesPerPage = 3;

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

    public static string DateTimeFormat = "HH:mm:ss dd/MM/yy";

    [SerializeField]
    private SaveBackButton backButton;

    private IEnumerator _update;

    private void Awake()
    {
        instance = this;

        InitFiles();

        bottomPages = gameObject.transform.GetChild(2).GetComponent<BottomPages>();
    }

    private void Start()
    {
        ScreenshotCamera = PanelsConfig.CurrentManager.GetGameCamera();
    }

    private void OnEnable()
    {
        _update = IUpdate();
        StartCoroutine(_update);
    }

    private void OnDisable()
    {
        if (_update != null)
        {
            StopCoroutine(_update);
        }
    }

    private void InitFiles()
    {
        GameObject Files = transform.GetChild(1).gameObject;

        for (int i = 0; i < savesPerPage; i++)
        {
            GameObject file = Files.transform.GetChild(i).gameObject;
            allFiles[i] = file;
            allScreenshots[i] = GetSaveFileFields(file).Screenshot;
        }

        try
        {
            if (ES3.FileExists(SaveSystemUtils.GameData) && ES3.KeyExists("SavesTaken", SaveSystemUtils.GameData))
            {
                SavesTaken = ES3.Load<SaveManagerData>("SavesTaken", SaveSystemUtils.GameData).SavesTaken;
            }
        }
        catch
        {
            //WarningPanel.instance.CreateWarningMessage(WarningPanelMessages.WarningTemplate.SaveSystemCorrupt, $"key: SavesTaken folder: {SaveSystemUtils.GameData}");
            //SaveSystemUtils.DumpFile(SaveSystemUtils.GameData);
        }
    }

    private IEnumerator IUpdate()
    {
        while (true)
        {
            if (StaticVariables.IN_SAVE_MENU &&
                !StaticVariables.GAME_IS_LOADING &&
                !StaticVariables.OVERLAY_ACTIVE &&
                !SaveManagerStatic.ClickBlocker &&
                !SaveManagerStatic.UiBloker)
            {

                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                {
                    ScrollPage(SavePageScroll.Right);
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
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
        if (!SaveManagerStatic.ClickBlocker)
        {
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
        if (!SaveManagerStatic.ClickBlocker)
        {
            SaveManagerStatic.ClickBlocker = true;

            int oldPage = currentPage;
            currentPage = pageToLoad;

            bottomPages.DisappearNumbers(oldPage);
            bottomPages.HidePage(oldPage);

            bottomPages.AppearNumbers(pageToLoad);
            bottomPages.ShowPage(pageToLoad);

            StartCoroutine(LoadPictures(oldPage, pageToLoad));
        }
    }

    private SaveFileFields GetSaveFileFields(GameObject file)
    {
        return file.transform.GetChild(0).GetComponent<SaveFileFields>();
    }

    public void LoadFirstPage()
    {
        for (int i = 0; i < savesPerPage; i++)
        {
            SaveFileFields saveFileFields = GetSaveFileFields(allFiles[i]);

            GameObject screenshot = allScreenshots[i];
            GameObject noImage = saveFileFields.NoImage;
            GameObject overPanel = saveFileFields.OverPanel;

            if (SavesTaken.ContainsKey(i))
            {
                saveFileFields.Datetime.ShowText();
                saveFileFields.Datetime.SetText(SavesTaken[i]);

                var texture = ES3.LoadImage($"{SaveSystemUtils.ScreenshotsFolder}/screenshot{i}.png");
                texture.name = "screenshot" + i;
                currentTextures[i] = texture;

                screenshot.GetComponent<RawImage>().texture = texture;
                screenshot.GetComponent<CanvasGroup>().alpha = 1f;
                FadeManager.FadeObject(overPanel, true);

                FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, false);

                if (SaveManagerStatic.ifInMainMenu)
                {
                    FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, false);
                    FadeManager.FadeObject(saveFileFields._MainMenuLoadObject, true);
                    saveFileFields._MainMenuLoad.ResetPanelSync();
                }
                else
                {
                    FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, true);
                    FadeManager.FadeObject(saveFileFields._MainMenuLoadObject, false);
                    saveFileFields._SaveChoiseAnimator.ResetPanelSync();
                }
            }
            else
            {
                FadeManager.FadeObject(overPanel, false);
                screenshot.GetComponent<CanvasGroup>().alpha = 0f;
                screenshot.GetComponent<RawImage>().texture = null;
                currentTextures[i] = null;
                saveFileFields.Datetime.HideText();

                FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, false);
                FadeManager.FadeObject(saveFileFields._MainMenuLoadObject, false);

                if (SaveManagerStatic.ifInMainMenu)
                {
                    FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, false);
                }
                else
                {
                    FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, true);
                    saveFileFields._FirstSaveAnimator.ResetPanelSync();
                }
            }
        }
    }

    private IEnumerator LoadPictures(int currentPage, int pageToLoad)
    {
        // Выцветание скринов
        List<IEnumerator> enumerators_prev = new List<IEnumerator>();
        List<IEnumerator> enumerators_next = new List<IEnumerator>();

        List<Action> middle_actions = new List<Action>();

        for (int i = 0; i < savesPerPage; i++)
        {
            int a_prev = currentPage * savesPerPage + i;
            int a_next = pageToLoad * savesPerPage + i;

            SaveFileFields saveFileFields = GetSaveFileFields(allFiles[i]);

            // Переход [Занятый] => [Занятый]
            if (SavesTaken.ContainsKey(a_prev) && SavesTaken.ContainsKey(a_next))
            {
                enumerators_prev.Add(FadeManager.FadeOnly(allScreenshots[i], false, speed));
                enumerators_prev.Add(saveFileFields.VF_Hide());
                enumerators_prev.Add(saveFileFields.Datetime.IHideText());

                enumerators_next.Add(FadeManager.FadeOnly(allScreenshots[i], true, speed));
                enumerators_next.Add(saveFileFields.VF_Show());
                enumerators_next.Add(saveFileFields.Datetime.IShowText(SavesTaken[a_next]));

                if (SaveManagerStatic.ifInMainMenu)
                {
                    enumerators_prev.Add(FadeManager.FadeObject(saveFileFields._MainMenuLoadObject, false, speed));
                    middle_actions.Add(delegate
                    {
                        StartCoroutine(saveFileFields.OpenOverPanel());
                        saveFileFields._MainMenuLoad.ResetPanelSync();
                    });
                    enumerators_next.Add(FadeManager.FadeObject(saveFileFields._MainMenuLoadObject, true, speed));
                }
                else
                {
                    enumerators_prev.Add(FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, false, speed));
                    middle_actions.Add(delegate
                    {
                        StartCoroutine(saveFileFields.OpenOverPanel());
                        saveFileFields._SaveChoiseAnimator.ResetPanelSync();
                    });
                    enumerators_next.Add(FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, true, speed));
                }
            }

            // Переход [Занятый] => [Пустой]
            if (SavesTaken.ContainsKey(a_prev) && !SavesTaken.ContainsKey(a_next))
            {
                enumerators_prev.Add(saveFileFields.Datetime.IHideText());
                enumerators_prev.Add(FadeManager.FadeOnly(allScreenshots[i], false, speed));
                enumerators_prev.Add(saveFileFields.VF_Hide());

                enumerators_next.Add(saveFileFields.VF_Show());
                enumerators_next.Add(saveFileFields.CloseOverPanel());

                if (SaveManagerStatic.ifInMainMenu)
                {
                    enumerators_prev.Add(FadeManager.FadeObject(saveFileFields._MainMenuLoadObject, false, speed));

                    middle_actions.Add(delegate
                    {
                        saveFileFields.CloseOverPanelInstant();
                    });
                }
                else
                {
                    enumerators_prev.Add(FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, false, speed));

                    middle_actions.Add(delegate
                    {
                        saveFileFields.CloseOverPanelInstant();
                       // FadeManager.FadeInLite(saveFileFields._FirstSaveAnimatorObject);
                        saveFileFields._FirstSaveAnimator.ResetPanelSync();
                    });

                    enumerators_next.Add(FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, true, speed));
                }
            }

            // Переход [Пустой] => [Занятый]
            if (!SavesTaken.ContainsKey(a_prev) && SavesTaken.ContainsKey(a_next))
            {
                enumerators_prev.Add(saveFileFields.VF_Hide());

                enumerators_next.Add(saveFileFields.Datetime.IShowText(SavesTaken[a_next]));
                enumerators_next.Add(FadeManager.FadeOnly(allScreenshots[i], true, speed));
                enumerators_next.Add(saveFileFields.VF_Show());

                if (SaveManagerStatic.ifInMainMenu)
                {
                    middle_actions.Add(delegate
                    {
                        FadeManager.FadeObject(saveFileFields._MainMenuLoadObject, true);
                        saveFileFields._MainMenuLoad.ResetPanelSync();
                        StartCoroutine(saveFileFields.OpenOverPanel());
                    });

                    enumerators_next.Add(FadeManager.FadeObject(saveFileFields._MainMenuLoadObject, true, speed));
                }
                else
                {
                    enumerators_prev.Add(FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, false, speed));

                    middle_actions.Add(delegate
                    {
                        FadeManager.FadeInLite(saveFileFields._SaveChoiseAnimatorObject);
                        saveFileFields._SaveChoiseAnimator.ResetPanelSync();
                        StartCoroutine(saveFileFields.OpenOverPanel());
                    });

                    enumerators_next.Add(FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, true, speed));
                }
            }

            // Переход [Пустой] => [Пустой]
            if (!SavesTaken.ContainsKey(a_prev) && !SavesTaken.ContainsKey(a_next))
            {
                if (SaveManagerStatic.ifInMainMenu)
                {

                }
                else
                {
                    enumerators_prev.Add(FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, false, speed * 3));

                    middle_actions.Add(delegate
                    {
                        //FadeManager.FadeInLite(saveFileFields._FirstSaveAnimatorObject);
                        saveFileFields._FirstSaveAnimator.ResetPanelSync();
                    });

                    enumerators_next.Add(FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, true, speed * 3));
                }
            }
        }

        yield return StartCoroutine(CoroutineUtils.WaitForAll(enumerators_prev));


        // Очистка текущих скринов
        ClearCurrenTextures();
        //yield return new WaitForSeconds(0.1f);


        // Загрузка текстур новых скринов
        for (int i = 0; i < savesPerPage; i++)
        {
            int actualSaveNum = GetActualSaveNum(i);

            if (SavesTaken.ContainsKey(actualSaveNum))
            {
                var texture = ES3.LoadImage($"{SaveSystemUtils.ScreenshotsFolder}/screenshot{actualSaveNum}.png");
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

        middle_actions.ForEach(x => x.Invoke());


        yield return StartCoroutine(CoroutineUtils.WaitForAll(enumerators_next));

        SaveManagerStatic.ClickBlocker = false;
    }

    public IEnumerator OverrideScreenshot(int saveNum, GameObject screenshot, GameObject overscreenshot, float speed)
    {
        SaveManagerStatic.DelayedSaveAction = true;
        int actualSaveNum = GetActualSaveNum(saveNum);
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

        string fileName = $"{SaveSystemUtils.ScreenshotsFolder}/screenshot{actualSaveNum}.png";
        if (ES3.FileExists(fileName))
        {
            ES3.DeleteFile(fileName);
        }

        currentTextures[saveNum] = texture;

        ES3.SaveImage(texture, fileName);

        screenshot.GetComponent<RawImage>().texture = texture;
        FadeManager.FadeObject(overscreenshot, false);
        overscreenshot.GetComponent<RawImage>().texture = null;
        SaveManagerStatic.DelayedSaveAction = false;
    }

    public IEnumerator SetScreenshot(int localSaveNum, GameObject screenshot)
    {
        SaveManagerStatic.DelayedSaveAction = true;

        int actualSaveNum = GetActualSaveNum(localSaveNum);
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

        yield return new WaitForSeconds(0.5f);

        currentTextures[localSaveNum] = texture;
        string fileName = $"{SaveSystemUtils.ScreenshotsFolder}/screenshot{actualSaveNum}.png";
        ES3.SaveImage(texture, fileName);
        SaveManagerStatic.DelayedSaveAction = false;
    }

    public void DeleteSave(int localSaveNum)
    {
        int actualSaveNum = GetActualSaveNum(localSaveNum);

        string screenshotFileName = $"{SaveSystemUtils.ScreenshotsFolder}/screenshot{actualSaveNum}.png";
        string fileName = $"{SaveSystemUtils.SaveFilesFolder}/{SaveSystemUtils.SaveFileSingleName}{actualSaveNum}.es3";

        try
        {
            if (ES3.FileExists(fileName) && ES3.FileExists(screenshotFileName))
            {
                ES3.DeleteFile(fileName);
                ES3.DeleteFile(screenshotFileName);
                if (SavesTaken.ContainsKey(actualSaveNum))
                {
                    SavesTaken.Remove(actualSaveNum);
                }
                SaveCurrentData();
            }
        }
        catch (Exception ex)
        {

        }
    }

    public void AddSaveData(int actualSaveNum, string datetime)
    {
        if (!SavesTaken.ContainsKey(actualSaveNum))
        {
            SavesTaken.Add(actualSaveNum, datetime);
        }
        else
        {
            SavesTaken[actualSaveNum] = datetime;
        }
        SaveCurrentData();
    }

    public void RemoveDateTime(int actualSaveNum)
    {
        if (SavesTaken.ContainsKey(actualSaveNum))
        {
            SavesTaken.Remove(actualSaveNum);
        }
    }

    public int GetActualSaveNum(int localSaveNum)
    {
        return currentPage * savesPerPage + localSaveNum;
    }

    private void SaveCurrentData()
    {
        ES3.Save<SaveManagerData>("SavesTaken", new SaveManagerData() { SavesTaken = SavesTaken }, SaveSystemUtils.GameData);
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
        bottomPages.StartingLoad();
        LoadFirstPage();

        SaveManagerStatic.ClickBlocker = false;
        SaveManagerStatic.UiBloker = false;
    }
}
