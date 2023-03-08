using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using System.Threading.Tasks;
using Unity.VisualScripting;

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

    public int currentPage = 0;

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

        GameObject Files = transform.GetChild(0).gameObject;

        for (int i = 0; i < savesPerPage; i++)
        {
            GameObject file = Files.transform.GetChild(i).gameObject;
            allFiles[i] = file;
            allScreenshots[i] = file.GetComponent<SaveFileFields>().screenshot;
        }

        if(!StaticVariables.ifInMainMenu)
        {
            GameCamera = PanelsManager.instance.GameCamera;
        }
        
        if (ES3.KeyExists("saveTaken", "screenshots.es3"))
        {
            savesTaken = ES3.Load<bool[]>("saveTaken", "screenshots.es3");
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
                } else
                {
                    FadeManager.FadeObject(savedPanel, true);
                }           
                
                saveFileFields.overPanel.GetComponent<CanvasGroup>().alpha = 1f; // Overpanel

                var texture = ES3.LoadImage("screenshots/screenshot" + saveNum + ".png");
                texture.name = "screenshot" + saveNum;
                currentTextures[saveNum] = texture;

                screenshot.GetComponent<RawImage>().texture = texture;

                screenshot.GetComponent<CanvasGroup>().alpha = 1f;
                saveFileFields.AllowSaveLoad = true;
                saveFileFields.AllowOverPanel = true;
            }
            else
            {
                screenshot.GetComponent<RawImage>().texture = null;
                currentTextures[saveNum] = null;

                screenshot.GetComponent<CanvasGroup>().alpha = 0f;
                if(!StaticVariables.ifInMainMenu)
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

            StartCoroutine(FadeManager.FadeObject(overPanel, false, pagesScrollSpeed)); // Overpanel
            StartCoroutine(FadeManager.FadeObject(savedPanel, false, pagesScrollSpeed));
            FadeManager.FadeObject(unsavedPanel, false);

            if (i != firstActiveSave)
                StartCoroutine(FadeManager.FadeObject(allScreenshots[i], false, pagesScrollSpeed));
        }
        if (firstActiveSave != -1)
            yield return StartCoroutine(FadeManager.FadeObject(allScreenshots[firstActiveSave], false, pagesScrollSpeed));

        // Очистка текущих скринов
        ClearCurrent();

        // Загрузка новых скринов
        for (int i = 0; i < savesPerPage; i++)
        {
            int saveNum = pageToLoad * savesPerPage + i;

            if (savesTaken[saveNum])
            {
                var texture = ES3.LoadImage("screenshots/screenshot" + saveNum + ".png");
                texture.name = "screenshot" + saveNum;
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

        firstActiveSave = -1;
        for (int i = 0; i < savesPerPage; i++)
        {
            if (savesTaken[pageToLoad * savesPerPage + i])
            {
                firstActiveSave = i;
                break;
            }
        }

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
                if (i != firstActiveSave)
                    StartCoroutine(FadeManager.FadeOnly(allScreenshots[i], true, pagesScrollSpeed));

                if(StaticVariables.ifInMainMenu)
                {
                    StartCoroutine(FadeManager.FadeObject(MainMenuPanel, true, pagesScrollSpeed));
                } else
                {
                    StartCoroutine(FadeManager.FadeObject(savedPanel, true, pagesScrollSpeed));
                }

                FadeManager.FadeObject(unsavedPanel, false);

                StartCoroutine(FadeManager.FadeObject(saveFileFields.overPanel, true, pagesScrollSpeed)); // Overpanel
                //saveFileFields.overPanel.GetComponent<CanvasGroup>().alpha = 1f; 
                saveFileFields.AllowSaveLoad = true;
                saveFileFields.AllowOverPanel = true;
            }
            else
            {
                allScreenshots[i].GetComponent<CanvasGroup>().alpha = 0f;
                if (StaticVariables.ifInMainMenu)
                {
                    FadeManager.FadeObject(MainMenuPanel, false);
                } else
                {
                    FadeManager.FadeObject(savedPanel, false);
                    FadeManager.FadeObject(unsavedPanel, true);
                }
               
                saveFileFields.overPanel.GetComponent<CanvasGroup>().alpha = 0f; // Overpanel
                saveFileFields.AllowSaveLoad = false;
                saveFileFields.AllowOverPanel = false;
            }
        }
        if (firstActiveSave != -1)
            yield return StartCoroutine(FadeManager.FadeOnly(allScreenshots[firstActiveSave], true, pagesScrollSpeed));
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

        if (ES3.FileExists("screenshots/screenshot" + actualSaveNum + ".png"))
        {
            ES3.DeleteFile("screenshots/screenshot" + actualSaveNum + ".png");
        }


        currentTextures[saveNum] = texture;

        ES3.SaveImage(texture, "screenshots/screenshot" + actualSaveNum + ".png");

        savesTaken[currentPage * savesPerPage + saveNum] = true;
        ES3.Save<bool[]>("saveTaken", savesTaken, "screenshots.es3");


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

        ES3.SaveImage(texture, "screenshots/screenshot" + actualSaveNum + ".png");

        savesTaken[currentPage * savesPerPage + saveNum] = true;
        ES3.Save<bool[]>("saveTaken", savesTaken, "screenshots.es3");

        StaticVariables.UIsystemDown = false;
    }

    public void CloseSave()
    {
        if(StaticVariables.ifInMainMenu) 
        {
            MMPanelsManager.instance.CloseSaveMenu();
        } else
        {
            PanelsManager.instance.closeSaveMenu();
        }    
    }
}
