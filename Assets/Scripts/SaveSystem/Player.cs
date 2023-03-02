using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fungus;

public class Player : MonoBehaviour
{
    public static Player instance = null;

    public Animator SceneAnimator;
    
    //ROOTS
    public int root1 = 7;
    public int root2 = 8;
    public int root3 = 9;
    public int root4 = 10;

    //SPRITES
    private SpriteController spritecontroller;
    PackageConntector packageConntector;

    // Текущие значения
    public static int CurrentBG = -1;
    public static string CurrentBlock;
    //public static string SaveBlock;

    public static int CurrentCommandIndex;
    public static int CurrentDialogIndex;


    public static int indexToLoad = -1;

    public Button referenceToTheButton;

    public static GameObject BlackPanel;
    public GameObject GameGuiPanel;
    public GameObject CameraManager;
    public GameObject Sprites;
    BackgroundManager backgroundManager;

    public AudioManager audiomanager;

    public static GameObject PanelsCanvas;
    //LOG
    public static List<string> LogBlocks;


    //LoadBlock
    public static Flowchart targetFlowchart;


    private void Awake()
    {
        GameObject SpriteManager = GameObject.Find("SpriteManager");
        spritecontroller = SpriteManager.GetComponent<SpriteController>();
        packageConntector = SpriteManager.GetComponent<PackageConntector>();

        PanelsCanvas = GameObject.Find("PanelsCanvas");

        audiomanager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        targetFlowchart = GameObject.Find("Episode1").GetComponent<Flowchart>();

        backgroundManager = GameObject.Find("BackGroundManager").GetComponent<BackgroundManager>();
    }
    public void Start()
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

    public static void DenySkip()
    {
        PanelsCanvas.GetComponent<DialogMod>().DenySkip();
    }

    public void PreloadSavesPictures()
    {
        CameraManager.GetComponent<ScreenRecorder>().Load();
    }

    public static void IncreaseIndex(int index)
    {
        CurrentCommandIndex += index;
    }

    public static void StartingLoad()
    {
       // cam.GetComponent<ScreenRecorder>().StartingLoad();
        //GameObject.Find("Audio").GetComponent<AudioManager>().LoadSliders();
    }

   

    public static void LoadGame(int SaveNum)
    {
       instance.StartCoroutine(ILoadGame(SaveNum));
    }

    public static IEnumerator ILoadGame(int SaveNum)
    {
        UnityEngine.Debug.Log("Loading game");
        yield return instance.StartCoroutine(FadeManager.FadeObject(BlackPanel, true, 1.5f));

        //SceneAnimator.SetTrigger("LoadGame");

        //Unloading
        //spritecontroller.UnloadSprites();
        // CameraManager.GetComponent<ScreenRecorder>().Unload();


        //Loading

        //LoadPlayer(SaveNum);

        //yield return new WaitForSeconds(0.3f); //Время для загрузки спрайтов, фонов
        //yield return StartCoroutine(FadeBlackScreen(false, 1.5f));    

        yield return null;
    }

    public static void SavePlayer(int SaveNum)
    {
        UnityEngine.Debug.Log("Game saved! " + CurrentBlock + " " + CurrentCommandIndex + " " + CurrentDialogIndex);

        SaveData newSave = new SaveData(SaveNum);

        newSave.Background = CurrentBG;
        newSave.CurrentBlock = CurrentBlock;

        newSave.CurrentCommandIndex = CurrentCommandIndex;
        newSave.CurrentDialogIndex = CurrentDialogIndex;

        newSave.SpriteData = instance.spritecontroller.GameSpriteData;

        ES3.Save<SaveData>("SaveFile" + SaveNum, newSave, "SaveFiles.es3");

        //newSave.CurrentOst = audiomanager.currentOst;
        //
        //newSave.LogBlocks = LogBlocks;
 
    }

    public static void LoadPlayer(int SaveNum)
    {
        
    }

    static void LoadBlock(string CurrentBlock, int CurrentIndex)
    {
      

        //IncreaseIndex(-1);
        Block targetBlock = targetFlowchart.FindBlock(CurrentBlock);

        if (targetBlock != null)
        {
            targetBlock.Stop();

            PanelsCanvas.GetComponent<Writer>().game_loading = true;

            targetFlowchart.ExecuteBlock(targetBlock, CurrentIndex, null);
        }
    }

    IEnumerator LoadAudio(int ost)
    {
        StartCoroutine(audiomanager.IAmbientEnd(1f));
        yield return StartCoroutine(audiomanager.IMusicEnd(1f));

        audiomanager.MusicStart(ost, 1f);
    }

    IEnumerator FadeBlackScreen(bool fadein, float speed)
    {
        BlackPanel.SetActive(true);

        if (fadein)
        {
            BlackPanel.GetComponent<CanvasGroup>().alpha = 0;
            int color = 1;
            for (float i = 0; i <= color + 0.05; i += speed * Time.deltaTime)
            {
                BlackPanel.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
        }
        else
        {
            BlackPanel.GetComponent<CanvasGroup>().alpha = 1;
            int color = 0;
            for (float i = 1; i >= color - 0.05; i -= speed * Time.deltaTime)
            {
                BlackPanel.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
            BlackPanel.SetActive(false);
        }

    }
}