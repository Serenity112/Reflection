using Fungus;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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
    }

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

        StaticVariables.StartingLoadIndex = actualSaveNum;

        StaticVariables.ifInMainMenu = false;

        var asyncLoadLevel = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);

        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
    }
}
