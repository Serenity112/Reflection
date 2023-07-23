using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public enum Side
{
    Left,
    Right,
}

public enum SaveOption
{
    Save,
    Load,
}
public class SaveChoiseAnimator : MonoBehaviour
{
    [SerializeField] private Animator SaveAnimator;
    [SerializeField] private Animator LoadAnimator;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button LoadButton;
    public GameObject DeleteCross;

    private int saveNum;

    private GameObject GradLeft;
    private GameObject GradRight;
    private GameObject IconLeft;
    private GameObject IconRight;

    private GameObject SavedPanel;
    private GameObject UnSavedPanel;

    private GameObject screenshot;
    private GameObject overscreenshot;

    private SaveFileFields saveFileFields;
    private SaveChoiseIconAnimator saveChoiseIconAnimator;

    private void Start()
    {
        saveFileFields = GetComponent<SaveFileFields>();

        saveNum = saveFileFields.saveNum;
        GradLeft = saveFileFields.GradLeft;
        GradRight = saveFileFields.GradRight;
        IconLeft = saveFileFields.IconLeft;
        IconRight = saveFileFields.IconRight;

        screenshot = saveFileFields.screenshot;
        overscreenshot = saveFileFields.overscreenshot;
        SavedPanel = saveFileFields.SavedPanel;
        UnSavedPanel = saveFileFields.UnSavedPanel;

        saveChoiseIconAnimator = GetComponent<SaveChoiseIconAnimator>();
    }

    public void SaveAction(SaveOption option)
    {
        if (!StaticVariables.UIsystemDown && !StaticVariables.OverlayPanelActive)
        {
            StartCoroutine(ISaveAction(option));
        }
    }
    public IEnumerator ISaveAction(SaveOption option)
    {
        switch (option)
        {
            case SaveOption.Save:
                SaveAnimator.SetTrigger("DoSave");

                StaticVariables.OverlayPanelActive = true;

                Vector3 saveScale = IconLeft.transform.localScale;
                yield return StartCoroutine(ExpandManager.ExpandObject(IconLeft, 0.9f, 0.05f));
                yield return StartCoroutine(ExpandManager.ExpandObject(IconLeft, saveScale, 0.05f));

                yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel("Перезаписать сохранение?", IRewriteSave(), ICancelSave()));

                break;
            case SaveOption.Load:
                LoadAnimator.SetTrigger("DoLoad");

                StaticVariables.OverlayPanelActive = true;

                Vector3 loadScale = IconRight.transform.localScale;
                yield return StartCoroutine(ExpandManager.ExpandObject(IconRight, 0.9f, 0.05f));
                yield return StartCoroutine(ExpandManager.ExpandObject(IconRight, loadScale, 0.05f));

                yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel("Загрузить сохранение?", LoadFile(), CancelLoad()));

                break;
        }
    }

    IEnumerator IRewriteSave()
    {
        StaticVariables.UIsystemDown = true;
        StaticVariables.OverlayPanelActive = false;

        string datetime = DateTime.Now.ToString("HH:mm dd/MM/yy");
        SaveManager.instance.SaveDateTime(saveNum, datetime);
        saveFileFields.datetime.GetComponent<Text>().text = datetime;

        UserData.instance.SavePlayer(saveNum);

        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross(2);
        StartCoroutine(saveFileFields.OpenOverPanel());

        saveChoiseIconAnimator.StopAllCoroutines();
        saveChoiseIconAnimator.HideLeft();

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel()
        }));

        yield return StartCoroutine(SaveManager.instance.OverrideScreenshot(saveNum, screenshot, overscreenshot, SaveManager.instance.speed));
        SaveAnimator.SetTrigger("StopSave");

        StaticVariables.UIsystemDown = false;
        saveFileFields.resetCassettePosition(IconLeft);
    }

    IEnumerator ICancelSave()
    {
        StaticVariables.UIsystemDown = true;
        StaticVariables.OverlayPanelActive = false;

        StartCoroutine(saveFileFields.OpenOverPanel());
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross(2);

        saveChoiseIconAnimator.StopAllCoroutines();
        saveChoiseIconAnimator.HideLeft();

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel()
        }));

        SaveAnimator.SetTrigger("StopSave");

        saveFileFields.resetCassettePosition(IconLeft);

        StaticVariables.UIsystemDown = false;
    }

    // Удаление сейва

    public void DeleteAction()
    {
        if (!StaticVariables.UIsystemDown && !StaticVariables.OverlayPanelActive)
        {
            StartCoroutine(IDeleteDialog());
        }
    }

    IEnumerator IDeleteDialog()
    {
        StaticVariables.OverlayPanelActive = true;
        yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel("Удалить сохранение?", IDeleteSave(), ICancelDelete()));
    }

    IEnumerator IDeleteSave()
    {
        StaticVariables.UIsystemDown = true;
        StaticVariables.OverlayPanelActive = false;

        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross(2);
        if (saveFileFields.exitLeft && saveFileFields.exitRight)
        {
            StartCoroutine(saveFileFields.CloseOverPanel());
        }

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeObject(SavedPanel, false, SaveManager.instance.speed * 2),
            FadeManager.FadeOnly(screenshot, false, SaveManager.instance.speed),
            FadeManager.FadeOnly(saveFileFields.NoImage, true, SaveManager.instance.speed),
            FadeManager.FadeObject(saveFileFields.datetime, false, SaveManager.instance.speed),
            ConfirmationPanel.instance.ClosePanel()
        }));

        FadeManager.FadeObject(UnSavedPanel, true);

        SaveManager.instance.RemoveDateTime(saveNum);
        SaveManager.instance.DeleteSave(saveNum);

        yield return new WaitForSeconds(0.1f);

        StaticVariables.UIsystemDown = false;
    }

    IEnumerator ICancelDelete()
    {
        StaticVariables.UIsystemDown = true;
        StaticVariables.OverlayPanelActive = false;

        StartCoroutine(saveFileFields.OpenOverPanel());
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross(2);

        yield return StartCoroutine(ConfirmationPanel.instance.ClosePanel());

        StaticVariables.UIsystemDown = false;
    }

    IEnumerator LoadFile()
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel(),
            PanelsManager.instance.ILoadGame(actualSaveNum)
        }));
    }

    IEnumerator CancelLoad()
    {
        StaticVariables.UIsystemDown = true;
        StaticVariables.OverlayPanelActive = false;

        StartCoroutine(saveFileFields.OpenOverPanel());
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross(2);

        saveChoiseIconAnimator.StopAllCoroutines();
        saveChoiseIconAnimator.HideRight();

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel()
        }));

        LoadAnimator.SetTrigger("StopLoad");

        saveFileFields.resetCassettePosition(IconRight);
        StaticVariables.UIsystemDown = false;
    }
}
