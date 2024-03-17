using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SaveManagerActions : MonoBehaviour
{
    private static float speed = 5f;

    private static SaveManagerActions instance;

    private void Awake()
    {
        instance = this;
    }

    // Удаление
    public static IEnumerator DeleteAction(
        SaveFileFields saveFileFields,
        DeleteCrossButton deleteCrossButton,
        GameObject screenshot,
        int saveNum,
        bool mainMenu = false)
    {
        return ConfirmationPanel.instance.CreateConfirmationPanel(
            "Удалить сохранение?",
            IDeleteSave(saveFileFields, deleteCrossButton, screenshot, saveNum, mainMenu),
            ICancelDelete(saveFileFields, deleteCrossButton));
    }

    private static IEnumerator IDeleteSave(
        SaveFileFields saveFileFields,
        DeleteCrossButton deleteCrossButton,
        GameObject screenshot,
        int saveNum, bool mainMenu)
    {
        SaveManagerStatic.UiBloker = false;
        instance.StartCoroutine(saveFileFields.OpenOverPanel());
        deleteCrossButton.DisappearCross();

        List<IEnumerator> fadeout = new List<IEnumerator>()
        {
            FadeManager.FadeOnly(screenshot, false, speed),
            saveFileFields.CloseOverPanel(),
            saveFileFields.Datetime.IHideText(),
            ConfirmationPanel.instance.ClosePanel()
        };

        if (mainMenu)
        {
            fadeout.Add(FadeManager.FadeObject(saveFileFields._MainMenuLoadObject, false, speed));
        }
        else
        {
            FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, true);
            saveFileFields._FirstSaveAnimator.ResetPanelSync();

            fadeout.Add(FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, false, speed));
            fadeout.Add(FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, true, speed));
        }

        yield return instance.StartCoroutine(CoroutineUtils.WaitForAll(fadeout));

        saveFileFields.Datetime.ClearText();
        SaveManager.instance.DeleteSave(saveNum);

        yield return new WaitForSeconds(0.1f);
        SaveManagerStatic.ClickBlocker = false;
    }

    private static IEnumerator ICancelDelete(
        SaveFileFields saveFileFields,
        DeleteCrossButton deleteCrossButton)
    {
        SaveManagerStatic.UiBloker = false;
        instance.StartCoroutine(saveFileFields.OpenOverPanel());
        deleteCrossButton.DisappearCross();

        yield return instance.StartCoroutine(ConfirmationPanel.instance.ClosePanel());

        SaveManagerStatic.ClickBlocker = false;
    }


    // Загрузка

    public static IEnumerator LoadAction(
        SaveFileFields saveFileFields,
        ISaveSystemButton LoadButton,
        IPanelsManager panelsManager,
        int saveNum)
    {
        return ConfirmationPanel.instance.CreateConfirmationPanel(
            "Загрузить сохранение?",
            ILoad(saveFileFields, LoadButton, panelsManager, saveNum),
            ICancelLoad(LoadButton));
    }

    private static IEnumerator ILoad(
        SaveFileFields saveFileFields,
        ISaveSystemButton LoadButton,
        IPanelsManager panelsManager,
        int saveNum)
    {
        SaveManagerStatic.UiBloker = false;
        LoadButton.ExitAction();

        int actualSaveNum = SaveManager.instance.GetActualSaveNum(saveNum);

        yield return instance.StartCoroutine(
            CoroutineUtils.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel(),
            panelsManager.ILoadGame(actualSaveNum)
        }));

        LoadButton.ResetButtonState();

        saveFileFields._SaveChoiseAnimator.ResetPanelSync();
        SaveManagerStatic.ClickBlocker = false;
    }

    private static IEnumerator ICancelLoad(
        ISaveSystemButton LoadButton)
    {
        SaveManagerStatic.UiBloker = false;
        LoadButton.ExitAction();

        yield return instance.StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel()
        }));

        LoadButton.ResetButtonState();
        SaveManagerStatic.ClickBlocker = false;
    }

    // Перезапись сохранения

    public static IEnumerator RewriteSaveAction(
        SaveFileFields saveFileFields,
        ISaveSystemButton SaveButton,
        GameObject screenshot,
        GameObject overscreenshot,
        int saveNum)
    {
        return ConfirmationPanel.instance.CreateConfirmationPanel(
            "Перезаписать сохранение?",
            IRewriteSave(saveFileFields, SaveButton, screenshot, overscreenshot, saveNum),
            ICancelSave(SaveButton));
    }

    private static IEnumerator IRewriteSave(
        SaveFileFields saveFileFields,
        ISaveSystemButton SaveButton,
        GameObject screenshot,
        GameObject overscreenshot,
        int saveNum)
    {
        SaveManagerStatic.UiBloker = false;
        SaveButton.ExitAction();

        int actualSaveNum = SaveManager.instance.GetActualSaveNum(saveNum);

        string datetime = DateTime.Now.ToString(SaveManager.DateTimeFormat);
        saveFileFields.Datetime.SetText(datetime);

        new Thread(() =>
        {
            SaveManager.instance.AddSaveData(actualSaveNum, datetime);
            UserData.instance.SavePlayer(actualSaveNum);
        }).Start();

        yield return instance.StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>
        {
            ConfirmationPanel.instance.ClosePanel()
        }));

        SaveButton.ResetButtonState();

        yield return instance.StartCoroutine(SaveManager.instance.OverrideScreenshot(saveNum, screenshot, overscreenshot, speed));

        SaveManagerStatic.ClickBlocker = false;
    }

    private static IEnumerator ICancelSave(
        ISaveSystemButton SaveButton)
    {
        SaveManagerStatic.UiBloker = false;
        SaveButton.ExitAction();

        yield return instance.StartCoroutine(ConfirmationPanel.instance.ClosePanel());
        SaveButton.ResetButtonState();

        SaveManagerStatic.ClickBlocker = false;
    }
}
