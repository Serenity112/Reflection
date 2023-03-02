using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangePerspective : MonoBehaviour
{
    GameObject SayDialogCanvas;
    GameObject GuiPanel;
    GameObject DownPanel;
    GameObject TopPanel;

    void Start()
    {
        SayDialogCanvas = GameObject.Find("SayDialogCanvas");
        GuiPanel = GameObject.Find("GuiPanel");
        DownPanel = GameObject.Find("DownPanel");
        TopPanel = GameObject.Find("TopPanel");
    }

    public void SetBlackLines()
    {
        CanvasScaler c = SayDialogCanvas.GetComponent<CanvasScaler>();
        c.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand; //Редактирование фона

        RectTransform rt = GuiPanel.GetComponent<RectTransform>(); //
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(1920, 1080);

        TopPanel.GetComponent<CanvasGroup>().alpha = 1;
        DownPanel.GetComponent<CanvasGroup>().alpha = 1;
    }


    public void SetFullScreen()
    {
        CanvasScaler c = SayDialogCanvas.GetComponent<CanvasScaler>();
        c.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;

        RectTransform rt = GuiPanel.GetComponent<RectTransform>(); //
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.sizeDelta = new Vector2(0, 0);

        TopPanel.GetComponent<CanvasGroup>().alpha = 0;
        DownPanel.GetComponent<CanvasGroup>().alpha = 0;
    }
}