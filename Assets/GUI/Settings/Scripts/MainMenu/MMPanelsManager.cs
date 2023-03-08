using System.Collections;
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
    private float fadingSpeed = 5f;

    private AsyncOperationHandle<GameObject> savesPanelHandler;

    private int lastUsedSave = -1;

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

        switch (StaticVariables.MainMenuContinueButtonAnimationTrigger)
        {
            case -1:
                RemoveContinueButton(); 
                break;
            case 0:
                StartCoroutine(IRemoveContinueButton());
                StaticVariables.MainMenuContinueButtonAnimationTrigger = -1;
                break;
            case 1:
                StartCoroutine(IAddContinueButton());
                StaticVariables.MainMenuContinueButtonAnimationTrigger = 2;
                break;
            case 2:
                AddContinueButton();
                break;
        }
    }

    // Кнопки меню
    private void AddContinueButton()
    {
        GameObject ContinueButto = Buttons.transform.GetChild(0).gameObject;
        ContinueButto.SetActive(true);
        ContinueButto.GetComponent<CanvasGroup>().alpha = 1;

        GameObject StaticButtons = Buttons.transform.GetChild(1).gameObject;
        StaticButtons.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -53);
    }

    private void RemoveContinueButton()
    {
        GameObject ContinueButton = Buttons.transform.GetChild(0).gameObject;
        ContinueButton.SetActive(false);
        ContinueButton.GetComponent<CanvasGroup>().alpha = 0;

        GameObject StaticButtons = Buttons.transform.GetChild(1).gameObject;
        StaticButtons.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    }

    private IEnumerator IAddContinueButton()
    {
        yield return new WaitForSeconds(1f);
        Buttons.GetComponent<Animator>().Play("ContinueButtonAppear");
    }

    private IEnumerator IRemoveContinueButton()
    {
        yield return new WaitForSeconds(1f);
        Buttons.GetComponent<Animator>().Play("ContinueButtonRemove");
    }

    // Новая игра
    public void StartNewGame()
    {

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

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, fadingSpeed));

        Resources.UnloadUnusedAssets();
    }

    public IEnumerator ILoadGame(int saveNum)
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;

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

    }

    public void CloseInfoMenu()
    {

    }

    // Выход из игры
    public void QuitGame()
    {
        Application.Quit();
    }
}
