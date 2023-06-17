using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using static StaticVariables;

public class MMPanelsManager : MonoBehaviour, IPanelsManager
{
    public static MMPanelsManager instance = null;

    public GameObject BlackPanel;

    public GameObject ActivePanels;

    public GameObject MainMenuButtons;

    public GameObject AboutUs;

    [SerializeField]
    private float FadingSpeed = 5f;

    private AsyncOperationHandle<GameObject> savesPanelHandler;

    private AsyncOperationHandle<GameObject> aboutUsHandler;

    private string SaveFilesData;

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

        SaveFilesData = SaveSystemUtils.SaveFilesData;

        StaticVariables.ifInMainMenu = true;
    }

    private void Start()
    {
        UpdateContinueButtonState();
    }

    public GameObject GetBlackPanel() => BlackPanel;

    public GameObject GetActivePanelsParent() => ActivePanels;

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
            //WarningPanel.instance.CreateWarningPanel(WarningPanel.SavingErrorMessage);
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

                int last_index = 0;
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

                StartCoroutine(ILoadGame(last_index));
            }
            else
            {
                // Кнопка "Продлжить" не может существовать в этом случае. Но на случай коррапта она дополнительно скрывается.
                DeleteContinueButton();
            }
        }
        catch (Exception)
        {
            //WarningPanel.instance.CreateWarningPanel(WarningPanel.SavingErrorMessage);
        }
    }

    // Новая игра
    public void StartNewGame() => StartCoroutine(ILoadGame(-1));

    // Сейвы
    public void OpenSaveMenu() => StartCoroutine(IOpenSaveMenu());

    public void CloseSaveMenu() => StartCoroutine(ICloseSaveMenu());

    private IEnumerator IOpenSaveMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));
        MMButtonsManager.instance.unlineButtons();
        MainMenuButtons.SetActive(false);

        savesPanelHandler = Addressables.InstantiateAsync("SaveGuiPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return savesPanelHandler;

        if (savesPanelHandler.Status == AsyncOperationStatus.Succeeded)
        {
            savesPanelHandler.Result.name = "SaveGuiPanel";

            yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));
        }
        else
        {
            Debug.Log("Error loading");
        }
    }

    private IEnumerator ICloseSaveMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));

        Addressables.ReleaseInstance(savesPanelHandler);

        MainMenuButtons.SetActive(true);

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));

        Resources.UnloadUnusedAssets();

        UpdateContinueButtonState();
    }

    public IEnumerator ILoadGame(int actualSaveNum)
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));

        StaticVariables.StartingLoadSaveFile = actualSaveNum;

        StaticVariables.ifInMainMenu = false;

        var asyncLoadLevel = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);

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

        aboutUsHandler = Addressables.InstantiateAsync("AboutUs", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return aboutUsHandler;

        if (aboutUsHandler.Status == AsyncOperationStatus.Succeeded)
        {
            aboutUsHandler.Result.name = "AboutUs";

            yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));
        }
        else
        {
            Debug.Log("Error loading");
        }
    }

    public void CloseInfoMenu() => StartCoroutine(ICloseInfoMenu());

    private IEnumerator ICloseInfoMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, FadingSpeed));

        Addressables.ReleaseInstance(aboutUsHandler);

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, FadingSpeed));

        Resources.UnloadUnusedAssets();
    }

    // Выход из игры
    public void QuitGame()
    {
        Application.Quit();
    }
}
