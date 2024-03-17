using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FirstSaveAnimator : MonoBehaviour
{
    [SerializeField] private FirstSaveButton firstSaveButton;

    private GameObject screenshot;
    private SaveFileFields saveFileFields;
    private int localSaveNum;
    private float speed = 4f;

    private IEnumerator CassetteFadeIn;
    private IEnumerator CassetteFadeOut;

    public bool Animating { get; set; } = false;

    private void Awake()
    {
        saveFileFields = transform.parent.GetComponent<SaveFileFields>();

        localSaveNum = saveFileFields.saveNum;
        screenshot = saveFileFields.Screenshot;
    }

    public void ResetPanelSync()
    {
        Animating = false;
        firstSaveButton.ResetButtonState();
        FadeManager.FadeOnly(firstSaveButton.CassetteImg, false);
        saveFileFields.Datetime.HideText();
    }

    public IEnumerator IFirstSaveIconClick()
    {
        Animating = true;
        int actualSaveNum = SaveManager.instance.GetActualSaveNum(localSaveNum);

        // Время покрутиться
        yield return new WaitForSeconds(0.3f);

        // Установка текущего времени
        string datetime = DateTime.Now.ToString(SaveManager.DateTimeFormat);
        saveFileFields.Datetime.HideText();
        saveFileFields.Datetime.SetText(datetime);

        StartCoroutine(SaveManager.instance.SetScreenshot(localSaveNum, screenshot));

        // Включение панельки сейв/лоад
        StartCoroutine(saveFileFields.OpenOverPanel());

        FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, true);
        saveFileFields._SaveChoiseAnimator.ResetPanelSync();

        // Сейв
        new Thread(() =>
        {
            SaveManager.instance.AddSaveData(actualSaveNum, datetime);
            UserData.instance.SavePlayer(actualSaveNum);
        }).Start();

        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            saveFileFields.Datetime.IShowText(),
            FadeManager.FadeObject(firstSaveButton.CassetteImg, false, speed),
            FadeManager.FadeObject(screenshot, true, speed),
        }));

        firstSaveButton.ResetButtonState();
        FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, false);
        SaveManagerStatic.ClickBlocker = false;
        Animating = false;
    }

    public IEnumerator IAppearCassette()
    {
        if (!SaveManagerStatic.UiBloker && !Animating)
        {
            if (CassetteFadeOut != null)
                StopCoroutine(CassetteFadeOut);

            CassetteFadeIn = FadeManager.FadeObject(firstSaveButton.CassetteImg, true, speed);
            yield return StartCoroutine(CassetteFadeIn);
        }
    }

    public IEnumerator IDisappearCassette()
    {
        if (!SaveManagerStatic.UiBloker && !Animating)
        {
            if (CassetteFadeIn != null)
                StopCoroutine(CassetteFadeIn);

            CassetteFadeOut = FadeManager.FadeObject(firstSaveButton.CassetteImg, false, speed);
            yield return StartCoroutine(CassetteFadeOut);
        }
    }

    public void DisappearCassetteInstant()
    {
        FadeManager.FadeObject(firstSaveButton.CassetteImg, false);
    }
}
