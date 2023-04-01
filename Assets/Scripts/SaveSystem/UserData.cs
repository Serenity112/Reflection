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

        specailEvent = SpecialEvents.none;
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

    public SpecialEvents specailEvent;
}

public enum SpecialEvents
{
    none,
    DreamSnow,
    TanyaCG,
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
    public SpecialEvents specialEvent { get; set; }

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

        CurrentBG = null;

        specialEvent = SpecialEvents.none;

        CurrentBlock = null;

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

    public void SavePlayer(int SaveNum)
    {
        SaveData newSave = new SaveData(SaveNum);

        newSave.Background = instance.CurrentBG;
        newSave.CurrentBlock = instance.CurrentBlock;

        newSave.CurrentCommandIndex = instance.CurrentCommandIndex;

        newSave.SpriteData = SpriteController.instance.GameSpriteData;

        newSave.CurrentMusic = instance.CurrentMusic;
        newSave.CurrentAmbient = instance.CurrentAmbient;
        newSave.MusicSourceVolume = instance.MusicSourceVolume;
        newSave.AmbientSourceVolume = instance.AmbientSourceVolume;

        newSave.specailEvent = instance.specialEvent;

        switch (specialEvent)
        {
            case SpecialEvents.none:
                break;
            case SpecialEvents.DreamSnow:
                ES3.Save<DreamSnowState>("DreamSnowSave" + SaveNum, DreamSnow.instance.currentState, "SpecialEvents.es3");
                break;
        }


        ES3.Save<SaveData>("SaveFile" + SaveNum, newSave, "SaveFiles.es3");

        //newSave.CurrentOst = audiomanager.currentOst;
        //
        //newSave.LogBlocks = LogBlocks;

    }

    private void LoadGameFromStart()
    {
        Flowchart flowchart = PanelsManager.instance.flowchart;
        Block targetBlock = flowchart.FindBlock("Dream");
        flowchart.ExecuteBlock(targetBlock);
    }

    public IEnumerator ILoadGame(int SaveNum)
    {
        yield return new WaitForSeconds(2f);


        SaveData newSave = ES3.Load<SaveData>("SaveFile" + SaveNum, "SaveFiles.es3");

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
        switch (specialEvent)
        {
            case SpecialEvents.none:
                break;
            case SpecialEvents.DreamSnow:
                yield return StartCoroutine(DreamSnow.instance.IReleaseEvent());
                break;
        }

        // Загрузка
        specialEvent = newSave.specailEvent;
        switch (specialEvent)
        {
            case SpecialEvents.none:
                break;
            case SpecialEvents.DreamSnow:
                DreamSnowState dreamSnowState = ES3.Load<DreamSnowState>("DreamSnowSave" + SaveNum, "SpecialEvents.es3");
                yield return StartCoroutine(DreamSnow.instance.ILoadEventByState(dreamSnowState));
                break;
        }

        //Sprites
        //Unload
        PackageConntector.instance.DisconnectAllPackages();
        SpriteController.instance.UnloadSprites();
        //Load
        SpriteController.instance.GameSpriteData = newSave.SpriteData;
        SpriteController.instance.AutoConnectPackages();
        SpriteController.instance.LoadSprites();

        //GameObject.Find("ChatLog").GetComponent<LogManager>().DelLog();
        //LogBlocks = newSave.LogBlocks;

        // Music
        // Отгрузка
        yield return AudioManager.instance.UnloadCurrent();
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

        flowchart.ExecuteBlock(flowchart.FindBlock(CurrentBlock), CurrentCommandIndex, null);
        CurrentCommandIndex--;
    }
}
