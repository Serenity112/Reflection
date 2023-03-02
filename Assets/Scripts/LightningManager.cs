using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightningManager : MonoBehaviour
{
    //ёзаем чЄрный экран и менем обратно, чтобы не создавать новый объект
    public GameObject WhitePanel;

    public GameObject BG1;
    public GameObject Rain;
    public GameObject DialogPanel;

    public void SummonScreenLightning(float disappearSpeed)
    {
        StartCoroutine(ISummonScreenLightning(disappearSpeed));
    }

    public IEnumerator ISummonScreenLightning(float disappearSpeed)
    {
        //–езка€ вспышка по клику
        WhitePanel.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
        WhitePanel.GetComponent<CanvasGroup>().alpha = 1;
        WhitePanel.SetActive(true);

       // DialogPanel.SetActive(false);
        DialogPanel.GetComponent<CanvasGroup>().alpha = 0;

        
        Rain.SetActive(true);
        BG1.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        //StartCoroutine(IFadeObject(BG1, false, disappearSpeed));
        StartCoroutine(IFadeObject(DialogPanel, true, disappearSpeed));
        yield return IFadeObject(WhitePanel, false, disappearSpeed);


        

        WhitePanel.GetComponent<Image>().color = new Color32(255, 255, 225, 0);
    }

    private IEnumerator IFadeObject(GameObject obj, bool fadein, float speed)
    {
        obj.SetActive(true);

        if (fadein)
        {
            obj.GetComponent<CanvasGroup>().alpha = 0;
            int color = 1;
            for (float i = 0; i <= color + 0.05; i += speed * Time.deltaTime)
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
            // Debug.Log("Screen faded in");
        }
        else
        {
            obj.GetComponent<CanvasGroup>().alpha = 1;
            int color = 0;
            for (float i = 1; i >= color - 0.05; i -= speed * Time.deltaTime)
            {
                obj.GetComponent<CanvasGroup>().alpha = i;
                yield return null;
            }
            obj.SetActive(false);
        }

    }
}
