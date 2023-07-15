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
    }

    public void SaveAction(SaveOption option)
    {
        if (saveFileFields.AllowSaveLoad && !StaticVariables.UIsystemDown && !StaticVariables.OverlayPanelActive)
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

                yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel("Перезаписать сохранение?", RewriteSave(), ICancelSave()));

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

    IEnumerator RewriteSave()
    {
        string datetime = DateTime.Now.ToString("HH:mm dd/MM/yy");
        SaveManager.instance.SaveDateTime(saveNum, datetime);
        saveFileFields.datetime.GetComponent<Text>().text = datetime;

        UserData.instance.SavePlayer(saveNum);

        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeObject(IconLeft, false, SaveManager.instance.optionsGradientSpeed),
            FadeManager.FadeObject(GradRight, false, SaveManager.instance.optionsGradientSpeed),
            ConfirmationPanel.instance.ClosePanel()
        });

        SaveAnimator.SetTrigger("StopSave");

        yield return StartCoroutine(SaveManager.instance.OverrideScreenshot(saveNum, screenshot, overscreenshot, SaveManager.instance.optionsGradientSpeed));

        saveFileFields.resetCassettePosition(IconLeft);
        StartCoroutine(saveFileFields.OpenOverPanel());
    }

    IEnumerator ICancelSave()
    {
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeObject(IconLeft, false, SaveManager.instance.optionsGradientSpeed),
            FadeManager.FadeObject(GradRight, false, SaveManager.instance.optionsGradientSpeed),
            ConfirmationPanel.instance.ClosePanel()
        });

        SaveAnimator.SetTrigger("StopSave");

        saveFileFields.resetCassettePosition(IconLeft);
        StartCoroutine(saveFileFields.OpenOverPanel());
    }

    // Удаление сейва

    public void DeleteAction()
    {
        if (saveFileFields.AllowSaveLoad && !StaticVariables.UIsystemDown && !StaticVariables.OverlayPanelActive)
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
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
        FadeManager.FadeObject(SavedPanel, false);
        FadeManager.FadeObject(UnSavedPanel, true);
        StartCoroutine(saveFileFields.CloseOverPanel());
        SaveManager.instance.RemoveDateTime(saveNum);

        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeOnly(screenshot, false, SaveManager.instance.optionsGradientSpeed),
            FadeManager.FadeObject(saveFileFields.datetime, false, SaveManager.instance.optionsGradientSpeed),
            ConfirmationPanel.instance.ClosePanel()
        });

        SaveManager.instance.DeleteSave(saveNum);
    }

    IEnumerator ICancelDelete()
    {
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
        yield return StartCoroutine(ConfirmationPanel.instance.ClosePanel());
    }

    IEnumerator LoadFile()
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;

        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel(),
            PanelsManager.instance.ILoadGame(actualSaveNum)
        });
    }

    IEnumerator CancelLoad()
    {
        SaveButton.interactable = true;
        LoadButton.interactable = true;
        DeleteCross.GetComponent<Button>().interactable = true;

        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();

        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeObject(IconRight, false, SaveManager.instance.optionsGradientSpeed),
            FadeManager.FadeObject(GradLeft, false, SaveManager.instance.optionsGradientSpeed),
            ConfirmationPanel.instance.ClosePanel()
        });

        SaveAnimator.SetTrigger("StopLoad");

        saveFileFields.resetCassettePosition(IconRight);
        StartCoroutine(saveFileFields.OpenOverPanel());
    }
}
