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
    Delete
}
public class SaveChoiseAnimator : MonoBehaviour, IDeleteActionExecutor, ILoadActionExecutor, IRewriteSaveExecutor
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
        saveFileFields = transform.parent.GetComponent<SaveFileFields>();
        saveChoiseIconAnimator = saveFileFields._SaveChoiseIconAnimator;

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

    public void SaveLoadDeleteAction(SaveOption option)
    {
        StartCoroutine(IAction(option));
    }

    private IEnumerator IAction(SaveOption option)
    {
        switch (option)
        {
            case SaveOption.Save:
                yield return StartCoroutine(RewriteSave());
                break;
            case SaveOption.Load:
                yield return StartCoroutine(LoadAction());
                break;
            case SaveOption.Delete:
                yield return StartCoroutine(DeleteAction());
                break;
        }
    }

    public IEnumerator RewriteSave()
    {
        yield return StartCoroutine(SaveManagerActions.RewriteSaveAction(saveFileFields, SaveButton, screenshot, overscreenshot, saveNum));
    }

    public IEnumerator LoadAction()
    {
        yield return StartCoroutine(SaveManagerActions.LoadAction(saveFileFields, LoadButton, PanelsManager.instance, saveNum));
    }

    public IEnumerator DeleteAction()
    {
        yield return StartCoroutine(SaveManagerActions.DeleteAction(saveFileFields, DeleteCross, screenshot, saveNum));
    }
}
