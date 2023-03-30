using Fungus;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

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
        if (ES3.FileExists("SaveFiles.es3") && ES3.KeyExists("ContinueTrigger", "SaveFiles.es3"))
        {
            StaticVariables.MainMenuContinueButtonAnimationTrigger = ES3.Load<int>("ContinueTrigger", "SaveFiles.es3");
        }

        switch (StaticVariables.MainMenuContinueButtonAnimationTrigger)
        {
            case -1:
                RemoveContinueButton();
                break;
            case 0:
                AddContinueButton(); // Так как в сцене изначально кнопки нет
                StartCoroutine(IRemoveContinueButton());
                StaticVariables.MainMenuContinueButtonAnimationTrigger = -1;
                ES3.Save<int>("ContinueTrigger", -1, "SaveFiles.es3");
                break;
            case 1:
                StartCoroutine(IAddContinueButton());
                StaticVariables.MainMenuContinueButtonAnimationTrigger = 2;
                ES3.Save<int>("ContinueTrigger", 2, "SaveFiles.es3");
                break;
            case 2:
                AddContinueButton();
                break;
        }
    }

    private void AddContinueButton()
    {
        Buttons.GetComponent<Animator>().Play("ConstantAppear");
    }

    private void RemoveContinueButton()
    {
        Buttons.GetComponent<Animator>().Play("ConstantRemove");
    }

    private IEnumerator IAddContinueButton()
    {
        yield return new WaitForSeconds(0.33f);
        Buttons.GetComponent<Animator>().Play("ContinueButtonAppear");
    }

    private IEnumerator IRemoveContinueButton()
    {
        yield return new WaitForSeconds(0.33f);
        Buttons.GetComponent<Animator>().Play("ContinueButtonRemove");
    }

    // Продолжить игру
    public void ContinueGame()
    {
        if (ES3.KeyExists("saveDataTimes", "SaveFiles.es3"))
        {
            string[] dates = ES3.Load<string[]>("saveDataTimes", "SaveFiles.es3");

            int last_index = 0;
            DateTime last_dt = DateTime.Parse(dates[0]);

            for (int i = 0; i < dates.Length; i++)
            {
                if (dates[i] != null && dates[i] != string.Empty)
                {
                    DateTime dateTime = DateTime.Parse(dates[i]);
                    if (dateTime < last_dt)
                    {
                        last_dt = dateTime;
                        last_index = i;
                    }
                }
            }

            StartCoroutine(ILoadGame(last_index));
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

        Resources.UnloadUnusedAssets();

        UpdateButtonsState();
    }

    public IEnumerator ILoadGame(int actualSaveNum)
    {
        Debug.Log("actualSaveNum:" + actualSaveNum);

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
        StartCoroutine(FadeManager.FadeObject(AboutUs, true, fadingSpeed));
    }

    public void CloseInfoMenu()
    {
        StartCoroutine(FadeManager.FadeObject(AboutUs, false, fadingSpeed));
    }

    // Выход из игры
    public void QuitGame()
    {
        Application.Quit();
    }
}
