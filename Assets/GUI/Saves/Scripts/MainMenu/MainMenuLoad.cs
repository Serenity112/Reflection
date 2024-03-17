using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLoad : MonoBehaviour, IDeleteActionExecutor, ILoadActionExecutor
{
    public SaveFileFields saveFileFields;
    [SerializeField] private MainMenuLoadButton LoadButton;
    [SerializeField] private Animator cassetteAnimator;
    public DeleteCrossButton DeleteCross;

    private int saveNum;
    private GameObject screenshot;

    void Awake()
    {
        saveFileFields = transform.parent.GetComponent<SaveFileFields>();
        saveNum = saveFileFields.saveNum;
        screenshot = saveFileFields.Screenshot;
    }

    public void SaveLoadDeleteAction(SaveOption option)
    {
        StartCoroutine(IAction(option));
    }

    private IEnumerator IAction(SaveOption option)
    {
        switch (option)
        {
            case SaveOption.Load:
                yield return StartCoroutine(LoadAction());
                break;
            case SaveOption.Delete:
                yield return StartCoroutine(DeleteAction());
                break;
        }
    }

    public IEnumerator LoadAction()
    {
        yield return StartCoroutine(SaveManagerActions.LoadAction(saveFileFields, LoadButton, MMPanelsManager.instance, saveNum));
    }

    public IEnumerator DeleteAction()
    {
        yield return StartCoroutine(SaveManagerActions.DeleteAction(saveFileFields, DeleteCross, screenshot, saveNum, true));
    }

    public void ResetPanelSync()
    {
        LoadButton.ResetButtonState();
        DeleteCross.ResetButtonState();
        FadeManager.FadeOnly(DeleteCross.gameObject, false);
    }
}
