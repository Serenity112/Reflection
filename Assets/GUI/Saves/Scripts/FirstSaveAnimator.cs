using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class FirstSaveAnimator : MonoBehaviour
{   
    [SerializeField] private GameObject CassetteImg;
    [SerializeField] private Button ButtonUnsaved;   
    [SerializeField] private Animator cassetteAnimator;

    private int saveNum;
    private GameObject screenshot;
    private GameObject SavedPanel;
    private GameObject UnSavedPanel;

    private IEnumerator CassetteFadeIn;
    private IEnumerator CassetteFadeOut;

    private SaveFileFields saveFileFields;

    private void Start()
    {          
        saveFileFields = GetComponent<SaveFileFields>();
        saveNum = saveFileFields.saveNum;
        screenshot = saveFileFields.screenshot;
        SavedPanel = saveFileFields.SavedPanel;
        UnSavedPanel = saveFileFields.UnSavedPanel;

        ButtonUnsaved.interactable = true;
    }

    public void AppearCassette()
    {
        if (ButtonUnsaved.interactable && !PanelsManager.confirmPanelActive)
        {
            if(CassetteFadeOut != null)
                StopCoroutine(CassetteFadeOut);

            CassetteFadeIn = FadeManager.FadeObject(CassetteImg, true, SaveManager.instance.optionsGradientSpeed);
            StartCoroutine(CassetteFadeIn);
        }            
    }

    public void DisappearCassette()
    {
        if (ButtonUnsaved.interactable && !PanelsManager.confirmPanelActive) 
        {
            if (CassetteFadeIn != null)
                StopCoroutine(CassetteFadeIn);

            CassetteFadeOut = FadeManager.FadeObject(CassetteImg, false, SaveManager.instance.optionsGradientSpeed);
            StartCoroutine(CassetteFadeOut);
        }
        
    }

    public void FirstSaveIconClick()
    {
        if (!StaticVariables.UIsystemDown)
        {
            StartCoroutine(IFirstSaveIconClick());
        }          
    }

    IEnumerator IFirstSaveIconClick()
    {
        StaticVariables.UIsystemDown = true;
        ButtonUnsaved.interactable = false;
        saveFileFields.AllowOverPanel = false;
        saveFileFields.AllowSaveLoad = false;
        saveFileFields.SavedPanel.SetActive(true);

        Vector3 currScale = CassetteImg.transform.localScale;
        cassetteAnimator.Play("CasseteAnim");
        yield return StartCoroutine(ExpandManager.ExpandObject(CassetteImg, 0.9f, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(CassetteImg, currScale, 0.05f));
       

        UserData.instance.SavePlayer(saveNum);
        StartCoroutine(SaveManager.instance.SetScreenshot(saveNum, screenshot));
        saveFileFields.AllowOverPanel = true;

        if (saveFileFields.exitLeft && saveFileFields.exitRight)
            saveFileFields.OpenOverPanel();

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(FadeManager.FadeObject(screenshot, true, SaveManager.instance.optionsGradientSpeed));
        StartCoroutine(FadeManager.FadeObject(SavedPanel, true, SaveManager.instance.optionsGradientSpeed));
        yield return StartCoroutine(FadeManager.FadeObject(UnSavedPanel, false, SaveManager.instance.optionsGradientSpeed));

        saveFileFields.AllowSaveLoad = true;
        saveFileFields.resetCassettePosition(CassetteImg);
        ButtonUnsaved.interactable = true;
    }
}