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
        SpriteData = new();
        Background = null;

        CurrentCommandIndex = 0;

        CurrentBlock = null;

        AudioData = new();

        specialEvent = SpecialEvent.none;
        specialEventData = null;

        SavedChoices = new();
    }

    public Dictionary<Character, SpriteData> SpriteData;
    public string Background;
    public int CurrentCommandIndex;
    public string CurrentBlock;

    // Music
    public AudioDataSaveFile AudioData;

    public SpecialEvent specialEvent;
    public string specialEventData;

    public Dictionary<string, int> SavedChoices;
}


public class UserData : MonoBehaviour
{
    public static UserData instance = null;

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

        SaveFileName = SaveSystemUtils.SaveFileName;
        SaveFilesFolder = SaveSystemUtils.SaveFilesFolder;
        SaveFilesData = SaveSystemUtils.SaveFilesData;
    }

    private void Start()
    {
        // Сейв, заданный в главном меню
        if (SaveManagerStatic.StartingLoadSaveFile == -1)
        {
            StartCoroutine(ILoadGameFromStart());
        }
        else
        {
            StartCoroutine(ILoadGameBySaveNum(SaveManagerStatic.StartingLoadSaveFile));
        }
    }

    public void SavePlayer(int actualSaveNum)
    {
        SaveData newSave = new SaveData(actualSaveNum);
        Flowchart flowchart = PanelsManager.instance.flowchart;

        // Фоны
        newSave.Background = BackgroundManager.instance.CurrentBG;
        newSave.CurrentBlock = flowchart.ActiveBlock.BlockName;

        // Команды        
        newSave.CurrentCommandIndex = PanelsManager.instance.flowchart.ActiveBlock.GetCurrentIndex();

        //Спрайты
        newSave.SpriteData = SpriteController.instance.CharacterSpriteData;

        // Музыка
        newSave.AudioData = AudioManager.instance.GetSaveData();

        // Ивенты
        newSave.specialEvent = SpecialEventManager.instance.CurrentEventEnum;
        newSave.specialEventData = newSave.specialEvent == SpecialEvent.none ? null : SpecialEventManager.instance.CurrentEventObject.GetData();

        // Прочитанные диалоги
        Typewriter.Instance.SaveDialogReadSaves();

        // Выборы
        newSave.SavedChoices = ChoiceManager.instance.GetSavedChoices();

        new Thread(() =>
        {
            ES3.Save<SaveData>("SaveFile" + actualSaveNum, newSave, $"{SaveFilesFolder}/{SaveFileName}{actualSaveNum}.es3");
        }).Start();
    }

    private IEnumerator ILoadGameFromStart()
    {
        string _startingDayName = "d1_1";
        Flowchart flowchart = PanelsManager.instance.flowchart;
        Block targetBlock = flowchart.FindBlock(_startingDayName);
        flowchart.ExecuteBlock(targetBlock, 0);
        yield return null;
    }

    private IEnumerator ILoadGameBySaveNum(int actualSaveNum)
    {
        FadeManager.FadeObject(PanelsManager.instance.BlackPanel, true);

        yield return StartCoroutine(ILoadGame(actualSaveNum));
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(PanelsManager.instance.EnableGuiOnStart(true));

        yield return StartCoroutine(FadeManager.FadeObject(PanelsManager.instance.BlackPanel, false, 3f));
    }

    // Фоновая загрузка игры
    public IEnumerator ILoadGame(int actualSaveNum)
    {
        string fileName = $"{SaveFilesFolder}/{SaveFileName}{actualSaveNum}.es3";
        SaveData newSave = ES3.Load<SaveData>($"SaveFile{actualSaveNum}", fileName);
        Flowchart flowchart = PanelsManager.instance.flowchart;


        // Block + index
        string currBlockName = flowchart.ActiveBlock?.BlockName;
        if (currBlockName != null)
        {
            Block activeBlock = flowchart.FindBlock(currBlockName);
            if (activeBlock != null)
            {
                activeBlock.Stop();
            }
        }

        currBlockName = newSave.CurrentBlock;
        int commandIndex = newSave.CurrentCommandIndex;
        Block targetBlock = flowchart.FindBlock(currBlockName);

        // Backgrounds + special event
        BackgroundManager.instance.CurrentBG = newSave.Background;

        IEnumerator i_bg_unload = CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
        {
            SpecialEventManager.instance.IReleaseCurrentEvent(),
            BackgroundManager.instance.IReleaseBackground(),
            TextBoxController.instance.ClearThemes(),
        });

        IEnumerator i_bg_load = CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
        {
            BackgroundManager.instance.ILoadBackground(BackgroundManager.instance.CurrentBG),
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
            SpriteController.instance.IUploadSprites(newSave.SpriteData),
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
            AudioManager.instance.FadeInCurrent(newSave.AudioData),
        });


        // Лог
        LogManager.instance.ClearLog();
        LogManager.instance.RestoreLogByBlock(targetBlock, commandIndex);


        // Прочитанные диалоги
        Typewriter.Instance.LoadDialogReadSaves();
        Typewriter.Instance.SetText("");


        // Выборы игрока
        ChoiceManager.instance.UploadChoices(newSave.SavedChoices);
        ChoiceManager.instance.HideChoiceBox();

        // Другие менеджеры
        SpriteExpand.instance.ResetManager();

        // Весь процесс загрузки
        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            i_bg,
            i_sprite,
            i_music
        }));


        flowchart.ExecuteBlock(targetBlock, commandIndex);
    }
}
