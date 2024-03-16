using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileFields : MonoBehaviour
{
    public int saveNum;

    public GameObject _SaveChoiseAnimatorObject;
    public GameObject _FirstSaveAnimatorObject;
    public GameObject _MainMenuLoadObject;

    [HideInInspector] public SaveChoiseAnimator _SaveChoiseAnimator;
    [HideInInspector] public FirstSaveAnimator _FirstSaveAnimator;
    [HideInInspector] public MainMenuLoad _MainMenuLoad;

    public GameObject Screenshot;
    public GameObject OverSreenshot;
    public GameObject OverPanel;
    public GameObject NoImage;
    public SaveDateTime Datetime;
    private GameObject VisualFile;


    private IEnumerator overPanelIn;
    private IEnumerator overPanelOut;

    private IEnumerator vfIn;
    private IEnumerator vfOut;

    private float speed = 4f;

    void Awake()
    {
        _SaveChoiseAnimator = GetComponent<SaveChoiseAnimator>();
        _FirstSaveAnimator = GetComponent<FirstSaveAnimator>();
        _MainMenuLoad = GetComponent<MainMenuLoad>();
        VisualFile = transform.GetChild(1).gameObject;
    }

    public IEnumerator OpenOverPanel()
    {
        if (overPanelOut != null)
            StopCoroutine(overPanelOut);

        overPanelIn = FadeManager.FadeOnly(OverPanel, true, speed);
        yield return StartCoroutine(overPanelIn);

    }

    public IEnumerator CloseOverPanel()
    {
        if (overPanelIn != null)
            StopCoroutine(overPanelIn);

        overPanelOut = FadeManager.FadeOnly(OverPanel, false, speed);
        yield return StartCoroutine(overPanelOut);
    }

    public void CloseOverPanelInstant()
    {
        FadeManager.FadeOnly(OverPanel, false);
    }

    public IEnumerator VF_Show()
    {
        if (vfOut != null)
            StopCoroutine(vfOut);

        vfIn = FadeManager.FadeOnly(VisualFile, true, speed);
        yield return StartCoroutine(vfIn);

    }

    public IEnumerator VF_Hide()
    {
        if (vfIn != null)
            StopCoroutine(vfIn);

        vfOut = FadeManager.FadeOnly(VisualFile, false, speed);
        yield return StartCoroutine(vfOut);
    }

    public void resetCassettePosition(GameObject cassette)
    {
        //cassette.GetComponent<CanvasGroup>().alpha = 0f;
        /*GameObject circle1 = cassette.transform.Find("Circle1").gameObject;
        GameObject circle2 = cassette.transform.Find("Circle2").gameObject;
        circle1.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
        circle2.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);*/
    }
}
