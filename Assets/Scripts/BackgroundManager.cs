using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Fungus;

public enum bgSwapType
{
    BlackFade,
    Instant,
    Overlay,
}

public struct BgData
{
    public BgData(int num, string name)
    {
        this.num = num;
        this.name = name;
    }

    public int num;
    public string name;
};
public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager instance = null;

    [SerializeField] 
    private GameObject BlackPanel;

    [SerializeField] 
    private GameObject backgroundsPanel;

    [SerializeField] 
    private GameObject storytext;

    private AsyncOperationHandle<GameObject> bg_handler;

    private List<BgData> BackGrounds;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        BackGrounds = new List<BgData>
        {
            new BgData(0, "AssemblyHall"),
            new BgData(1, "Corridor"),
            new BgData(2, "SkverDay"),
            new BgData(3, "Robotech"),
            new BgData(4, "NastyaCG"),
            new BgData(5, "EvelinaCG"),
            new BgData(6, "Statue"),
            new BgData(7, "Museum"),
            new BgData(8, "Dream"),
            new BgData(9, "DreamLaunch"),
            new BgData(10, "Corridor"),
            new BgData(11, "Classroom"),
            new BgData(12, "StationFocused"),
            new BgData(13, "AssemblyHallCG"),
            new BgData(14, "HallwayMuseum"),
            new BgData(15, "Plan"),
            new BgData(16, "RoomNight1"),
            new BgData(17, "RoomNight2"),
            new BgData(18, "Lectornaya"),
            new BgData(19, "Hall"),
            new BgData(20, "Street"),
            new BgData(21, "Monorels")
        };
    }

    public IEnumerator IReleaseBackground()
    {
        if (bg_handler.IsValid())
        {
            yield return Addressables.ReleaseInstance(bg_handler);
        }
    }

    public IEnumerator ISwap(bgSwapType type, int num, float speed)
    {
        switch (type)
        {
            case bgSwapType.BlackFade:
                yield return IBlackFadeBackground(num, speed);
                break;
            case bgSwapType.Instant:
                yield return ISwapBackground(num);
                break;
            case bgSwapType.Overlay:
                yield return IOverlayBackground(num, speed);
                break;
        }
    }

    // Фон -> чёрный экран -> фон
    public IEnumerator IBlackFadeBackground(int Bg_num, float speed)
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, speed));
        storytext.GetComponent<Text>().text = "";

        // Удаление старых фонов    
        if (bg_handler.IsValid())
        {
            Addressables.ReleaseInstance(bg_handler);
        }

        string bg_adress = BackGrounds[Bg_num].name;

        bg_handler = Addressables.InstantiateAsync(bg_adress, backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        if (bg_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error loading second bg with adress " + bg_adress);
        }

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));
    }

    // Фон -> резкая смена -> фон
    public IEnumerator ISwapBackground(int Bg_num)
    {
        string bg_adress = BackGrounds[Bg_num].name;

        AsyncOperationHandle<GameObject> old_handler = bg_handler;

        bg_handler = Addressables.InstantiateAsync(bg_adress, backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        if (bg_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error loading second bg with adress " + bg_adress);
        }

        if (old_handler.IsValid())
        {
            Addressables.ReleaseInstance(old_handler);
        }

        Resources.UnloadUnusedAssets();
    }

    // Фон поверх старого фона
    public IEnumerator IOverlayBackground(int Bg_num, float speed)
    {
        string bg_adress = BackGrounds[Bg_num].name;

        AsyncOperationHandle<GameObject> old_handler = bg_handler;

        bg_handler = Addressables.InstantiateAsync(bg_adress, backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        if (bg_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error loading second bg with adress " + bg_adress);
        }

        GameObject newBg = bg_handler.Result;

        newBg.GetComponent<CanvasGroup>().alpha = 0f;

        newBg.transform.SetSiblingIndex(0);

        newBg.transform.GetComponent<Canvas>().sortingOrder = 2;
        yield return StartCoroutine(FadeManager.FadeObject(newBg, true, speed));
        newBg.transform.GetComponent<Canvas>().sortingOrder = 1;

        if (old_handler.IsValid())
        {
            Addressables.ReleaseInstance(old_handler);
        }

        Resources.UnloadUnusedAssets();
    }
}