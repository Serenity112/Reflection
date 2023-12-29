using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using System;
using System.Threading;
using static AudioManager;

public struct SaveData
{
    public SaveData(int saveNum)
    {
        SpriteData = new SpriteData[4];
        Background = null;

        CurrentCommandIndex = 0;

        CurrentBlock = null;

        audioData = new();

        LogBlocks = new List<string>();

        specialEvent = SpecialEvent.none;
        specialEventData = null;
    }

    public SpriteData[] SpriteData;
    public string Background;
    public int CurrentCommandIndex;
    public string CurrentBlock;

    // Music
    public AudioDataSaveFile audioData;

    public List<string> LogBlocks;

    public SpecialEvent specialEvent;
    public string specialEventData;
}


public class UserData : MonoBehaviour
{
    public static UserData instance = null;

    // Indexes
    public int CurrentCommandIndex { get; set; }

    public string CurrentBlock { get; set; } = null;

    // Bg
    public string CurrentBG { get; set; } = null;

    // Events

    private static string SaveFileName;
    private static string SaveFilesFolder;
    private static string SaveFilesData;

    private float speed = 5f;

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

        SaveFileName = SaveSystemUtils.SaveFileName;
        SaveFilesFolder = SaveSystemUtils.SaveFilesFolder;
        SaveFilesData = SaveSystemUtils.SaveFilesData;
    }

    private void Start()
    {
        // Сейв, заданный в главном меню
        if (StaticVariables.StartingLoadSaveFile == -1)
        {
            StartCoroutine(ILoadGameFromStart());
        }
        else
        {
            StartCoroutine(ILoadGameBySaveNum(StaticVariables.StartingLoadSaveFile));
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
        newSave.audioData = AudioManager.instance.GetSaveData();

        // Ивенты
        newSave.specialEvent = SpecialEventManager.instance.CurrentEventEnum;
        newSave.specialEventData = newSave.specialEvent == SpecialEvent.none ? null : SpecialEventManager.instance.CurrentEventObject.GetData();

        // Внешние сейвы
        // Прочитанные диалоги
        Typewriter.Instance.SaveDialogSaves();

        // Выборы
        //ChoiceManager.instance.SaveChoices(actualSaveNum);


        new Thread(() =>
        {
            ES3.Save<SaveData>("SaveFile" + actualSaveNum, newSave, $"{SaveFilesFolder}/{SaveFileName}{actualSaveNum}.es3");
        }).Start();

        //newSave.LogBlocks = LogBlocks;
    }

    private IEnumerator ILoadGameFromStart()
    {
        string _startingDayName = "Dream";
        Flowchart flowchart = PanelsManager.instance.flowchart;
        Block targetBlock = flowchart.FindBlock(_startingDayName);
        flowchart.ExecuteBlock(targetBlock);
        yield return null;
    }

    private IEnumerator ILoadGameBySaveNum(int actualSaveNum)
    {
        FadeManager.FadeObject(PanelsManager.instance.BlackPanel, true);

        yield return StartCoroutine(ILoadGame(actualSaveNum));

        StartCoroutine(PanelsManager.instance.EnableGuiOnStart(true));

        yield return StartCoroutine(FadeManager.FadeObject(PanelsManager.instance.BlackPanel, false, 3f));
    }

    public IEnumerator ILoadGame(int actualSaveNum)
    {
        string fileName = $"{SaveFilesFolder}/{SaveFileName}{actualSaveNum}.es3";
        SaveData newSave = ES3.Load<SaveData>($"SaveFile{actualSaveNum}", fileName);

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

        // Backgrounds + special event
        CurrentBG = newSave.Background;

        IEnumerator i_bg_unload = CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
        {
            SpecialEventManager.instance.IReleaseCurrentEvent(),
            BackgroundManager.instance.IReleaseBackground(),
            TextBoxController.instance.ClearThemes(),
        });

        IEnumerator i_bg_load = CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
        {
            BackgroundManager.instance.ILoadBackground(CurrentBG),
            SpecialEventManager.instance.ILoadCurrentEventByState(newSave.specialEvent, newSave.specialEventData)
        });

        IEnumerator i_bg = CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
        {
            i_bg_unload,
            i_bg_load,
        });

        // Sprites

        IEnumerator i_sprite_unload = CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            PackageConntector.instance.IDisconnectAllPackages(),
            SpriteController.instance.IUnloadSprites(),
        });

        IEnumerator i_sprite_load = CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            SpriteController.instance.LoadSprites(newSave.SpriteData),
        });

        IEnumerator i_sprite = CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
        {
            i_sprite_unload,
            i_sprite_load,
        });

        // Музыка
        IEnumerator i_music = CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
        {
            AudioManager.instance.FadeOutCurrent(),
            AudioManager.instance.FadeInCurrent(newSave.audioData),
        });

        // Лог
        LogManager.instance.DelLog();

        // Прочитанные диалоги
        Typewriter.Instance.LoadDialogSaves();

        // Выборы игрока
        //StartCoroutine(ChoiceManager.instance.HideOptionsBox(20f));
        //ChoiceManager.instance.ReleaseChoiceBox();
        //ChoiceManager.instance.LoadSavedChoices(actualSaveNum);

        // Весь процесс загрузки
        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            i_bg,
            i_sprite,
            i_music
        }));


        flowchart.ExecuteBlock(flowchart.FindBlock(CurrentBlock), CurrentCommandIndex);
        CurrentCommandIndex--;
    }
}
