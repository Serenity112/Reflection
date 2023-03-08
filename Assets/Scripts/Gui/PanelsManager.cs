using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PanelsManager : MonoBehaviour
{
    public static PanelsManager instance = null;
    
    [SerializeField] private GameObject ActivePanels;

    public Camera GameCamera;

    public Camera PanelsCamera;

    [SerializeField] private GameObject blackPanelGame;  

    [SerializeField] private GameObject blackPanelPanels;

    public GameObject gameGuiPanel;

    public GameObject GameButtons;

    public GameObject ChatLog;

    public GameObject pausePanel;

    public GameObject PanelsCanvas;

    public GameObject GameCanvas;

    private GameObject savePanel;

    public Flowchart flowchart;

    [HideInInspector]
    public static bool confirmPanelActive = false;

    // handlers
    private AsyncOperationHandle<GameObject> savePanelHandler;




    [SerializeField]
    private float savesSpeed;

    void Awake()
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


    // SETTINGS



    // LOAD
    public IEnumerator ILoadGame(int saveNum)
    {
        FadeManager.FadeObject(blackPanelGame, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, savesSpeed));

        SaveManager.instance.ClearCurrent();
        Addressables.ReleaseInstance(savePanelHandler);

        ButtonsManager.unlinePauseButtons();
        PanelsCamera.enabled = false;

        Resources.UnloadUnusedAssets();
        UserData.instance.LoadPlayer(saveNum);

        FadeManager.FadeObject(blackPanelPanels, false);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelGame, false, savesSpeed));  
    }

    // SAVES
    public void openSaveMenu()
    {
        StartCoroutine(IopenSaveMenu());
    }
    public void closeSaveMenu()
    {
        //ButtonsManager.unlinePauseButtons();
        StartCoroutine(IcloseSaveMenu());
    }
    private IEnumerator IopenSaveMenu()
    {
        savePanelHandler = Addressables.InstantiateAsync("SaveGuiPanel", ActivePanels.GetComponent<RectTransform>(), false, true);
        yield return savePanelHandler;

        if (savePanelHandler.Status == AsyncOperationStatus.Succeeded)
        {
            FadeManager.FadeObject(blackPanelPanels, true);
            yield return StartCoroutine(FadeManager.FadeObject(blackPanelGame, true, savesSpeed));

            // Включаем игровое гуи и выключаем паузу для корректных скриншотов

            FadeManager.FadeObject(pausePanel, false);
            gameGuiPanel.GetComponent<CanvasGroup>().alpha = 1f;
            GameButtons.GetComponent<CanvasGroup>().alpha = 1f;
            PanelsCamera.enabled = true;

            FadeManager.FadeObject(blackPanelGame, false);
            StartCoroutine(FadeManager.FadeObject(blackPanelPanels, false, savesSpeed));
        }
        else
        {
            Debug.Log("Error loading");
        }
    }
    private IEnumerator IcloseSaveMenu()
    {
        FadeManager.FadeObject(blackPanelGame, true);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelPanels, true, savesSpeed));

        SaveManager.instance.ClearCurrent();
        Addressables.ReleaseInstance(savePanelHandler);

        ButtonsManager.unlinePauseButtons();
        FadeManager.FadeObject(pausePanel, true);
        gameGuiPanel.GetComponent<CanvasGroup>().alpha = 0f;
        GameButtons.GetComponent<CanvasGroup>().alpha = 0f;
        PanelsCamera.enabled = false;

        FadeManager.FadeObject(blackPanelPanels, false);
        yield return StartCoroutine(FadeManager.FadeObject(blackPanelGame, false, savesSpeed));

        Resources.UnloadUnusedAssets();
    }


}