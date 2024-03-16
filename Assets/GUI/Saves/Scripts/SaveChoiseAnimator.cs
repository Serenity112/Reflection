using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading;

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
    [SerializeField] public SaveChoiseButton SaveButton;
    [SerializeField] public SaveChoiseButton LoadButton;
    [SerializeField] public DeleteCrossButton DeleteCross;

    private int saveNum;

    private GameObject screenshot;
    private GameObject overscreenshot;

    private SaveFileFields saveFileFields;
    private SaveChoiseIconAnimator saveChoiseIconAnimator;

    private float speed = 4f;

    private void Awake()
    {
        saveFileFields = GetComponent<SaveFileFields>();
        saveChoiseIconAnimator = GetComponent<SaveChoiseIconAnimator>();

        screenshot = saveFileFields.Screenshot;
        overscreenshot = saveFileFields.OverSreenshot;
        saveNum = saveFileFields.saveNum;
    }

    public void ResetPanelSync()
    {
        SaveButton.ResetButtonState();
        LoadButton.ResetButtonState();
        DeleteCross.ResetButtonState();
        FadeManager.FadeOnly(DeleteCross.gameObject, false);
        saveChoiseIconAnimator.ResetPanel();
    }

    public void SaveLoadAction(SaveOption option)
    {
        StartCoroutine(ISaveAction(option));
    }

    public IEnumerator ISaveAction(SaveOption option)
    {
        switch (option)
        {
            case SaveOption.Save:
                yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel(
                    "Перезаписать сохранение?", IRewriteSave(), ICancelSave()));

                break;
            case SaveOption.Load:
                yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel(
                    "Загрузить сохранение?", ILoadFile(), ICancelLoad()));
                break;
        }
    }

    private IEnumerator IRewriteSave()
    {
        SaveManagerStatic.UiBloker = false;
        SaveButton.ExitAction();

        int actualSaveNum = SaveManager.instance.GetActualSaveNum(saveNum);

        string datetime = DateTime.Now.ToString("HH:mm dd/MM/yy");
        saveFileFields.Datetime.SetText(datetime);

        new Thread(() =>
        {
            SaveManager.instance.AddSaveData(actualSaveNum, datetime);
            UserData.instance.SavePlayer(actualSaveNum);
        }).Start();

        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel()
        }));

        SaveButton.ResetButtonState();

        yield return StartCoroutine(SaveManager.instance.OverrideScreenshot(saveNum, screenshot, overscreenshot, speed));

        SaveManagerStatic.ClickBlocker = false;
    }

    private IEnumerator ICancelSave()
    {
        SaveManagerStatic.UiBloker = false;
        SaveButton.ExitAction();

        yield return StartCoroutine(ConfirmationPanel.instance.ClosePanel());
        SaveButton.ResetButtonState();

        SaveManagerStatic.ClickBlocker = false;
    }

    // Удаление

    public IEnumerator DeleteAction()
    {
        yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel(
            "Удалить сохранение?", IDeleteSave(), ICancelDelete()));
    }

    private IEnumerator IDeleteSave()
    {
        SaveManagerStatic.UiBloker = false;
        StartCoroutine(saveFileFields.OpenOverPanel());
        DeleteCross.DisappearCross();

        FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, true);
        saveFileFields._FirstSaveAnimator.ResetPanelSync();

        List<IEnumerator> fadeout = new List<IEnumerator>()
        {
            FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, false, speed),
            FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, true, speed),
            FadeManager.FadeOnly(screenshot, false, speed),
            saveFileFields.CloseOverPanel(),
            FadeManager.FadeOnly(saveFileFields.NoImage, true, speed),
            saveFileFields.Datetime.IHideText(),
            ConfirmationPanel.instance.ClosePanel()
        };

        yield return StartCoroutine(CoroutineUtils.WaitForAll(fadeout));

        saveFileFields.Datetime.SetText("");
        SaveManager.instance.DeleteSave(saveNum);

        yield return new WaitForSeconds(0.1f);
        SaveManagerStatic.ClickBlocker = false;
    }

    private IEnumerator ICancelDelete()
    {
        SaveManagerStatic.UiBloker = false;
        StartCoroutine(saveFileFields.OpenOverPanel());
        DeleteCross.DisappearCross();

        yield return StartCoroutine(ConfirmationPanel.instance.ClosePanel());

        SaveManagerStatic.ClickBlocker = false;
    }

    // Загрузка
    private IEnumerator ILoadFile()
    {
        SaveManagerStatic.UiBloker = false;
        LoadButton.ExitAction();

        int actualSaveNum = SaveManager.instance.GetActualSaveNum(saveNum);

        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel(),
            PanelsManager.instance.ILoadGame(actualSaveNum)
        }));

        LoadButton.ResetButtonState();

        saveFileFields._SaveChoiseAnimator.ResetPanelSync();
        SaveManagerStatic.ClickBlocker = false;
    }

    private IEnumerator ICancelLoad()
    {
        SaveManagerStatic.UiBloker = false;
        LoadButton.ExitAction();

        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel()
        }));

        LoadButton.ResetButtonState();
        SaveManagerStatic.ClickBlocker = false;
    }

    public void HideCross()
    {
        DeleteCross.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
