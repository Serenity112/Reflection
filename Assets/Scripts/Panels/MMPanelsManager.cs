using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MMPanelsManager : MonoBehaviour, IPanelsManager
{
    public static MMPanelsManager instance = null;

    public GameObject BlackPanel;

    public GameObject SaveGuiPanel;

    public GameObject MainMenuButtons;

    public GameObject AboutUs;

    public GameObject GameLogo;

    [SerializeField] Camera GameCamera;

    private float FadingSpeed = 5f;

    private void Awake()
    {
        instance = this;

        PanelsConfig.CurrentManager = this;
    }

    private void Start()
    {
        MMButtonsManager.instance.gameObject.GetComponent<GraphicRaycaster>().enabled = false;

        UpdateContinueButtonState();

        // Флаг того, что произошёл переход игра => меню. Будет другая анимация, а не лого
        if (SaveManagerStatic.ifInMainMenu == false)
        {
            StartCoroutine(StartingGuiAnim());
        }
        else
        {
            StartCoroutine(LoadLogo());
        }

        SaveManagerStatic.ifInMainMenu = true;
    }

    // Переход из основной игры в главное меню
    private IEnumerator StartingGuiAnim()
    {
        MMButtonsManager.instance.gameObject.GetComponent<GraphicRaycaster>().enabled = false;
        FadeManager.FadeObject(BlackPanel, true);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));
        MMButtonsManager.instance.gameObject.GetComponent<GraphicRaycaster>().enabled = true;
    }

    // Лого игры, для первого запуcка
    private IEnumerator LoadLogo()
    {
        FadeManager.FadeObject(GameLogo, true);

        GameLogoAnimator animator = GameLogo.GetComponent<GameLogoAnimator>();

        yield return StartCoroutine(animator.IAnimate());
        yield return StartCoroutine(FadeManager.FadeObject(GameLogo, false, 3f));

        MMButtonsManager.instance.gameObject.GetComponent<GraphicRaycaster>().enabled = true;
    }

    public GameObject GetBlackPanel() => BlackPanel;

    public Camera GetGameCamera() => GameCamera;

    private void ContinueButtonEnable()
    {
        MMButtonsManager.instance.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void ContinueButtonDisable()
    {
        MMButtonsManager.instance.transform.GetChild(0).gameObject.SetActive(false);
    }

    // Состояние кнопки "Продолжить"
    private void UpdateContinueButtonState()
    {
        try
        {
            if (ES3.FileExists(SaveSystemUtils.GameData) && ES3.KeyExists("SavesTaken", SaveSystemUtils.GameData))
            {
                Dictionary<string, string> SavesTaken = ES3.Load<Dictionary<string, string>>("SavesTaken", SaveSystemUtils.GameData);

                if (SavesTaken != null && SavesTaken.Count > 0)
                {
                    ContinueButtonEnable();
                }
                else
                {
                    ContinueButtonDisable();
                }
            }
            else
            {
                ContinueButtonDisable();
            }
        }
        catch (Exception ex)
        {
            WarningPanel.instance.CreateWarningMessage(WarningPanelMessages.WarningTemplate.SaveSystemCorrupt, $" key: SavesTaken folder: {SaveSystemUtils.GameData}");
            SaveSystemUtils.DumpFile(SaveSystemUtils.GameData);
            ContinueButtonDisable();
        }
    }

    // Нажатие кнопки "Продолжиь игру"
    public void ContinueGame()
    {
        try
        {
            if (ES3.FileExists(SaveSystemUtils.GameData) && ES3.KeyExists("SavesTaken", SaveSystemUtils.GameData))
            {
                string format = "HH:mm:ss dd/MM/yy";
                Dictionary<int, string> data = ES3.Load<SaveManagerData>("SavesTaken", SaveSystemUtils.GameData).SavesTaken;
                DateTime lateDateTime = data.Max(pair => DateTime.ParseExact(pair.Value, format, null));
                int lateIndex = data.Where(pair => (DateTime.ParseExact(pair.Value, format, null) == lateDateTime)).First().Key;
                StartCoroutine(ILoadGameFromMainMenu(lateIndex));
            }
            else
            {
                MMButtonsManager.instance.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        catch (Exception ex)
        {
            ContinueButtonDisable();
            WarningPanel.instance.CreateWarningMessage(WarningPanelMessages.WarningTemplate.SaveSystemCorrupt, $" key: SavesTaken folder: {SaveSystemUtils.GameData}");
            SaveSystemUtils.DumpFile(SaveSystemUtils.GameData);
        }
    }

    // Новая игра
    public void StartNewGame() => StartCoroutine(ILoadGameFromMainMenu(-1));

    // Сейвы
    public void OpenSaveMenu() => StartCoroutine(IOpenSaveMenu());

    public void CloseSaveMenu() => StartCoroutine(ICloseSaveMenu());

    private IEnumerator IOpenSaveMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));
        FadeManager.FadeObject(SaveGuiPanel, true);

        SaveManager.instance.ResetManager();
        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));
        StaticVariables.IN_SAVE_MENU = true;
    }

    private IEnumerator ICloseSaveMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));

        FadeManager.FadeObject(SaveGuiPanel, false);
        SaveManager.instance.ClearCurrenTextures();

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));

        UpdateContinueButtonState();
        StaticVariables.IN_SAVE_MENU = false;
    }

    public IEnumerator ILoadGameFromMainMenu(int actualSaveNum)
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));

        SaveManagerStatic.StartingLoadSaveFile = actualSaveNum;

        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);

        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
    }

    // Меню "О нас"
    public void OpenInfoMenu() => StartCoroutine(IOpenInfoMenu());

    private IEnumerator IOpenInfoMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));
        FadeManager.FadeObject(AboutUs, true);

        AboutUsButtonManager.instance.ResetManager();

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));
        AboutUsButtonManager.IN_ABOUTUS_MENU = true;
    }

    public void CloseInfoMenu() => StartCoroutine(ICloseInfoMenu());

    private IEnumerator ICloseInfoMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));

        FadeManager.FadeObject(AboutUs, false);

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));
        AboutUsButtonManager.IN_ABOUTUS_MENU = false;
    }

    // Выход из игры
    public void QuitGame()
    {
        Application.Quit();
    }
}
