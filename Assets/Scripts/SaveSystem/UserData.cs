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

        specailEvent = SpecialEvent.none;
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

    public SpecialEvent specailEvent;
}

public enum SpecialEvent
{
    none,
    DreamSnow,
    StationScroll,
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
    public SpecialEvent specialEvent { get; set; }

    private const string _startingDayName = "Dream";

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

        specialEvent = SpecialEvent.none;

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

    public void SavePlayer(int saveNum)
    {
        SaveData newSave = new SaveData(saveNum);

        newSave.Background = instance.CurrentBG;
        newSave.CurrentBlock = instance.CurrentBlock;

        newSave.CurrentCommandIndex = instance.CurrentCommandIndex;

        newSave.SpriteData = SpriteController.instance.GameSpriteData;

        newSave.CurrentMusic = instance.CurrentMusic;
        newSave.CurrentAmbient = instance.CurrentAmbient;
        newSave.MusicSourceVolume = instance.MusicSourceVolume;
        newSave.AmbientSourceVolume = instance.AmbientSourceVolume;

        newSave.specailEvent = instance.specialEvent;
        SaveSpecialEvent(newSave.specailEvent, saveNum);


        ES3.Save<SaveData>("SaveFile" + saveNum, newSave, "SaveFiles.es3");

        //newSave.CurrentOst = audiomanager.currentOst;
        //
        //newSave.LogBlocks = LogBlocks;

    }

    private void LoadGameFromStart()
    {
        Flowchart flowchart = PanelsManager.instance.flowchart;
        Block targetBlock = flowchart.FindBlock(_startingDayName);
        flowchart.ExecuteBlock(targetBlock);
    }

    // При добавлении новых ивентов нужно обновить следующие 3 метода :)
    private void SaveSpecialEvent(SpecialEvent specialEvent, int saveNum)
    {
        switch (specialEvent)
        {
            case SpecialEvent.none:
                break;
            case SpecialEvent.DreamSnow:
                ES3.Save<DreamSnowState>("DreamSnowSave" + saveNum, DreamSnow.instance.currentState, "SpecialEvents.es3");
                break;
            case SpecialEvent.StationScroll:
                ES3.Save<StationScrollState>("StationScrollSave" + saveNum, StationScroll.instance.currentState, "SpecialEvents.es3");
                break;
        }
    }

    private IEnumerator UnloadSpecialEvent(SpecialEvent specialEvent)
    {
        switch (specialEvent)
        {
            case SpecialEvent.none:
                break;
            case SpecialEvent.DreamSnow:
                yield return StartCoroutine(DreamSnow.instance.IReleaseEvent());
                break;
            case SpecialEvent.StationScroll:
                yield return StartCoroutine(StationScroll.instance.IReleaseEvent());
                break;
        }
    }

    private IEnumerator LoadSpecialEvent(SpecialEvent specialEvent, int saveNum)
    {
        switch (specialEvent)
        {
            case SpecialEvent.none:
                break;
            case SpecialEvent.DreamSnow:
                DreamSnowState dreamSnowState = ES3.Load<DreamSnowState>("DreamSnowSave" + saveNum, "SpecialEvents.es3");
                yield return StartCoroutine(DreamSnow.instance.ILoadEventByState(dreamSnowState));
                break;
            case SpecialEvent.StationScroll:
                StationScrollState stationScrollState = ES3.Load<StationScrollState>("StationScrollSave" + saveNum, "SpecialEvents.es3");
                yield return StartCoroutine(StationScroll.instance.ILoadEventByState(stationScrollState));
                break;
        }
    }

    public IEnumerator ILoadGame(int saveNum)
    {
        yield return new WaitForSeconds(0.5f);

        SaveData newSave = ES3.Load<SaveData>("SaveFile" + saveNum, "SaveFiles.es3");

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
        yield return StartCoroutine(UnloadSpecialEvent(specialEvent));
        // Загрузка
        specialEvent = newSave.specailEvent;
        yield return StartCoroutine(LoadSpecialEvent(specialEvent, saveNum));


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

        // Закрыть меню выбора, если оно вдруг есть
        StartCoroutine(ChoiceManager.instance.HideOptionsBox(20f));
        ChoiceManager.instance.ReleaseChoiceBox();

        flowchart.ExecuteBlock(flowchart.FindBlock(CurrentBlock), CurrentCommandIndex, null);
        CurrentCommandIndex--;

        //yield return null;
    }
}
