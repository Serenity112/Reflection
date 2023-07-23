using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstSaveAnimator : MonoBehaviour
{
    [SerializeField] private GameObject CassetteImg;
    [SerializeField] private Button ButtonUnsaved;
    [SerializeField] private Animator cassetteAnimator;

    private int saveNum;
    private GameObject screenshot;
    private GameObject SavedPanel;
    private GameObject UnSavedPanel;

    private IEnumerator CassetteFadeIn;
    private IEnumerator CassetteFadeOut;

    private SaveFileFields saveFileFields;

    private void Start()
    {
        saveFileFields = GetComponent<SaveFileFields>();
        saveNum = saveFileFields.saveNum;
        screenshot = saveFileFields.screenshot;
        SavedPanel = saveFileFields.SavedPanel;
        UnSavedPanel = saveFileFields.UnSavedPanel;

        ButtonUnsaved.interactable = true;
    }

    public void AppearCassette()
    {
        if (ButtonUnsaved.interactable && !StaticVariables.OverlayPanelActive)
        {
            if (CassetteFadeOut != null)
                StopCoroutine(CassetteFadeOut);

            CassetteFadeIn = FadeManager.FadeObject(CassetteImg, true, SaveManager.instance.speed);
            StartCoroutine(CassetteFadeIn);
        }
    }

    public void DisappearCassette()
    {
        if (ButtonUnsaved.interactable && !StaticVariables.OverlayPanelActive)
        {
            if (CassetteFadeIn != null)
                StopCoroutine(CassetteFadeIn);

            CassetteFadeOut = FadeManager.FadeObject(CassetteImg, false, SaveManager.instance.speed);
            StartCoroutine(CassetteFadeOut);
        }
    }

    private void InstantHideCassette()
    {
        FadeManager.FadeObject(CassetteImg, false);
    }

    public void FirstSaveIconClick()
    {
        if (!StaticVariables.UIsystemDown)
        {
            StartCoroutine(IFirstSaveIconClick());
        }
    }

    IEnumerator IFirstSaveIconClick()
    {
        Resources.UnloadUnusedAssets();

        StaticVariables.UIsystemDown = true;
        ButtonUnsaved.interactable = false;

        cassetteAnimator.Play("CasseteAnim");
        Vector3 currScale = CassetteImg.transform.localScale;
        yield return StartCoroutine(ExpandManager.ExpandObject(CassetteImg, 0.9f, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(CassetteImg, currScale, 0.05f));

        // Дата и время
        FadeManager.FadeObject(saveFileFields.datetime, false);
        string datetime = DateTime.Now.ToString("HH:mm dd/MM/yy");
        SaveManager.instance.SaveDateTime(saveNum, datetime);
        saveFileFields.datetime.GetComponent<Text>().text = datetime;
        StartCoroutine(FadeManager.FadeObject(saveFileFields.datetime, true, SaveManager.instance.speed));

        // Сам сейв
        UserData.instance.SavePlayer(saveNum);

        // Скриншот
        StartCoroutine(SaveManager.instance.SetScreenshot(saveNum, screenshot));

        DisappearCassette(); 
        
        yield return StartCoroutine(FadeManager.FadeObject(UnSavedPanel, false, SaveManager.instance.speed));

        if (!saveFileFields.OnMouseEnter)
        {
            StartCoroutine(saveFileFields.OpenOverPanel());
        }

        List<IEnumerator> enumerators_next = new List<IEnumerator>()
        {
            FadeManager.FadeToTargetAlpha(saveFileFields.NoImage, SaveManager.instance.frameAplhaOff, SaveManager.instance.speed),
            FadeManager.FadeObject(screenshot, true, SaveManager.instance.speed),
            FadeManager.FadeObject(SavedPanel, true, SaveManager.instance.speed),
        };    

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(enumerators_next));

        InstantHideCassette();

        saveFileFields.resetCassettePosition(CassetteImg);

        ButtonUnsaved.interactable = true;
        StaticVariables.UIsystemDown = false;
    }
}
