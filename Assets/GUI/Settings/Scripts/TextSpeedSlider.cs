using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class TextSpeedSlider : MonoBehaviour, IEndDragHandler, IBeginDragHandler, ISettingsOption
{
    [SerializeField]
    private Settings setting;

    [SerializeField]
    private SettingsOptions option;

    [SerializeField]
    private GameObject TextObj;

    private Text text;

    private Slider slider;

    private bool dragging = false;
    private bool enter = false;

    private IEnumerator speedIn;
    private IEnumerator speedOut;

    private void Awake()
    {
        SettingsConfig.subscribeOption(GetComponent<TextSpeedSlider>());

        slider = GetComponent<Slider>();
        text = TextObj.GetComponent<Text>();
    }

    public void OnValueChanged()
    {
        text.text = Convert.ToInt32(slider.value).ToString();

        SettingsConfig.SaveOption(setting, option, slider.value);

        SettingsConfig.ApplySetting(setting, option, slider.value);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;

        if (!enter)
        {
            HideSpeed();
        }
    }

    public void OnMouseEnter()
    {
        enter = true;

        if (!dragging)
        {
            ShowSped();
        }
    }

    public void OnMouseExit()
    {
        enter = false;

        if (!dragging)
        {
            HideSpeed();
        }
    }

    private void ShowSped()
    {
        if (speedOut != null)
            StopCoroutine(speedOut);

        speedIn = FadeManager.FadeOnly(TextObj, true, SettingsConfig.sliderGuiSpeed);
        StartCoroutine(speedIn);
    }

    private void HideSpeed()
    {
        if (speedIn != null)
            StopCoroutine(speedIn);

        speedOut = FadeManager.FadeOnly(TextObj, false, SettingsConfig.sliderGuiSpeed);
        StartCoroutine(speedOut);
    }

    public void UpdateVisuals()
    {
        float speed = SettingsConfig.chosenOptions[setting].data;

        slider.value = speed;

        text.text = Convert.ToInt32(speed).ToString();

        TextObj.GetComponent<CanvasGroup>().alpha = 0f;
    }
}