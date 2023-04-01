using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanel : MonoBehaviour
{
    public static ConfirmationPanel instance = null;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }

    public GameObject Panel;
    public GameObject YesButton;
    public GameObject NoButton;
    public GameObject Title;
    public float speed;

    IEnumerator IYes;
    IEnumerator INo;

    public static IEnumerator CreatePanel(string title, IEnumerator YesAction, IEnumerator NoAction)
    {
        instance.IYes = YesAction;
        instance.INo = NoAction;
        instance.Title.GetComponent<Text>().text = title;

        yield return instance.StartCoroutine(FadeManager.FadeObject(instance.Panel, true, instance.speed));
    }

    public static IEnumerator ClosePanel()
    {
        PanelsManager.confirmPanelActive = false;
        yield return instance.StartCoroutine(FadeManager.FadeObject(instance.Panel, false, instance.speed));
    }

    public void ChooseYes()
    {
        StartCoroutine(IYes);
    }

    public void ChooseNo()
    {
        StartCoroutine(INo);
    }

}
