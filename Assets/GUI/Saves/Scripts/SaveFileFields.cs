using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileFields : MonoBehaviour
{
    public int saveNum;

    public GameObject _SaveChoiseAnimatorObject;
    public GameObject _FirstSaveAnimatorObject;
    public GameObject _MainMenuLoadObject;

    public SaveChoiseAnimator _SaveChoiseAnimator;
    public SaveChoiseIconAnimator _SaveChoiseIconAnimator;
    public FirstSaveAnimator _FirstSaveAnimator;
    public MainMenuLoad _MainMenuLoad;

    public GameObject Screenshot;
    public GameObject OverSreenshot;
    public GameObject OverPanel;
    public GameObject NoImage;
    public SaveDateTime Datetime;
    public GameObject VisualFile;

    private IEnumerator overPanelIn;
    private IEnumerator overPanelOut;

    private IEnumerator vfIn;
    private IEnumerator vfOut;

    private float speed = 4f;

    void Awake()
    {
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

    public void OpenOverPanelInstant()
    {
        FadeManager.FadeOnly(OverPanel, true);
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
}
