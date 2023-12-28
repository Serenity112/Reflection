using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileFields : MonoBehaviour
{
    public int saveNum;

    public GameObject SavedPanel;
    public GameObject UnSavedPanel;
    public GameObject MainMenuPanel;

    public GameObject screenshot;
    public GameObject overscreenshot;
    public GameObject overPanel;

    public GameObject GradLeft;
    public GameObject GradRight;
    public GameObject IconLeft;
    public GameObject IconRight;

    public GameObject NoImage;

    public GameObject datetime;

    public bool OnMouseEnter;

    public bool exitLeft;
    public bool exitRight;

    private IEnumerator overPanelIn;
    private IEnumerator overPanelOut;

    private IEnumerator overPanelInColor;
    private IEnumerator overPanelOutColor;

    private float op_alpha = 0.4f;

    void Awake()
    {
        OnMouseEnter = false;
        exitLeft = true;
        exitRight = true;
    }

    public IEnumerator OpenOverPanelColor()
    {
        if (!StaticVariables.OverlayPanelActive)
        {
            if (overPanelOutColor != null)
                StopCoroutine(overPanelOutColor);

            overPanelInColor = FadeManager.FadeImageToColor(overPanel.GetComponent<Image>(), new Color(0, 0, 0, op_alpha), SaveManager.instance.speed);
            yield return StartCoroutine(overPanelInColor);
        }
    }

    public IEnumerator CloseOverPanelColor()
    {
        if (!StaticVariables.OverlayPanelActive)
        {
            if (overPanelInColor != null)
                StopCoroutine(overPanelInColor);

            overPanelOutColor = FadeManager.FadeImageToColor(overPanel.GetComponent<Image>(), new Color(0, 0, 0, 0), SaveManager.instance.speed);
            yield return StartCoroutine(overPanelOutColor);
        }
    }

    public IEnumerator OpenOverPanel(float coef = 1)
    {
        if (!StaticVariables.OverlayPanelActive)
        {
            if (overPanelOut != null)
                StopCoroutine(overPanelOut);

            overPanelIn = FadeManager.FadeOnly(overPanel, true, SaveManager.instance.speed * coef);
            yield return StartCoroutine(overPanelIn);
        }

        yield return null;
    }

    public IEnumerator CloseOverPanel(float coef = 1)
    {
        if (overPanelIn != null)
            StopCoroutine(overPanelIn);

        overPanelOut = FadeManager.FadeOnly(overPanel, false, SaveManager.instance.speed * coef);
        yield return StartCoroutine(overPanelOut);
    }

    public void resetCassettePosition(GameObject cassette)
    {
        //cassette.GetComponent<CanvasGroup>().alpha = 0f;
        GameObject circle1 = cassette.transform.Find("Circle1").gameObject;
        GameObject circle2 = cassette.transform.Find("Circle2").gameObject;
        circle1.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
        circle2.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
    }

    public void ResetVisuals()
    {
        SavedPanel.GetComponent<CanvasGroup>().alpha = 0f;
        UnSavedPanel.GetComponent<CanvasGroup>().alpha = 0f;
        MainMenuPanel.GetComponent<CanvasGroup>().alpha = 0f;
        GradLeft.GetComponent<CanvasGroup>().alpha = 0f;
        GradRight.GetComponent<CanvasGroup>().alpha = 0f;
        IconLeft.GetComponent<CanvasGroup>().alpha = 0f;
        IconRight.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
