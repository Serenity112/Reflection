using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenuLoad : MonoBehaviour
{
    [SerializeField] private GameObject Cassette;
    [SerializeField] private Button ButtonLoad;
    [SerializeField] private Animator cassetteAnimator;

    private int saveNum;
    private GameObject screenshot;
    private GameObject SavedPanel;
    private GameObject MainMenuPanel;

    private IEnumerator CassetteFadeIn;
    private IEnumerator CassetteFadeOut;

    private SaveFileFields saveFileFields;
    void Start()
    {
        saveFileFields = GetComponent<SaveFileFields>();
        saveNum = saveFileFields.saveNum;
        screenshot = saveFileFields.screenshot;
        SavedPanel = saveFileFields.SavedPanel;
        MainMenuPanel = saveFileFields.MainMenuPanel;

        ButtonLoad.interactable = true;
    }

    public void AppearCassette()
    {
        if (ButtonLoad.interactable && !PanelsManager.confirmPanelActive)
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
        if (ButtonLoad.interactable && !PanelsManager.confirmPanelActive)
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
        PanelsManager.confirmPanelActive = true;

        Vector3 currScale = Cassette.transform.localScale;

        cassetteAnimator.SetTrigger("DoLoad");

        yield return StartCoroutine(ExpandManager.ExpandObject(Cassette, 0.9f, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(Cassette, currScale, 0.05f));

        yield return StartCoroutine(ConfirmationPanel.CreatePanel("Загрузить !!!сохранение?", LoadFile(), CancelLoad()));
    }

    IEnumerator LoadFile()
    {
        StartCoroutine(FadeManager.FadeObject(Cassette, false, SaveManager.instance.optionsGradientSpeed));
        StartCoroutine(ConfirmationPanel.ClosePanel());

        yield return StartCoroutine(MMPanelsManager.instance.ILoadGame(saveNum));

        cassetteAnimator.SetTrigger("StopLoad");
    }

    IEnumerator CancelLoad()
    {
        StartCoroutine(FadeManager.FadeObject(Cassette, false, SaveManager.instance.optionsGradientSpeed));
        yield return StartCoroutine(ConfirmationPanel.ClosePanel());

        cassetteAnimator.SetTrigger("StopLoad");

        saveFileFields.resetCassettePosition(Cassette);
    }
}