using System.Collections;
using UnityEngine;

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

    public bool AllowSaveLoad;
    public bool AllowOverPanel;

    public bool exitLeft;
    public bool exitRight;

    private IEnumerator overPanelIn;
    private IEnumerator overPanelOut;

    void Start()
    {
        exitLeft = true;
        exitRight = true;
    }
    public void OpenOverPanel()
    {
        if (!PanelsManager.confirmPanelActive)
        {
            overPanelIn = FadeManager.FadeObject(overPanel, true, SaveManager.instance.optionsGradientSpeed);
            StartCoroutine(overPanelIn);

            if (overPanelOut != null)
                StopCoroutine(overPanelOut);

        }
    }

    public void CloseOverPanel()
    {
        overPanelOut = FadeManager.FadeObject(overPanel, false, SaveManager.instance.optionsGradientSpeed);
        StartCoroutine(overPanelOut);

        if (overPanelIn != null)
            StopCoroutine(overPanelIn);
    }

    public void resetCassettePosition(GameObject cassette)
    {
        cassette.GetComponent<CanvasGroup>().alpha = 0f;
        GameObject circle1 = cassette.transform.Find("Circle1").gameObject;
        GameObject circle2 = cassette.transform.Find("Circle2").gameObject;
        circle1.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
        circle2.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
    }
}