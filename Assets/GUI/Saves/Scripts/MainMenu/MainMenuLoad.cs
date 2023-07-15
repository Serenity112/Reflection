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

    public SaveFileFields saveFileFields;

    [HideInInspector] public bool onEnter = false;

    void Start()
    {
        saveFileFields = GetComponent<SaveFileFields>();
        saveNum = saveFileFields.saveNum;
        screenshot = saveFileFields.screenshot;
        MainMenuPanel = saveFileFields.MainMenuPanel;

        ButtonLoad.interactable = true;
    }

    public IEnumerator AppearCassette()
    {
        if (ButtonLoad.interactable)
        {
            if (CassetteFadeOut != null)
                StopCoroutine(CassetteFadeOut);

            CassetteFadeIn = FadeManager.FadeObject(Cassette, true, SaveManager.instance.optionsGradientSpeed);
            yield return StartCoroutine(CassetteFadeIn);
        }

        yield return null;
    }

    public IEnumerator DisappearCassette()
    {
        if (ButtonLoad.interactable)
        {
            if (CassetteFadeIn != null)
                StopCoroutine(CassetteFadeIn);

            CassetteFadeOut = FadeManager.FadeObject(Cassette, false, SaveManager.instance.optionsGradientSpeed);
            yield return StartCoroutine(CassetteFadeOut);
        }

        yield return null;
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
        StaticVariables.OverlayPanelActive = true;
        cassetteAnimator.SetTrigger("DoLoad");

        Vector3 currScale = Cassette.transform.localScale;
        yield return StartCoroutine(ExpandManager.ExpandObject(Cassette, 0.9f, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(Cassette, currScale, 0.05f));

        yield return StartCoroutine(ConfirmationPanel.instance.CreateConfirmationPanel("Загрузить сохранение?", LoadFile(), CancelLoad()));
    }

    IEnumerator LoadFile()
    {
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            FadeManager.FadeObject(Cassette, false, SaveManager.instance.optionsGradientSpeed),
            ConfirmationPanel.instance.ClosePanel(),
            MMPanelsManager.instance.ILoadGameFromMainMenu(saveNum)
        });

        cassetteAnimator.SetTrigger("StopLoad");
    }

    IEnumerator CancelLoad()
    {
        StaticVariables.OverlayPanelActive = false;

        StartCoroutine(DisappearCassette());
        StartCoroutine(saveFileFields.OpenOverPanel());
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();

        yield return StartCoroutine(ConfirmationPanel.instance.ClosePanel());

        cassetteAnimator.SetTrigger("StopLoad");

        saveFileFields.resetCassettePosition(Cassette);
    }


    // Удаление сейва, активируется крестиком
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
        StartCoroutine(FadeManager.FadeOnly(screenshot, false, SaveManager.instance.optionsGradientSpeed));
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();
        FadeManager.FadeObject(MainMenuPanel, false);
        StartCoroutine(saveFileFields.CloseOverPanel());

        int actualSaveNum = SaveManager.instance.currentPage * SaveManager.savesPerPage + saveNum;

        StartCoroutine(FadeManager.FadeObject(saveFileFields.datetime, false, SaveManager.instance.optionsGradientSpeed));
        SaveManager.instance.RemoveDateTime(actualSaveNum);

        yield return StartCoroutine(ConfirmationPanel.instance.ClosePanel());

        SaveManager.instance.DeleteSave(actualSaveNum);
    }

    IEnumerator ICancelDelete()
    {
        StaticVariables.OverlayPanelActive = false;

        StartCoroutine(DisappearCassette());
        StartCoroutine(saveFileFields.OpenOverPanel());
        DeleteCross.GetComponent<DeleteCrossButton>().DisappearCross();

        yield return StartCoroutine(ConfirmationPanel.instance.ClosePanel());
    }
}
