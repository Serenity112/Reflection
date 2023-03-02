using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

public struct SaveData
{
    public SaveData(int saveNum)
    {
        this.saveNum = saveNum;
        SpriteData = new SpriteData[4];
        Background = -1;

        CurrentCommandIndex = 0;
        CurrentDialogIndex = 0;

        CurrentBlock = null;
        CurrentOst = 0;
        LogBlocks = new List<string>();

        specailEvent = SpecialEvents.none;
    }

    int saveNum;

    public SpriteData[] SpriteData;
    public int Background;
    public int CurrentCommandIndex;
    public int CurrentDialogIndex;
    public string CurrentBlock;
    public int CurrentOst;
    public List<string> LogBlocks;

    public SpecialEvents specailEvent;
}

public enum SpecialEvents {
    none,
    DreamSnow,
    TanyaCG,

}

public class UserData : MonoBehaviour
{
    public static UserData instance = null;

    public int CurrentCommandIndex { get; set; }

    public int CurrentDialogIndex { get; set; }

    public string CurrentBlock { get; set; }

    public int CurrentBG { get; set; }

    public int indexToLoad { get; set; }

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

        CurrentBG = -1;

        indexToLoad = -1;

        specialEvent = SpecialEvents.none;
    }

    public void SavePlayer(int SaveNum)
    {
        SaveData newSave = new SaveData(SaveNum);

        newSave.Background = instance.CurrentBG;
        newSave.CurrentBlock = instance.CurrentBlock;

        newSave.CurrentCommandIndex = instance.CurrentCommandIndex;
        newSave.CurrentDialogIndex = instance.CurrentDialogIndex;

        newSave.SpriteData = SpriteController.instance.GameSpriteData;

        newSave.specailEvent = instance.specialEvent;

        switch(specialEvent)
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
    public void LoadPlayer(int SaveNum)
    {
        StartCoroutine(ILoadPlayer(SaveNum));
    }
    private IEnumerator ILoadPlayer(int SaveNum)
    {
        SaveData newSave = ES3.Load<SaveData>("SaveFile" + SaveNum, "SaveFiles.es3");

        Flowchart flowchart = PanelsManager.instance.flowchart;

        // Block
        Block targetBlock = flowchart.FindBlock(CurrentBlock);
        targetBlock.Stop();
        CurrentBlock = newSave.CurrentBlock;

        // Index
        CurrentCommandIndex = newSave.CurrentCommandIndex;
        CurrentCommandIndex--;

        CurrentDialogIndex = newSave.CurrentDialogIndex;
        indexToLoad = CurrentDialogIndex;

        // Background
        yield return StartCoroutine(BackgroundManager.instance.IReleaseBackground());

        CurrentBG = newSave.Background;
        if (CurrentBG != -1)
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

        //StartCoroutine(LoadAudio(newSave.CurrentOst));
        //GameObject.Find("ChatLog").GetComponent<LogManager>().DelLog();
        //UnityEngine.Debug.Log("Вызов Load");
        //LogBlocks = newSave.LogBlocks;
        //GameObject.Find("ChatLog").GetComponent<LoadLog>().Load(CurrentIndex);


        DialogMod.denyNextDialog = false;

        flowchart.ExecuteBlock(targetBlock, CurrentCommandIndex, null);
    }
    
}