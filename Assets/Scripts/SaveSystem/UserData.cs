using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using System;
using System.Threading;

public struct SaveData
{
    public SaveData(int saveNum)
    {
        this.saveNum = saveNum;
        SpriteData = new SpriteData[4];
        Background = null;

        CurrentCommandIndex = 0;

        CurrentBlock = null;

        CurrentMusic = (null, 1);
        CurrentAmbient1 = (null, 1);
        CurrentAmbient2 = (null, 1);

        LogBlocks = new List<string>();

        specialEvent = SpecialEvent.none;
        specialEventData = null;
    }

    int saveNum;

    public SpriteData[] SpriteData;
    public string Background;
    public int CurrentCommandIndex;
    public string CurrentBlock;

    // Music
    public (string Name, float Volume) CurrentMusic;
    public (string Name, float Volume) CurrentAmbient1;
    public (string Name, float Volume) CurrentAmbient2;

    public List<string> LogBlocks;

    public SpecialEvent specialEvent;
    public string specialEventData;
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
    public (string Name, float Volume) CurrentMusic { get; set; }

    public (string Name, float Volume) CurrentAmbient1 { get; set; }

    public (string Name, float Volume) CurrentAmbient2 { get; set; }

    // Events

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
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;
        SaveData newSave = new SaveData(actualSaveNum);

        // Фоны
        newSave.Background = instance.CurrentBG;
        newSave.CurrentBlock = instance.CurrentBlock;

        // Команды
        newSave.CurrentCommandIndex = instance.CurrentCommandIndex;

        //Спрайты
        newSave.SpriteData = SpriteController.instance.GameSpriteData;

        // Музыка
        newSave.CurrentMusic = instance.CurrentMusic;
        newSave.CurrentAmbient1 = instance.CurrentAmbient1;
        newSave.CurrentAmbient2 = instance.CurrentAmbient2;

        // Ивенты
        newSave.specialEvent = SpecialEventManager.instance.currentEventEnum;
        newSave.specialEventData = newSave.specialEvent == SpecialEvent.none ? null : SpecialEventManager.instance.currentEvent.GetData();

        // Выборы
        ChoiceManager.instance.SaveChoices(actualSaveNum);

        new Thread(() =>
        {
            ES3.Save<SaveData>("SaveFile" + actualSaveNum, newSave, $"{SaveFilesFolder}/{SaveFileName}{actualSaveNum}.es3");
        }).Start();
      
        //newSave.LogBlocks = LogBlocks;
    }

    private void LoadGameFromStart()
    {
        Flowchart flowchart = PanelsManager.instance.flowchart;
        Block targetBlock = flowchart.FindBlock(_startingDayName);
        flowchart.ExecuteBlock(targetBlock);
    }

    public IEnumerator ILoadGame(int actualSaveNum)
    {
        yield return new WaitForSeconds(0.5f);

        string fileName = $"{SaveFilesFolder}/{SaveFileName}{actualSaveNum}.es3";
        SaveData newSave = ES3.Load<SaveData>("SaveFile" + actualSaveNum, fileName);

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
        yield return StartCoroutine(SpecialEventManager.instance.IReleaseCurrentEvent());
        yield return StartCoroutine(BackgroundManager.instance.IReleaseBackground());

        CurrentBG = newSave.Background;
        if (CurrentBG != null)
        {
            yield return StartCoroutine(BackgroundManager.instance.ILoadBackground(CurrentBG));
        }

        // Special Events
        yield return StartCoroutine(SpecialEventManager.instance.ILoadCurrentEventByState(newSave.specialEvent, newSave.specialEventData));

        // Спрайты
        // Отгрузка
        PackageConntector.instance.DisconnectAllPackages();
        SpriteController.instance.UnloadSprites();
        // Загрузка
        SpriteController.instance.GameSpriteData = newSave.SpriteData;
        yield return StartCoroutine(SpriteController.instance.AutoConnectPackages());
        yield return StartCoroutine(SpriteController.instance.LoadSprites());

        //GameObject.Find("ChatLog").GetComponent<LogManager>().DelLog();
        //LogBlocks = newSave.LogBlocks;

        // Музыка
        // Отгрузка
        yield return StartCoroutine(AudioManager.instance.FadeOutCurrent());

        // Загрузка
        CurrentMusic = newSave.CurrentMusic;
        CurrentAmbient1 = newSave.CurrentAmbient1;
        CurrentAmbient2 = newSave.CurrentAmbient2;

        if (CurrentMusic.Name != null)
        {
            AudioManager.instance.MusicStart(CurrentMusic.Name, 3f, CurrentMusic.Volume);
        }
        if (CurrentAmbient1.Name != null)
        {
            AudioManager.instance.AmbientStart(CurrentAmbient1.Name, 3f, CurrentAmbient1.Volume, false);
        }
        if (CurrentAmbient2.Name != null)
        {
            AudioManager.instance.AmbientStart(CurrentAmbient2.Name, 3f, CurrentAmbient2.Volume, true);
        }

        DialogMod.denyNextDialog = false;

        // Лог
        LogManager.instance.DelLog();

        // Выборы игрока
        StartCoroutine(ChoiceManager.instance.HideOptionsBox(20f));
        ChoiceManager.instance.ReleaseChoiceBox();
        ChoiceManager.instance.LoadSavedChoices(actualSaveNum);

        flowchart.ExecuteBlock(flowchart.FindBlock(CurrentBlock), CurrentCommandIndex, null);
        CurrentCommandIndex--;

        //yield return null;
    }
}
