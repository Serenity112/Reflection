using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uistopper : MonoBehaviour
{
    public GameObject panel;

    public void fadeout() { 
        StartCoroutine(FadeManager.FadeObject(panel, false, 0.3f));
    }

    public void fadein()
    {
        StartCoroutine(FadeManager.FadeObject(panel, true, 0.3f));
    }
}
