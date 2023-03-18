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
    }

    public IEnumerator IReleaseBackground()
    {
        if (bg_handler.IsValid())
        {
            yield return Addressables.ReleaseInstance(bg_handler);
        }
    }

    public IEnumerator ISwap(string bg_adress, bgSwapType type, float speed)
    {
        switch (type)
        {
            case bgSwapType.BlackFade:
                yield return IBlackFadeBackground(bg_adress, speed);
                break;
            case bgSwapType.Instant:
                yield return ISwapBackground(bg_adress);
                break;
            case bgSwapType.Overlay:
                yield return IOverlayBackground(bg_adress, speed);
                break;
        }
    }

    // Фон -> чёрный экран -> фон
    public IEnumerator IBlackFadeBackground(string bg_adress, float speed)
    {
        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, true, speed));
        storytext.GetComponent<Text>().text = "";

        // Удаление старых фонов    
        if (bg_handler.IsValid())
        {
            Addressables.ReleaseInstance(bg_handler);
        }

        bg_handler = Addressables.InstantiateAsync(bg_adress, backgroundsPanel.gameObject.GetComponent<RectTransform>(), false, true);
        yield return bg_handler;

        if (bg_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error loading second bg with adress " + bg_adress);
        }

        yield return StartCoroutine(FadeManager.FadeObject(BlackPanel, false, speed));
    }

    // Фон -> резкая смена -> фон
    public IEnumerator ISwapBackground(string bg_adress)
    {
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
    public IEnumerator IOverlayBackground(string bg_adress, float speed)
    {
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