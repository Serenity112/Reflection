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

        AudioData = new();

        specialEvent = SpecialEvent.none;
        specialEventData = null;

        SavedChoices = new();
    }

    public SpriteData[] SpriteData;
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
        // ����, �������� � ������� ����
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

        // ����
        newSave.Background = instance.CurrentBG;
        newSave.CurrentBlock = instance.CurrentBlock;

        // �������        
        newSave.CurrentCommandIndex = PanelsManager.instance.flowchart.FindBlock(CurrentBlock).GetCurrentIndex();

        //�������
        newSave.SpriteData = SpriteController.instance.GameSpriteData;

        // ������
        newSave.AudioData = AudioManager.instance.GetSaveData();

        // ������
        newSave.specialEvent = SpecialEventManager.instance.CurrentEventEnum;
        newSave.specialEventData = newSave.specialEvent == SpecialEvent.none ? null : SpecialEventManager.instance.CurrentEventObject.GetData();

        // ����������� �������
        Typewriter.Instance.SaveDialogReadSaves();

        // ������
        newSave.SavedChoices = ChoiceManager.instance.GetSavedChoices();

        new Thread(() =>
        {
            ES3.Save<SaveData>("SaveFile" + actualSaveNum, newSave, $"{SaveFilesFolder}/{SaveFileName}{actualSaveNum}.es3");
        }).Start();
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
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(PanelsManager.instance.EnableGuiOnStart(true));

        yield return StartCoroutine(FadeManager.FadeObject(PanelsManager.instance.BlackPanel, false, 3f));
    }

    public IEnumerator ILoadGame(int actualSaveNum)
    {
        string fileName = $"{SaveFilesFolder}/{SaveFileName}{actualSaveNum}.es3";
        SaveData newSave = ES3.Load<SaveData>($"SaveFile{actualSaveNum}", fileName);
        Flowchart flowchart = PanelsManager.instance.flowchart;


        // Block + index
        if (CurrentBlock != null)
        {
            Block activeBlock = flowchart.FindBlock(CurrentBlock);
            if (activeBlock != null)
            {
                activeBlock.Stop();
            }
        }

        CurrentBlock = newSave.CurrentBlock;
        CurrentCommandIndex = newSave.CurrentCommandIndex;
        Block targetBlock = flowchart.FindBlock(CurrentBlock);

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


        // ������
        IEnumerator i_music = CoroutineWaitForAll.instance.WaitForSequence(new List<IEnumerator>()
        {
            AudioManager.instance.FadeOutCurrent(),
            AudioManager.instance.FadeInCurrent(newSave.AudioData),
        });


        // ���
        LogManager.instance.ClearLog();
        LogManager.instance.RestoreLogByBlock(targetBlock, CurrentCommandIndex);


        // ����������� �������
        Typewriter.Instance.LoadDialogReadSaves();
        Typewriter.Instance.SetText("");


        // ������ ������
        ChoiceManager.instance.UploadChoices(newSave.SavedChoices);
        ChoiceManager.instance.HideChoiceBox();


        // ���� ������� ��������
        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            i_bg,
            i_sprite,
            i_music
        }));


        flowchart.ExecuteBlock(targetBlock, CurrentCommandIndex);
        CurrentCommandIndex--;
    }
}
