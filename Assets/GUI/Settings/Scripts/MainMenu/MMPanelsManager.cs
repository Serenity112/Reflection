using Fungus;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using static StaticVariables;

public class MMPanelsManager : MonoBehaviour
{
    public static MMPanelsManager instance = null;

    [SerializeField]
    private GameObject BlackPanel;

    [SerializeField]
    private GameObject ActivePanels;

    [SerializeField]
    private GameObject Buttons;

    [SerializeField]
    private GameObject AboutUs;

    [SerializeField]
    private float fadingSpeed = 5f;

    private AsyncOperationHandle<GameObject> savesPanelHandler;

    private AsyncOperationHandle<GameObject> aboutUsHandler;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        UpdateButtonsState();
    }

    // Кнопки меню
    private void UpdateButtonsState()
    {
        try
        {
            if (ES3.FileExists("SaveFiles.es3") && ES3.KeyExists("ContinueTrigger", "SaveFiles.es3"))
            {
                StaticVariables.MainMenuContinueButtonAnimationTrigger = ES3.Load<MMContinueButtonState>("ContinueTrigger", "SaveFiles.es3");
            }
        }
        catch (Exception)
        {
            ES3Utils.FileRecovery("ContinueTrigger", "SaveFiles.es3");
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
                ES3.Save<MMContinueButtonState>("ContinueTrigger", MMContinueButtonState.ButtonDeleted, "SaveFiles.es3");
                break;
            case MMContinueButtonState.AppearAnimation:
                StartCoroutine(IAppearContinueButton());
                StaticVariables.MainMenuContinueButtonAnimationTrigger = MMContinueButtonState.ButtonAdded;
                ES3.Save<MMContinueButtonState>("ContinueTrigger", MMContinueButtonState.ButtonAdded, "SaveFiles.es3");
                break;
            case MMContinueButtonState.ButtonAdded:
                AddContinueButton();
                break;
        }
    }

    private void AddContinueButton()
    {
        Buttons.GetComponent<Animator>().Play("ConstantAppear");
    }

    private void DeleteContinueButton()
    {
        Buttons.GetComponent<Animator>().Play("ConstantRemove");
    }

    private IEnumerator IAppearContinueButton()
    {
        yield return new WaitForSeconds(0.33f);
        Buttons.GetComponent<Animator>().Play("ContinueButtonAppear");
    }

    private IEnumerator IHideContinueButton()
    {
        yield return new WaitForSeconds(0.33f);
        Buttons.GetComponent<Animator>().Play("ContinueButtonRemove");
    }

    // Продолжить игру
    public void ContinueGame()
    {
        try
        {
            if (ES3.KeyExists("saveDataTimes", "SaveFiles.es3"))
            {
                string[] dates = ES3.Load<string[]>("saveDataTimes", "SaveFiles.es3");

                int last_index = 0;
                DateTime last_dt = new DateTime();

                for (int i_beg = 0; i_beg < dates.Length; i_beg++)
                {
                    if (dates[i_beg] != null && dates[i_beg] != string.Empty)
                    {
                        last_index = i_beg;
                        last_dt = DateTime.Parse(dates[i_beg]);
                        break;
                    }
                }

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
        }
        catch (Exception) // Cottupted save
        {
            ES3Utils.FileRecovery("saveDataTimes", "SaveFiles.es3");
            StartCoroutine(ILoadGame(-1));
        }
    }

    // Новая игра
    public void StartNewGame()
    {
        StartCoroutine(ILoadGame(-1));
    }

    // Сейвы
    public void OpenSaveMenu()
    {
        StartCoroutine(IOpenSaveMenu());
    }

    public void CloseSaveMenu()
    {
        StartCoroutine(ICloseSaveMenu());
    }

    private IEnumerator IOpenSaveMenu()
    {
        StaticVariables.ifInMainMenu = true;

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, fadingSpeed));
        MMButtonsManager.instance.unlineButtons();
        Buttons.SetActive(false);

        savesPanelHandler = Addressables.InstantiateAsync("SaveGuiPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return savesPanelHandler;

        if (savesPanelHandler.Status == AsyncOperationStatus.Succeeded)
        {
            savesPanelHandler.Result.name = "SaveGuiPanel";

            yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, fadingSpeed));
        }
        else
        {
            Debug.Log("Error loading");
        }
    }

    private IEnumerator ICloseSaveMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, fadingSpeed));

        Addressables.ReleaseInstance(savesPanelHandler);

        Buttons.SetActive(true);

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, fadingSpeed));

        yield return Resources.UnloadUnusedAssets();

        UpdateButtonsState();
    }

    public IEnumerator ILoadGame(int actualSaveNum)
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, fadingSpeed));

        StaticVariables.StartingLoadSaveFile = actualSaveNum;

        StaticVariables.ifInMainMenu = false;

        var asyncLoadLevel = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);

        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
    }

    // Меню "О нас"
    public void OpenInfoMenu()
    {
        StartCoroutine(IOpenInfoMenu());
    }

    private IEnumerator IOpenInfoMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, fadingSpeed));

        aboutUsHandler = Addressables.InstantiateAsync("AboutUs", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return aboutUsHandler;

        if (aboutUsHandler.Status == AsyncOperationStatus.Succeeded)
        {
            aboutUsHandler.Result.name = "AboutUs";

            yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, fadingSpeed));
        }
        else
        {
            Debug.Log("Error loading");
        }
    }

    public void CloseInfoMenu()
    {
        StartCoroutine(ICloseInfoMenu());
    }

    private IEnumerator ICloseInfoMenu()
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, fadingSpeed));

        Addressables.ReleaseInstance(aboutUsHandler);

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, fadingSpeed));

        yield return Resources.UnloadUnusedAssets();
    }

    // Выход из игры
    public void QuitGame()
    {
        Application.Quit();
    }
}
