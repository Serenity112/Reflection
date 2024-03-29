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

        SavedTime = DateTime.Now;

        AudioData = new();

        specialEvent = SpecialEvent.none;
        specialEventData = null;

        SavedChoices = new();
    }

    public Dictionary<Character, SpriteData> SpriteData;
    public string Background;
    public int CurrentCommandIndex;
    public string CurrentBlock;
    public DateTime SavedTime;

    public AudioDataSaveFile AudioData;

    public SpecialEvent specialEvent;
    public string specialEventData;

    public Dictionary<string, int> SavedChoices;
}


public class UserData : MonoBehaviour
{
    public static UserData instance = null;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        // ����, �������� � ������� ����
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

        // ����
        newSave.Background = BackgroundManager.instance.CurrentBG;
        newSave.CurrentBlock = flowchart.ActiveBlock.BlockName;

        // �������        
        newSave.CurrentCommandIndex = PanelsManager.instance.flowchart.ActiveBlock.GetCurrentIndex();

        //�������
        newSave.SpriteData = SpriteController.instance.CharacterSpriteData;

        // ����� �����
        newSave.SavedTime = DateTime.Now;

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
            string fileName = $"{SaveSystemUtils.SaveFileSingleName}{actualSaveNum}.es3";
            ES3.Save<SaveData>("Save", newSave, $"{SaveSystemUtils.SaveFilesFolder}/{fileName}");
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
        yield return new WaitForSeconds(0.2f);

        StartCoroutine(PanelsManager.instance.EnableGuiOnStart(true));

        yield return StartCoroutine(FadeManager.FadeObject(PanelsManager.instance.BlackPanel, false, 3f));
    }

    // ������� �������� ����
    public IEnumerator ILoadGame(int actualSaveNum)
    {
        string fileName = $"{SaveSystemUtils.SaveFileSingleName}{actualSaveNum}.es3";
        SaveData newSave = ES3.Load<SaveData>("Save", $"{SaveSystemUtils.SaveFilesFolder}/{fileName}");
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

        IEnumerator i_bg_unload = CoroutineUtils.WaitForSequence(new List<IEnumerator>()
        {
            SpecialEventManager.instance.IReleaseCurrentEvent(),
            BackgroundManager.instance.IReleaseBackground(),
            TextBoxController.instance.ClearThemes(),
        });

        IEnumerator i_bg_load = CoroutineUtils.WaitForSequence(new List<IEnumerator>()
        {
            BackgroundManager.instance.ILoadBackground(BackgroundManager.instance.CurrentBG),
            SpecialEventManager.instance.ILoadCurrentEventByState(newSave.specialEvent, newSave.specialEventData)
        });

        IEnumerator i_bg = CoroutineUtils.WaitForSequence(new List<IEnumerator>()
        {
            i_bg_unload,
            i_bg_load,
        });


        // Sprites
        IEnumerator i_sprite_unload = CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            PackageConntector.instance.IDisconnectAllPackages(),
            SpriteController.instance.IUnloadSprites(),
        });

        IEnumerator i_sprite_load = CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            SpriteController.instance.IUploadSprites(newSave.SpriteData),
        });

        IEnumerator i_sprite = CoroutineUtils.WaitForSequence(new List<IEnumerator>()
        {
            i_sprite_unload,
            i_sprite_load,
        });


        // ������
        IEnumerator i_music = CoroutineUtils.WaitForSequence(new List<IEnumerator>()
        {
            AudioManager.instance.FadeOutCurrent(),
            AudioManager.instance.FadeInCurrent(newSave.AudioData),
        });


        // ���
        LogManager.instance.ClearLog();
        LogManager.instance.RestoreLogByBlock(targetBlock, commandIndex);


        // ����������� �������
        Typewriter.Instance.LoadDialogReadSaves();
        Typewriter.Instance.SetText("");


        // ������ ������
        ChoiceManager.instance.UploadChoices(newSave.SavedChoices);
        ChoiceManager.instance.HideChoiceBox();

        // ������ ���������
        SpriteExpand.instance.ResetManager();

        // ���� ������� ��������
        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            i_bg,
            i_sprite,
            i_music
        }));


        flowchart.ExecuteBlock(targetBlock, commandIndex);
    }
}
