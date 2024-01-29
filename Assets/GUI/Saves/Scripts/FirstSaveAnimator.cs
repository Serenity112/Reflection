using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class FirstSaveAnimator : MonoBehaviour
{
    [SerializeField] private FirstSaveButton firstSaveButton;

    private GameObject screenshot;
    private SaveFileFields saveFileFields;
    private int localSaveNum;
    private float speed = 4f;

    private IEnumerator CassetteFadeIn;
    private IEnumerator CassetteFadeOut;

    private void Awake()
    {
        saveFileFields = GetComponent<SaveFileFields>();

        localSaveNum = saveFileFields.saveNum;
        screenshot = saveFileFields.Screenshot;
    }

    public void ResetPanel()
    {
        Awake();
        firstSaveButton.ResetButtonState();
    }

    public void FirstSaveIconClick()
    {
        StartCoroutine(IFirstSaveIconClick());
    }

    private IEnumerator IFirstSaveIconClick()
    {
        Resources.UnloadUnusedAssets();

        int actualSaveNum = SaveManager.instance.GetActualSaveNum(localSaveNum);

        // Время покрутиться
        yield return new WaitForSeconds(0.3f);

        // Установка текущего времени
        string datetime = DateTime.Now.ToString("HH:mm dd/MM/yy");
        saveFileFields.Datetime.HideText();
        saveFileFields.Datetime.SetText(datetime);

        StartCoroutine(SaveManager.instance.SetScreenshot(localSaveNum, screenshot));

        // Включение панельки сейв/лоад
        saveFileFields._SaveChoiseAnimator.ResetPanel();
        FadeManager.FadeObject(saveFileFields._SaveChoiseAnimatorObject, true);

        // Сейв
        new Thread(() =>
        {
            SaveManager.instance.AddSaveData(actualSaveNum, datetime);
            SaveManager.instance.SaveCurrentData();
            UserData.instance.SavePlayer(actualSaveNum);
        }).Start();


        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            saveFileFields.Datetime.IShowText(),
            FadeManager.FadeObject(saveFileFields._FirstSaveAnimatorObject, false, speed),
            //FadeManager.FadeToTargetAlpha(saveFileFields.NoImage, SaveManager.instance.frameAplhaOff, speed),
            FadeManager.FadeObject(screenshot, true, speed),
        }));

        firstSaveButton.ResetButtonState();
        SaveManagerStatic.UIsystemDown = false;
    }

    public IEnumerator IAppearCassette()
    {
        if (CassetteFadeOut != null)
            StopCoroutine(CassetteFadeOut);

        CassetteFadeIn = FadeManager.FadeObject(firstSaveButton.CassetteImg, true, speed);
        yield return StartCoroutine(CassetteFadeIn);
    }

    public IEnumerator IDisappearCassette()
    {
        if (CassetteFadeIn != null)
            StopCoroutine(CassetteFadeIn);

        CassetteFadeOut = FadeManager.FadeObject(firstSaveButton.CassetteImg, false, speed);
        yield return StartCoroutine(CassetteFadeOut);
    }

    public void DisappearCassetteInstant()
    {
        FadeManager.FadeObject(firstSaveButton.CassetteImg, false);
    }
}
