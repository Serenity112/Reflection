using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelsManager : MonoBehaviour, IPanelsManager
{
    public static PanelsManager instance = null;

    public GameObject ActivePanels;

    public Camera GameCamera;

    public Camera PanelsCamera;

    public GameObject BlackPanel;

    public GameObject blackPanelPanels;

    public GameObject GameGuiPanel;

    public GameObject GameButtons;

    public GameObject ChatLog;

    public GameObject PanelsCanvas;

    public GameObject GameCanvas;

    private GameObject savePanel;

    public Flowchart flowchart;

    // handlers
    private AsyncOperationHandle<GameObject> savePanelHandler;

    private float speed = 5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        PanelsConfig.CurrentManager = this;

        StaticVariables.ifInMainMenu = false;

        DisableGuiOnStart();
    }

    private void Start()
    {
        ConfirmationPanel.instance.ActivePanels = ActivePanels;
        // WarningPanel.instance.ActivePanels = ActivePanels;

        StartCoroutine(IPreloadSavesPanel());
    }

    #region GameButtons

    // Метод для удаления заранее проставленных фраз из эдитора. В финальном билде удвлить метод + очистить текстовые поля в игре
    private void HidePresetData()
    {
        Typewriter.Instance.SetText("");
        NameChanger.instance.SetName(Character.None);
    }

    public void DisableGuiOnStart()
    {
        FadeManager.FadeObject(BlackPanel, true);
        GameButtons.GetComponent<CanvasGroup>().alpha = 0f;
        GameGuiPanel.GetComponent<CanvasGroup>().alpha = 0f;

        GameButtonsDeactivate();

        HidePresetData();
    }

    public IEnumerator EnableGuiOnStart(bool instant)
    {
        float fadeInSpeed = 3f;

        if (instant)
        {
            GameButtons.GetComponent<CanvasGroup>().alpha = 1f;
            GameGuiPanel.GetComponent<CanvasGroup>().alpha = 1f;
        }
        else
        {
            yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
            {
                FadeManager.FadeObject(GameButtons, true, fadeInSpeed),
                FadeManager.FadeObject(GameGuiPanel, true, fadeInSpeed)
            }));
        }

        GameButtonsActivate();

        yield return null;
    }

    public void GameButtonsActivate()
    {
        GameButtons.GetComponent<GraphicRaycaster>().enabled = true;
    }

    public void GameButtonsDeactivate()
    {
        GameButtons.GetComponent<GraphicRaycaster>().enabled = false;
    }

    #endregion

    #region SavePanel

    // Загрузка из сейв системы внутри игры
    public IEnumerator ILoadGame(int saveNum)
    {
        StaticVariables.GAME_LOADING = true;

        FadeManager.FadeObject(BlackPanel, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, speed));

        SaveManager.instance.ClearCurrenTextures();
        SaveManager.instance.OnSaveClose();
        // Так как внутри ClearCurrent идёт чтение из файла, это ломает корутины почему-то...
        yield return new WaitForSeconds(0.1f);
        //PauseButtonsManager.instance.ResetManager();
        PanelsCamera.enabled = false;
        savePanelHandler.Result.SetActive(false);

        // Выключаем меню паузы
        FadeManager.FadeOnly(PauseButtonsManager.instance.PausePanel, false);
        FadeManager.FadeObject(PauseButtonsManager.instance.PausePanel, false);

        yield return StartCoroutine(UserData.instance.ILoadGame(saveNum));
        yield return new WaitForSeconds(0.1f);

        FadeManager.FadeObject(blackPanelPanels, false);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));

        StaticVariables.GAME_LOADING = false;
        StaticVariables.IN_SAVE_MENU = false;
        StaticVariables.PAUSED = false;
        StaticVariables.PAUSE_ANIM_ENDED = false;
    }

    private IEnumerator IPreloadSavesPanel()
    {
        savePanelHandler = Addressables.InstantiateAsync("SaveGuiPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return savePanelHandler;

        if (savePanelHandler.Status == AsyncOperationStatus.Succeeded)
        {
            savePanelHandler.Result.name = "SaveGuiPanel";
            savePanelHandler.Result.SetActive(false);
        }
    }

    public void OpenSaveMenu() => StartCoroutine(IopenSaveMenu());

    public void CloseSaveMenu() => StartCoroutine(IcloseSaveMenu());

    private IEnumerator IopenSaveMenu()
    {
        FadeManager.FadeObject(blackPanelPanels, true);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, speed));
        yield return new WaitForEndOfFrame();

        // Для корректных скринщотов -----
        FadeManager.FadeOnly(PauseButtonsManager.instance.PausePanel, false);
        GameGuiPanel.GetComponent<CanvasGroup>().alpha = 1f;
        GameButtons.GetComponent<CanvasGroup>().alpha = 1f;
        // -----

        savePanelHandler.Result.SetActive(true);
        PanelsCamera.enabled = true;
        SaveManager.instance.InitialReset();
        // Так как внутри InitialReset идёт чтение из файла, это ломает корутины почему-то...
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, false, speed));
        FadeManager.FadeObject(BlackPanel, false);

        StaticVariables.IN_SAVE_MENU = true;
    }

    private IEnumerator IcloseSaveMenu()
    {
        FadeManager.FadeObject(BlackPanel, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, speed));

        PanelsCamera.enabled = false;
        savePanelHandler.Result.SetActive(false);
        PauseButtonsManager.instance.ResetManager();
        SaveManager.instance.ClearCurrenTextures();
        SaveManager.instance.OnSaveClose();

        FadeManager.FadeOnly(PauseButtonsManager.instance.PausePanel, true);
        GameGuiPanel.GetComponent<CanvasGroup>().alpha = 0f;
        GameButtons.GetComponent<CanvasGroup>().alpha = 0f;

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));
        FadeManager.FadeObject(blackPanelPanels, false);

        StaticVariables.IN_SAVE_MENU = false;

        Resources.UnloadUnusedAssets();
    }

    public void ReleaseSaveMenu()
    {
        Addressables.ReleaseInstance(savePanelHandler);
    }

    #endregion

    // Выход в главное меню
    public void QuitToMainMenu()
    {
        StartCoroutine(IQuitToMainMenu());
    }

    private IEnumerator IQuitToMainMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, 5f));

        var asyncLoadLevel = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);

        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
    }

    public GameObject GetBlackPanel() => BlackPanel;

    public GameObject GetActivePanelsParent() => ActivePanels;

    public Camera GetGameCamera() => GameCamera;
}
