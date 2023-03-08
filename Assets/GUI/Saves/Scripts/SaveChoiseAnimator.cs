using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
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
    [SerializeField] private GameObject DeleteCross;

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
        if (saveFileFields.AllowSaveLoad && !StaticVariables.UIsystemDown && !PanelsManager.confirmPanelActive)
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

                PanelsManager.confirmPanelActive = true;

                Vector3 saveScale = IconLeft.transform.localScale;
                yield return StartCoroutine(ExpandManager.ExpandObject(IconLeft, 0.9f, 0.05f));
                yield return StartCoroutine(ExpandManager.ExpandObject(IconLeft, saveScale, 0.05f));

                yield return StartCoroutine(ConfirmationPanel.CreatePanel("Перезаписать сохранение?", RewriteSave(), ICancelSave()));

                break;
            case SaveOption.Load:
                LoadAnimator.SetTrigger("DoLoad");

                PanelsManager.confirmPanelActive = true;

                Vector3 loadScale = IconRight.transform.localScale;
                yield return StartCoroutine(ExpandManager.ExpandObject(IconRight, 0.9f, 0.05f));
                yield return StartCoroutine(ExpandManager.ExpandObject(IconRight, loadScale, 0.05f));

                yield return StartCoroutine(ConfirmationPanel.CreatePanel("Загрузить сохранение?", LoadFile(), CancelLoad()));

                break;
        }
    }

    IEnumerator RewriteSave()
    {
        UserData.instance.SavePlayer(saveNum);

        StartCoroutine(FadeManager.FadeObject(IconLeft, false, SaveManager.instance.optionsGradientSpeed));
        StartCoroutine(FadeManager.FadeObject(GradRight, false, SaveManager.instance.optionsGradientSpeed));
        yield return StartCoroutine(ConfirmationPanel.ClosePanel());

        SaveAnimator.SetTrigger("StopSave");

        yield return StartCoroutine(SaveManager.instance.OverrideScreenshot(saveNum, screenshot, overscreenshot, SaveManager.instance.optionsGradientSpeed));

        saveFileFields.resetCassettePosition(IconLeft);
        saveFileFields.OpenOverPanel();
    }

    IEnumerator ICancelSave()
    {
        StartCoroutine(FadeManager.FadeObject(IconLeft, false, SaveManager.instance.optionsGradientSpeed));
        StartCoroutine(FadeManager.FadeObject(GradRight, false, SaveManager.instance.optionsGradientSpeed));
        yield return StartCoroutine(ConfirmationPanel.ClosePanel());

        SaveAnimator.SetTrigger("StopSave");

        saveFileFields.resetCassettePosition(IconLeft);
        saveFileFields.OpenOverPanel();
    }

    public void DeleteAction()
    {
        if (saveFileFields.AllowSaveLoad && !StaticVariables.UIsystemDown && !PanelsManager.confirmPanelActive)
        {
            StartCoroutine(IDelete());
        }
    }

    IEnumerator IDelete()
    {
        PanelsManager.confirmPanelActive = true;

        Vector3 crossScale = DeleteCross.transform.localScale;
        yield return StartCoroutine(ExpandManager.ExpandObject(DeleteCross, 0.8f, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(DeleteCross, crossScale, 0.05f));

        yield return StartCoroutine(ConfirmationPanel.CreatePanel("Удалить сохранение?", IDeleteSave(), ICancelDelete()));
    }
    IEnumerator IDeleteSave()
    {
        StartCoroutine(FadeManager.FadeOnly(screenshot, false, SaveManager.instance.optionsGradientSpeed));
        FadeManager.FadeObject(SavedPanel, false);
        FadeManager.FadeObject(UnSavedPanel, true);

        yield return StartCoroutine(ConfirmationPanel.ClosePanel());

        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;

        if (ES3.FileExists("screenshots/screenshot" + actualSaveNum + ".png"))
        {
            ES3.DeleteFile("screenshots/screenshot" + actualSaveNum + ".png");
            SaveManager.instance.savesTaken[actualSaveNum] = false;
            ES3.Save<bool[]>("saveTaken", SaveManager.instance.savesTaken, "screenshots.es3");

            ES3.DeleteKey("SaveFile" + actualSaveNum, "SaveFiles.es3");

            // Добавить удаление спешл ивентов!
        }
    }

    IEnumerator ICancelDelete()
    {
        yield return StartCoroutine(ConfirmationPanel.ClosePanel());
    }

    IEnumerator LoadFile()
    {
        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;

        StartCoroutine(ConfirmationPanel.ClosePanel());

        yield return StartCoroutine(PanelsManager.instance.ILoadGame(actualSaveNum));

        saveFileFields.resetCassettePosition(IconRight);
    }

    IEnumerator CancelLoad()
    {
        SaveButton.interactable = true;
        LoadButton.interactable = true;
        DeleteCross.GetComponent<Button>().interactable = true;

        StartCoroutine(FadeManager.FadeObject(IconRight, false, SaveManager.instance.optionsGradientSpeed));
        StartCoroutine(FadeManager.FadeObject(GradLeft, false, SaveManager.instance.optionsGradientSpeed));
        yield return StartCoroutine(ConfirmationPanel.ClosePanel());

        SaveAnimator.SetTrigger("StopLoad");

        saveFileFields.resetCassettePosition(IconRight);
        saveFileFields.OpenOverPanel();
    }
}