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

    [SerializeField] private GameObject ActivePanels;

    public Camera GameCamera;

    public Camera PanelsCamera;

    public GameObject BlackPanel;

    public GameObject blackPanelPanels;

    public GameObject GameGuiPanel;

    public GameObject GameButtons;

    public GameObject ChatLog;

    public GameObject pausePanel;

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
        WarningPanel.instance.ActivePanels = ActivePanels;
    }

    #region GameButtons

    // Метод для удаления заранее проставленных фраз из эдитора. В финальном билде удвлить метод + очистить текстовые поля в игре
    private void HidePresetData()
    {
        Typewriter.Instance.SetText("");
        NameChanger.instance.SetName(string.Empty);
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
        Typewriter.Instance.denySkip = false;
        GameButtons.GetComponent<GraphicRaycaster>().enabled = true;
    }

    public void GameButtonsDeactivate()
    {
        Typewriter.Instance.denySkip = true;
        GameButtons.GetComponent<GraphicRaycaster>().enabled = false;
    }

    #endregion

    // LOAD
    public IEnumerator ILoadGame(int saveNum)
    {
        FadeManager.FadeObject(BlackPanel, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, speed));

        SaveManager.instance.ClearCurrent();
        Addressables.ReleaseInstance(savePanelHandler);

        PauseButtonsManager.instance.unlinePauseButtons();
        PanelsCamera.enabled = false;

        Resources.UnloadUnusedAssets();

        yield return StartCoroutine(UserData.instance.ILoadGame(saveNum));

        FadeManager.FadeObject(blackPanelPanels, false);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));
    }

    // SAVES
    public void OpenSaveMenu()
    {
        StartCoroutine(IopenSaveMenu());
    }
    public void CloseSaveMenu()
    {
        //ButtonsManager.unlinePauseButtons();
        StartCoroutine(IcloseSaveMenu());
    }
    private IEnumerator IopenSaveMenu()
    {
        FadeManager.FadeObject(blackPanelPanels, true);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, speed));

        savePanelHandler = Addressables.InstantiateAsync("SaveGuiPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return savePanelHandler;

        if (savePanelHandler.Status == AsyncOperationStatus.Succeeded)
        {
            // Включаем игровое гуи и выключаем паузу для корректных скриншотов

            FadeManager.FadeObject(pausePanel, false);
            GameGuiPanel.GetComponent<CanvasGroup>().alpha = 1f;
            GameButtons.GetComponent<CanvasGroup>().alpha = 1f;
            PanelsCamera.enabled = true;
        }
        else
        {
            Debug.Log("Error loading");
        }

        FadeManager.FadeObject(BlackPanel, false);
        StartCoroutine(FadeManager.FadeObject(blackPanelPanels, false, speed));
    }
    private IEnumerator IcloseSaveMenu()
    {
        FadeManager.FadeObject(BlackPanel, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, speed));

        SaveManager.instance.ClearCurrent();
        Addressables.ReleaseInstance(savePanelHandler);

        PauseButtonsManager.instance.unlinePauseButtons();
        FadeManager.FadeObject(pausePanel, true);
        GameGuiPanel.GetComponent<CanvasGroup>().alpha = 0f;
        GameButtons.GetComponent<CanvasGroup>().alpha = 0f;
        PanelsCamera.enabled = false;

        FadeManager.FadeObject(blackPanelPanels, false);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));

        Resources.UnloadUnusedAssets();
    }

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
