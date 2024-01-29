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

    private IEnumerator overPanelIn;
    private IEnumerator overPanelOut;

    private float op_alpha = 0.4f;
    private float speed = 4f;

    void Awake()
    {
        _SaveChoiseAnimator = GetComponent<SaveChoiseAnimator>();
        _FirstSaveAnimator = GetComponent<FirstSaveAnimator>();
        _MainMenuLoad = GetComponent<MainMenuLoad>();
    }

    public IEnumerator OpenOverPanel(float coef = 1)
    {
        if (!SaveManagerStatic.OverlayPanelActive)
        {
           
        }

        if (overPanelOut != null)
            StopCoroutine(overPanelOut);

        overPanelIn = FadeManager.FadeOnly(OverPanel, true, speed * coef);
        yield return StartCoroutine(overPanelIn);
    }

    public IEnumerator CloseOverPanel(float coef = 1)
    {
        if (overPanelIn != null)
            StopCoroutine(overPanelIn);

        overPanelOut = FadeManager.FadeOnly(OverPanel, false, speed * coef);
        yield return StartCoroutine(overPanelOut);
    }

    public void resetCassettePosition(GameObject cassette)
    {
        //cassette.GetComponent<CanvasGroup>().alpha = 0f;
        /*GameObject circle1 = cassette.transform.Find("Circle1").gameObject;
        GameObject circle2 = cassette.transform.Find("Circle2").gameObject;
        circle1.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);
        circle2.GetComponent<RectTransform>().rotation = new Quaternion(0, 0, 0, 0);*/
    }

    public void ResetAll()
    {
        _SaveChoiseAnimator.ResetPanel();
        _FirstSaveAnimator.ResetPanel();
        _MainMenuLoad.ResetPanel();


        /*_SaveChoiseAnimator.GetComponent<CanvasGroup>().alpha = 0f;
        _FirstSaveAnimator.GetComponent<CanvasGroup>().alpha = 0f;
        _MainMenuLoad.GetComponent<CanvasGroup>().alpha = 0f;*/
        /* GradLeft.GetComponent<CanvasGroup>().alpha = 0f;
         GradRight.GetComponent<CanvasGroup>().alpha = 0f;
         IconLeft.GetComponent<CanvasGroup>().alpha = 0f;
         IconRight.GetComponent<CanvasGroup>().alpha = 0f;*/
    }
}
