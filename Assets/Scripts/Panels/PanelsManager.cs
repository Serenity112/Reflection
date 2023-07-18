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

    // Сделать главной
    public GameObject BlackPanel;

    // УДАЛИТЬ
    public GameObject blackPanelPanels;

    public GameObject gameGuiPanel;

    public GameObject GameButtons;

    public GameObject ChatLog;

    public GameObject pausePanel;

    public GameObject PanelsCanvas;

    public GameObject GameCanvas;

    private GameObject savePanel;

    public Flowchart flowchart;

    // handlers
    private AsyncOperationHandle<GameObject> savePanelHandler;

    [SerializeField]
    private float savesSpeed;

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
    }

    private void Start()
    {
        ConfirmationPanel.instance.ActivePanels = ActivePanels;
        WarningPanel.instance.ActivePanels = ActivePanels;
    }

    // LOAD
    public IEnumerator ILoadGame(int saveNum)
    {
        FadeManager.FadeObject(BlackPanel, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, savesSpeed));

        SaveManager.instance.ClearCurrent();
        Addressables.ReleaseInstance(savePanelHandler);

        ButtonsManager.instance.unlinePauseButtons();
        PanelsCamera.enabled = false;

        Resources.UnloadUnusedAssets();

        StartCoroutine(UserData.instance.ILoadGame(saveNum));
        // yield return'ы не работают тут по какой-то причине
        //yield return new WaitForSeconds(0.5f);

        FadeManager.FadeObject(blackPanelPanels, false);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, savesSpeed));
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
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, savesSpeed));

        savePanelHandler = Addressables.InstantiateAsync("SaveGuiPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return savePanelHandler;

        if (savePanelHandler.Status == AsyncOperationStatus.Succeeded)
        {
            // Включаем игровое гуи и выключаем паузу для корректных скриншотов

            FadeManager.FadeObject(pausePanel, false);
            gameGuiPanel.GetComponent<CanvasGroup>().alpha = 1f;
            GameButtons.GetComponent<CanvasGroup>().alpha = 1f;
            PanelsCamera.enabled = true;
        }
        else
        {
            Debug.Log("Error loading");
        }

        FadeManager.FadeObject(BlackPanel, false);
        StartCoroutine(FadeManager.FadeObject(blackPanelPanels, false, savesSpeed));
    }
    private IEnumerator IcloseSaveMenu()
    {
        FadeManager.FadeObject(BlackPanel, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, savesSpeed));

        SaveManager.instance.ClearCurrent();
        Addressables.ReleaseInstance(savePanelHandler);

        ButtonsManager.instance.unlinePauseButtons();
        FadeManager.FadeObject(pausePanel, true);
        gameGuiPanel.GetComponent<CanvasGroup>().alpha = 0f;
        GameButtons.GetComponent<CanvasGroup>().alpha = 0f;
        PanelsCamera.enabled = false;

        FadeManager.FadeObject(blackPanelPanels, false);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, savesSpeed));

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

        StaticVariables.ifInMainMenu = true;

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
