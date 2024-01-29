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
    [SerializeField] private SaveChoiseButton SaveButton;
    [SerializeField] private SaveChoiseButton LoadButton;
    [SerializeField] public DeleteCrossButton DeleteCross;

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

        /*saveNum = saveFileFields.saveNum;
        GradLeft = saveFileFields.GradLeft;
        GradRight = saveFileFields.GradRight;
        IconLeft = saveFileFields.IconLeft;
        IconRight = saveFileFields.IconRight;

        screenshot = saveFileFields.screenshot;
        overscreenshot = saveFileFields.overscreenshot;
        SavedPanel = saveFileFields._SaveChoiseAnimator;
        UnSavedPanel = saveFileFields._FirstSaveAnimator;
*/
        saveChoiseIconAnimator = GetComponent<SaveChoiseIconAnimator>();
    }

    public void ResetPanel()
    {
        SaveButton.ResetButtonState();
        LoadButton.ResetButtonState();
        DeleteCross.ResetButtonState();
    }

    public void SaveAction(SaveOption option)
    {
        if (!SaveManagerStatic.UIsystemDown && !SaveManagerStatic.OverlayPanelActive)
        {
            
        }

        StartCoroutine(ISaveAction(option));
    }
    public IEnumerator ISaveAction(SaveOption option)
    {
        switch (option)
        {
            case SaveOption.Save:
                //SaveAnimator.SetTrigger("DoSave");

                SaveManagerStatic.OverlayPanelActive = true;

                Vector3 saveScale = IconLeft.transform.localScale;
                yield return StartCoroutine(ExpandManager.ExpandObject(IconLeft, 0.9f, 0.05f));
                yield return StartCoroutine(ExpandManager.ExpandObject(IconLeft, saveScale, 0.05f));

                yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel("Перезаписать сохранение?", IRewriteSave(), ICancelSave()));

                break;
            case SaveOption.Load:
                //LoadAnimator.SetTrigger("DoLoad");

                SaveManagerStatic.OverlayPanelActive = true;

                Vector3 loadScale = IconRight.transform.localScale;
                yield return StartCoroutine(ExpandManager.ExpandObject(IconRight, 0.9f, 0.05f));
                yield return StartCoroutine(ExpandManager.ExpandObject(IconRight, loadScale, 0.05f));

                yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel("Загрузить сохранение?", LoadFile(), CancelLoad()));
                SaveManagerStatic.OverlayPanelActive = false;
                break;
        }
    }

    IEnumerator IRewriteSave()
    {
        SaveManagerStatic.UIsystemDown = true;
        SaveManagerStatic.OverlayPanelActive = false;

        string datetime = DateTime.Now.ToString("HH:mm dd/MM/yy");
        //SaveManager.instance.SaveDateTime(saveNum, datetime);
        saveFileFields.Datetime.GetComponent<Text>().text = datetime;

        UserData.instance.SavePlayer(saveNum);

        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
        StartCoroutine(saveFileFields.OpenOverPanel());

        saveChoiseIconAnimator.StopAllCoroutines();
        saveChoiseIconAnimator.ExitLeft();

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel()
        }));

        yield return StartCoroutine(SaveManager.instance.OverrideScreenshot(saveNum, screenshot, overscreenshot, SaveManager.instance.speed));
        //SaveAnimator.SetTrigger("StopSave");

        SaveManagerStatic.UIsystemDown = false;
        saveFileFields.resetCassettePosition(IconLeft);
    }

    IEnumerator ICancelSave()
    {
        SaveManagerStatic.UIsystemDown = true;
        SaveManagerStatic.OverlayPanelActive = false;

        StartCoroutine(saveFileFields.OpenOverPanel());
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();

        saveChoiseIconAnimator.StopAllCoroutines();
        saveChoiseIconAnimator.ExitLeft();

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel()
        }));

        //SaveAnimator.SetTrigger("StopSave");

        saveFileFields.resetCassettePosition(IconLeft);

        SaveManagerStatic.UIsystemDown = false;
    }

    // Удаление сейва

    public void DeleteAction()
    {
        if (!SaveManagerStatic.UIsystemDown && !SaveManagerStatic.OverlayPanelActive)
        {
            StartCoroutine(IDeleteDialog());
        }
    }

    IEnumerator IDeleteDialog()
    {
        SaveManagerStatic.OverlayPanelActive = true;
        yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel("Удалить сохранение?", IDeleteSave(), ICancelDelete()));
    }

    IEnumerator IDeleteSave()
    {
        SaveManagerStatic.UIsystemDown = true;
        SaveManagerStatic.OverlayPanelActive = false;

        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
        /*if (saveFileFields.exitLeft && saveFileFields.exitRight)
        {
            StartCoroutine(saveFileFields.CloseOverPanel());
        }*/

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeObject(SavedPanel, false, SaveManager.instance.speed * 2),
            FadeManager.FadeOnly(screenshot, false, SaveManager.instance.speed),
            FadeManager.FadeOnly(saveFileFields.NoImage, true, SaveManager.instance.speed),
            //FadeManager.FadeObject(saveFileFields.Datetime, false, SaveManager.instance.speed),
            ConfirmationPanel.instance.ClosePanel()
        }));

        FadeManager.FadeObject(UnSavedPanel, true);
        saveChoiseIconAnimator.ResetPanel();
        DeleteCross.GetComponent<CanvasGroup>().alpha = 0f;

        SaveManager.instance.RemoveDateTime(saveNum);
        SaveManager.instance.DeleteSave(saveNum);

        yield return new WaitForSeconds(0.1f);

        SaveManagerStatic.UIsystemDown = false;
    }

    IEnumerator ICancelDelete()
    {
        SaveManagerStatic.UIsystemDown = true;
        SaveManagerStatic.OverlayPanelActive = false;

        StartCoroutine(saveFileFields.OpenOverPanel());
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();

        yield return StartCoroutine(ConfirmationPanel.instance.ClosePanel());

        SaveManagerStatic.UIsystemDown = false;
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
        SaveManagerStatic.UIsystemDown = true;
        SaveManagerStatic.OverlayPanelActive = false;

        StartCoroutine(saveFileFields.OpenOverPanel());
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();

        saveChoiseIconAnimator.StopAllCoroutines();
        saveChoiseIconAnimator.ExitRight();

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel()
        }));

        //LoadAnimator.SetTrigger("StopLoad");

        saveFileFields.resetCassettePosition(IconRight);
        SaveManagerStatic.UIsystemDown = false;
    }

    public void HideCross()
    {
        DeleteCross.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
