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

    public static bool GAME_LOADING { get; set; } = false;

    public GameObject PanelsCanvas;

    public GameObject GameCanvas;

    public GameObject OverlaysCanvas;

    public GameObject ActivePanels;

    public Camera GameCamera;

    public Camera PanelsCamera;

    public GameObject BlackPanel;

    public GameObject blackPanelPanels;

    public GameObject GameGuiPanel;

    public GameObject GameButtons;

    public GameObject ChatLog;

    public GameObject SaveGuiPanel;

    public Flowchart flowchart;

    private float speed = 5f;

    private void Awake()
    {
        instance = this;

        SaveGuiPanel = ActivePanels.transform.GetChild(1).gameObject;

        PanelsConfig.CurrentManager = this;

        StaticVariables.ifInMainMenu = false;

        DisableGuiOnStart();
    }

    #region GameButtons

    public void DisableGuiOnStart()
    {
        FadeManager.FadeObject(BlackPanel, true);
        FadeManager.FadeOnly(GameButtons, false);
        FadeManager.FadeOnly(GameGuiPanel, false);

        GameButtonsManager.instance.BlockButtonsClick = true;

        Typewriter.Instance.SetText("");
        NameChanger.instance.SetName(Character.None);
    }

    public IEnumerator EnableGuiOnStart(bool instant)
    {
        float fadeInSpeed = 3f;

        if (instant)
        {
            FadeManager.FadeOnly(GameButtons, true);
            FadeManager.FadeOnly(GameGuiPanel, true);
        }
        else
        {
            yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
            {
                FadeManager.FadeObject(GameButtons, true, fadeInSpeed),
                FadeManager.FadeObject(GameGuiPanel, true, fadeInSpeed)
            }));
        }

        GameButtonsManager.instance.BlockButtonsClick = false;

        yield return null;
    }

    #endregion

    #region SavePanel

    // Загрузка из сейв системы внутри игры
    public IEnumerator ILoadGame(int saveNum)
    {
        GAME_LOADING = true;

        FadeManager.FadeObject(BlackPanel, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, speed));

        SaveManager.instance.ClearCurrenTextures();
        SaveManager.instance.OnSaveClose();
        // Так как внутри ClearCurrent идёт чтение из файла, это ломает корутины почему-то...
        yield return new WaitForSeconds(0.1f);
        //PauseButtonsManager.instance.ResetManager();
        GameButtonsManager.instance.ResetManager();
        PanelsCamera.enabled = false;
        GameCamera.enabled = true;
        SaveGuiPanel.SetActive(false);

        // Выключаем меню паузы
        FadeManager.FadeOnly(PauseButtonsManager.instance.PausePanel, false);
        FadeManager.FadeObject(PauseButtonsManager.instance.PausePanel, false);

        yield return StartCoroutine(UserData.instance.ILoadGame(saveNum));
        yield return new WaitForSeconds(0.1f);

        FadeManager.FadeObject(blackPanelPanels, false);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));

        GAME_LOADING = false;
        StaticVariables.IN_SAVE_MENU = false;
        PauseButtonsManager.GAME_IS_PAUSED = false;
        PauseButtonsManager.GAME_IS_PAUSED_POST = false;
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

        FadeManager.FadeObject(SaveGuiPanel, true);
        GameCamera.enabled = false;
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
        GameCamera.enabled = true;

        FadeManager.FadeObject(SaveGuiPanel, false);
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
