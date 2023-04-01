using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuLoad : MonoBehaviour
{
    [SerializeField] private GameObject Cassette;
    [SerializeField] private Button ButtonLoad;
    [SerializeField] private Animator cassetteAnimator;
    public GameObject DeleteCross;


    private int saveNum;
    private GameObject screenshot;
    private GameObject MainMenuPanel;

    private IEnumerator CassetteFadeIn;
    private IEnumerator CassetteFadeOut;


    private SaveFileFields saveFileFields;
    void Start()
    {
        saveFileFields = GetComponent<SaveFileFields>();
        saveNum = saveFileFields.saveNum;
        screenshot = saveFileFields.screenshot;
        MainMenuPanel = saveFileFields.MainMenuPanel;

        ButtonLoad.interactable = true;
    }

    public void AppearCassette()
    {
        if (ButtonLoad.interactable)
        {
            if (CassetteFadeOut != null)
                StopCoroutine(CassetteFadeOut);

            CassetteFadeIn = FadeManager.FadeObject(Cassette, true, SaveManager.instance.optionsGradientSpeed);
            StartCoroutine(CassetteFadeIn);

            saveFileFields.CloseOverPanel();
        }
    }

    public void DisappearCassette()
    {
        if (ButtonLoad.interactable)
        {
            if (CassetteFadeIn != null)
                StopCoroutine(CassetteFadeIn);

            CassetteFadeOut = FadeManager.FadeObject(Cassette, false, SaveManager.instance.optionsGradientSpeed);
            StartCoroutine(CassetteFadeOut);

            saveFileFields.OpenOverPanel();
        }
    }

    public void Click()
    {
        if (!StaticVariables.UIsystemDown)
        {
            StartCoroutine(IClick());
        }
    }

    IEnumerator IClick()
    {
        StaticVariables.ConfirmationPanelActive = true;

        Vector3 currScale = Cassette.transform.localScale;

        cassetteAnimator.SetTrigger("DoLoad");

        yield return StartCoroutine(ExpandManager.ExpandObject(Cassette, 0.9f, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(Cassette, currScale, 0.05f));

        yield return StartCoroutine(ConfirmationPanel.CreatePanel("Загрузить сохранение?", LoadFile(), CancelLoad()));
    }

    IEnumerator LoadFile()
    {
        StartCoroutine(FadeManager.FadeObject(Cassette, false, SaveManager.instance.optionsGradientSpeed));
        StartCoroutine(ConfirmationPanel.ClosePanel());

        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;
        yield return StartCoroutine(MMPanelsManager.instance.ILoadGame(actualSaveNum));

        cassetteAnimator.SetTrigger("StopLoad");
    }

    IEnumerator CancelLoad()
    {
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
        StartCoroutine(FadeManager.FadeObject(Cassette, false, SaveManager.instance.optionsGradientSpeed));
        yield return StartCoroutine(ConfirmationPanel.ClosePanel());

        cassetteAnimator.SetTrigger("StopLoad");

        saveFileFields.resetCassettePosition(Cassette);
    }


    // Удаление сейва
    public void DeleteAction()
    {
        if (saveFileFields.AllowSaveLoad && !StaticVariables.UIsystemDown && !StaticVariables.ConfirmationPanelActive)
        {
            StartCoroutine(IDeleteDialog());
        }
    }

    IEnumerator IDeleteDialog()
    {
        StaticVariables.ConfirmationPanelActive = true;
        yield return StartCoroutine(ConfirmationPanel.CreatePanel("Удалить сохранение?", IDeleteSave(), ICancelDelete()));
    }
    IEnumerator IDeleteSave()
    {
        StartCoroutine(FadeManager.FadeOnly(screenshot, false, SaveManager.instance.optionsGradientSpeed));
        FadeManager.FadeObject(MainMenuPanel, false);
        saveFileFields.CloseOverPanel();
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();

        StartCoroutine(FadeManager.FadeObject(saveFileFields.datetime, false, SaveManager.instance.optionsGradientSpeed));
        SaveManager.instance.RemoveDateTime(saveNum);

        yield return StartCoroutine(ConfirmationPanel.ClosePanel());

        SaveManager.instance.DeleteSave(saveNum);
    }

    IEnumerator ICancelDelete()
    {
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
        yield return StartCoroutine(ConfirmationPanel.ClosePanel());         
    }
}
