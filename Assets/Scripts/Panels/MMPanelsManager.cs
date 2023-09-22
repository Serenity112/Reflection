using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static StaticVariables;

public class MMPanelsManager : MonoBehaviour, IPanelsManager
{
    public static MMPanelsManager instance = null;

    public GameObject BlackPanelMenu;

    public GameObject GrayPanelLogo;

    public GameObject ActivePanels;

    public GameObject MainMenuButtons;

    public GameObject AboutUs;

    [SerializeField] Camera GameCamera;

    private AsyncOperationHandle<GameObject> savesPanelHandler;

    private AsyncOperationHandle<GameObject> aboutUsHandler;

    private string SaveFilesData;

    // Скорости
    private float FadingSpeed = 5f;

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

        // Иначе нет гарантии, что ActivePanels внутри WarningPanel проиницализируется раньше, чем скрипт обратится к ней
        SaveFilesData = SaveSystemUtils.SaveFilesData;
    }

    private void Start()
    {
        ConfirmationPanel.instance.ActivePanels = ActivePanels;
        WarningPanel.instance.ActivePanels = ActivePanels;
        UpdateContinueButtonState();

        MMButtonsManager.instance.gameObject.GetComponent<GraphicRaycaster>().enabled = false;
        // Флаг того, что произошёл переход игра => меню. В этом случае нужна доп. анимация
        if (StaticVariables.ifInMainMenu == false)
        {
            StartCoroutine(StartingGuiAnim());
        }
        else
        {
            StartCoroutine(LoadLogo());
        }

        StaticVariables.ifInMainMenu = true;
    }
    
    // Переход из основной игры в главное меню
    private IEnumerator StartingGuiAnim()
    {
        FadeManager.FadeObject(BlackPanelMenu, true);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, false, FadingSpeed));
        MMButtonsManager.instance.gameObject.GetComponent<GraphicRaycaster>().enabled = true;
    }

    // Лого игры, для первого запуcrf
    private IEnumerator LoadLogo()
    {
        float logoSpeed = 1.0f;

        FadeManager.FadeObject(GrayPanelLogo, true);

        var handler = Addressables.InstantiateAsync("GameLogo", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return handler;

        FadeManager.FadeObject(GrayPanelLogo, false);

        GameObject result = handler.Result;
        GameObject UnityBG = result.transform.GetChild(0).gameObject;
        GameObject LogoNew = result.transform.GetChild(1).gameObject;
        GameObject LogoOld = result.transform.GetChild(2).gameObject;
        GameObject WRLogo = result.transform.GetChild(3).gameObject;

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>() {
            FadeManager.FadeObject(WRLogo, true, logoSpeed),
            FadeManager.FadeObject(UnityBG, false, logoSpeed)
        }));

        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(FadeManager.FadeObject(WRLogo, false, logoSpeed));

        yield return StartCoroutine(FadeManager.FadeObject(LogoNew, true, logoSpeed));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(FadeManager.FadeObject(LogoOld, true, logoSpeed));
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, true, logoSpeed));
        yield return Addressables.ReleaseInstance(handler);
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, false, logoSpeed));

        yield return new WaitForSeconds(0.5f);
        MMButtonsManager.instance.gameObject.GetComponent<GraphicRaycaster>().enabled = true;
    }

    public GameObject GetBlackPanel() => BlackPanelMenu;

    public GameObject GetActivePanelsParent() => ActivePanels;

    public Camera GetGameCamera() => GameCamera;

    // Состояние кнопки "Продолжить"
    private void UpdateContinueButtonState()
    {
        try
        {
            if (ES3.FileExists(SaveFilesData) && ES3.KeyExists("ContinueTrigger", SaveFilesData))
            {
                StaticVariables.MainMenuContinueButtonAnimationTrigger = ES3.Load<MMContinueButtonState>("ContinueTrigger", SaveFilesData);
            }
        }
        catch (Exception)
        {
            WarningPanel.instance.CreateWarningPanel(WarningPanel.SavingErrorMessage);
        }

        switch (StaticVariables.MainMenuContinueButtonAnimationTrigger)
        {
            case MMContinueButtonState.ButtonDeleted:
                DeleteContinueButton();
                break;
            case MMContinueButtonState.HideAnimation:
                AddContinueButton(); // Так как в сцене изначально кнопки нет, для анимации исчезания
                StartCoroutine(IHideContinueButton());
                StaticVariables.MainMenuContinueButtonAnimationTrigger = MMContinueButtonState.ButtonDeleted;
                ES3.Save<MMContinueButtonState>("ContinueTrigger", MMContinueButtonState.ButtonDeleted, SaveFilesData);
                break;
            case MMContinueButtonState.AppearAnimation:
                StartCoroutine(IAppearContinueButton());
                StaticVariables.MainMenuContinueButtonAnimationTrigger = MMContinueButtonState.ButtonAdded;
                ES3.Save<MMContinueButtonState>("ContinueTrigger", MMContinueButtonState.ButtonAdded, SaveFilesData);
                break;
            case MMContinueButtonState.ButtonAdded:
                AddContinueButton();
                break;
        }
    }

    // Анимации кнопки "Продолжить"
    private void AddContinueButton()
    {
        MainMenuButtons.GetComponent<Animator>().Play("ConstantAppear");
    }

    private void DeleteContinueButton()
    {
        MainMenuButtons.GetComponent<Animator>().Play("ConstantRemove");
    }

    private IEnumerator IAppearContinueButton()
    {
        yield return new WaitForSeconds(0.33f);
        MainMenuButtons.GetComponent<Animator>().Play("ContinueButtonAppear");
    }

    private IEnumerator IHideContinueButton()
    {
        yield return new WaitForSeconds(0.33f);
        MainMenuButtons.GetComponent<Animator>().Play("ContinueButtonRemove");
    }

    // Нажатие кнопки "Продолжиь игру"
    public void ContinueGame()
    {
        try
        {
            if (ES3.FileExists(SaveFilesData) && ES3.KeyExists("saveDataTimes", SaveFilesData))
            {
                string[] dates = ES3.Load<string[]>("saveDataTimes", SaveFilesData);

                int last_index = -1;
                DateTime last_dt = new DateTime();

                // Определение первого last_dt, чтобы далее с ним сравнивать
                for (int i = 0; i < dates.Length; i++)
                {
                    if (dates[i] != null && dates[i] != string.Empty)
                    {
                        last_index = i;
                        last_dt = DateTime.Parse(dates[i]);
                        break;
                    }
                }

                // Поиск минимальной даты начиная со следующей после найденной в предыдущем цикле
                for (int i = last_index + 1; i < dates.Length; i++)
                {
                    if (dates[i] != null && dates[i] != string.Empty)
                    {
                        DateTime dateTime = DateTime.Parse(dates[i]);

                        if (dateTime > last_dt)
                        {
                            last_dt = dateTime;
                            last_index = i;
                        }
                    }
                }

                StartCoroutine(ILoadGameFromMainMenu(last_index));
            }
            else
            {
                // Кнопка "Продлжить" не может существовать в этом случае. Но на случай коррапта она дополнительно скрывается.
                DeleteContinueButton();
            }
        }
        catch (Exception)
        {
            WarningPanel.instance.CreateWarningPanel(WarningPanel.SavingErrorMessage);
        }
    }

    // Новая игра
    public void StartNewGame() => StartCoroutine(ILoadGameFromMainMenu(-1));

    // Сейвы
    public void OpenSaveMenu() => StartCoroutine(IOpenSaveMenu());

    public void CloseSaveMenu() => StartCoroutine(ICloseSaveMenu());

    private IEnumerator IOpenSaveMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, true, FadingSpeed));
        MMButtonsManager.instance.UnSelectButtons();

        savesPanelHandler = Addressables.InstantiateAsync("SaveGuiPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return savesPanelHandler;

        if (savesPanelHandler.Status == AsyncOperationStatus.Succeeded)
        {
            savesPanelHandler.Result.name = "SaveGuiPanel";

            yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, false, FadingSpeed));
        }
        else
        {
            Debug.Log("Error loading");
        }
    }

    private IEnumerator ICloseSaveMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, true, FadingSpeed));

        Addressables.ReleaseInstance(savesPanelHandler);

        MMButtonsManager.instance.EnableButtons();

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, false, FadingSpeed));

        Resources.UnloadUnusedAssets();

        UpdateContinueButtonState();
    }

    public void ReleaseSaveMenu()
    {
        Addressables.ReleaseInstance(savesPanelHandler);
    }

    public IEnumerator ILoadGameFromMainMenu(int actualSaveNum)
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, true, FadingSpeed));

        StaticVariables.StartingLoadSaveFile = actualSaveNum;

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
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, true, FadingSpeed));

        aboutUsHandler = Addressables.InstantiateAsync("AboutUs", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return aboutUsHandler;

        if (aboutUsHandler.Status == AsyncOperationStatus.Succeeded)
        {
            aboutUsHandler.Result.name = "AboutUs";

            yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, false, FadingSpeed));
        }
        else
        {
            Debug.Log("Error loading");
        }
    }

    public void CloseInfoMenu() => StartCoroutine(ICloseInfoMenu());

    private IEnumerator ICloseInfoMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, true, FadingSpeed));

        Addressables.ReleaseInstance(aboutUsHandler);

        MMButtonsManager.instance.EnableButtons();

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanelMenu, false, FadingSpeed));

        Resources.UnloadUnusedAssets();
    }

    // Выход из игры
    public void QuitGame()
    {
        Application.Quit();
    }
}
