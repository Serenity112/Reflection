using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using System;

public struct SaveData
{
    public SaveData(int saveNum)
    {
        this.saveNum = saveNum;
        SpriteData = new SpriteData[4];
        Background = null;

        CurrentCommandIndex = 0;

        CurrentBlock = null;

        CurrentMusic = null;
        CurrentAmbient = null;
        MusicSourceVolume = 1;
        AmbientSourceVolume = 1;

        LogBlocks = new List<string>();

        specialEvent = SpecialEvent.none;
        specialEventState = 0;
    }

    int saveNum;

    public SpriteData[] SpriteData;
    public string Background;
    public int CurrentCommandIndex;
    public string CurrentBlock;

    // Music
    public string CurrentMusic;
    public string CurrentAmbient;
    public float MusicSourceVolume;
    public float AmbientSourceVolume;

    public List<string> LogBlocks;

    public SpecialEvent specialEvent;
    public int specialEventState;
}


public class UserData : MonoBehaviour
{
    public static UserData instance = null;

    // Indexes
    public int CurrentCommandIndex { get; set; }

    public string CurrentBlock { get; set; }

    // Bg
    public string CurrentBG { get; set; }

    // Music
    public string CurrentMusic { get; set; }

    public string CurrentAmbient { get; set; }

    public float MusicSourceVolume { get; set; }

    public float AmbientSourceVolume { get; set; }

    // Events
    // public SpecialEvent SpecialEvent { get; set; }

    private const string _startingDayName = "Dream";

    private static string SaveFileName;
    private static string SaveFilesFolder;
    private static string SaveFilesData;

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

        CurrentBG = null;

        CurrentBlock = null;

        SaveFileName = SaveSystemUtils.SaveFileName;
        SaveFilesFolder = SaveSystemUtils.SaveFilesFolder;
        SaveFilesData = SaveSystemUtils.SaveFilesData;
    }

    private void Start()
    {
        // Сейв, заданный в главном меню
        if (StaticVariables.StartingLoadSaveFile == -1)
        {
            LoadGameFromStart();
        }
        else
        {
            StartCoroutine(ILoadGame(StaticVariables.StartingLoadSaveFile));
        }
    }

    public void SavePlayer(int saveNum)
    {
        SaveData newSave = new SaveData(saveNum);

        // Фоны
        newSave.Background = instance.CurrentBG;
        newSave.CurrentBlock = instance.CurrentBlock;

        // Команды
        newSave.CurrentCommandIndex = instance.CurrentCommandIndex;

        //Спрайты
        newSave.SpriteData = SpriteController.instance.GameSpriteData;

        // Музыка
        newSave.CurrentMusic = instance.CurrentMusic;
        newSave.CurrentAmbient = instance.CurrentAmbient;
        newSave.MusicSourceVolume = instance.MusicSourceVolume;
        newSave.AmbientSourceVolume = instance.AmbientSourceVolume;

        // Ивенты
        newSave.specialEvent = SpecialEventManager.instance.currentEventEnum;
        newSave.specialEventState = newSave.specialEvent == SpecialEvent.none ? 0 : SpecialEventManager.instance.currentEvent.GetState();

        // Выборы
        ChoiceManager.instance.SaveChoices(saveNum);

        ES3.Save<SaveData>("SaveFile" + saveNum, newSave, $"{SaveFilesFolder}/{SaveFileName}{saveNum}.es3");

        //newSave.LogBlocks = LogBlocks;
    }

    private void LoadGameFromStart()
    {
        Flowchart flowchart = PanelsManager.instance.flowchart;
        Block targetBlock = flowchart.FindBlock(_startingDayName);
        flowchart.ExecuteBlock(targetBlock);
    }

    public IEnumerator ILoadGame(int saveNum)
    {
        yield return new WaitForSeconds(0.5f);

        string fileName = $"{SaveFilesFolder}/{SaveFileName}{saveNum}.es3";
        SaveData newSave = ES3.Load<SaveData>("SaveFile" + saveNum, fileName);

        Flowchart flowchart = PanelsManager.instance.flowchart;

        // Block
        if (CurrentBlock != null)
        {
            Block activeBlock = flowchart.FindBlock(CurrentBlock);
            if (activeBlock != null)
            {
                activeBlock.Stop();
            }
        }

        CurrentBlock = newSave.CurrentBlock;

        // Index

        CurrentCommandIndex = newSave.CurrentCommandIndex;

        // Background
        yield return StartCoroutine(BackgroundManager.instance.IReleaseBackground());

        CurrentBG = newSave.Background;
        if (CurrentBG != null)
        {
            StartCoroutine(BackgroundManager.instance.ISwapBackground(CurrentBG));
        }


        // Special Events
        // Отгрузка
        yield return StartCoroutine(SpecialEventManager.instance.IReleaseCurrentEvent());
        // Загрузка
        SpecialEventManager.instance.currentEventEnum = newSave.specialEvent;
        yield return StartCoroutine(SpecialEventManager.instance.ILoadCurrentEventByState(newSave.specialEventState));


        // Спрайты
        // Отгрузка
        PackageConntector.instance.DisconnectAllPackages();
        SpriteController.instance.UnloadSprites();
        // Загрузка
        SpriteController.instance.GameSpriteData = newSave.SpriteData;
        SpriteController.instance.AutoConnectPackages();
        SpriteController.instance.LoadSprites();

        //GameObject.Find("ChatLog").GetComponent<LogManager>().DelLog();
        //LogBlocks = newSave.LogBlocks;

        // Музыка
        // Отгрузка
        yield return StartCoroutine(AudioManager.instance.FadeOutCurrent());

        // Загрузка
        CurrentMusic = newSave.CurrentMusic;
        CurrentAmbient = newSave.CurrentAmbient;
        MusicSourceVolume = newSave.MusicSourceVolume;
        AmbientSourceVolume = newSave.AmbientSourceVolume;
        if (CurrentMusic != null)
        {
            AudioManager.instance.MusicStart(CurrentMusic, 3f, MusicSourceVolume);
        }
        if (CurrentAmbient != null)
        {
            AudioManager.instance.AmbientStart(CurrentAmbient, 3f, AmbientSourceVolume);
        }
        DialogMod.denyNextDialog = false;

        // Выборы игрока
        StartCoroutine(ChoiceManager.instance.HideOptionsBox(20f));
        ChoiceManager.instance.ReleaseChoiceBox();
        ChoiceManager.instance.LoadSavedChoices(saveNum);

        flowchart.ExecuteBlock(flowchart.FindBlock(CurrentBlock), CurrentCommandIndex, null);
        CurrentCommandIndex--;

        //yield return null;
    }
}
