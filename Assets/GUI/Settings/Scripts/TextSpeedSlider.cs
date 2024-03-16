using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TextSpeedSlider : MonoBehaviour,  ISettingsOptions, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private SettingsList setting;

    [SerializeField]
    private SettingsOptions option;

    [SerializeField]
    private GameObject TextObj;

    [SerializeField]
    private GameObject InfinityObj;

    private Text text;

    private Slider slider;

    private bool pointer_down = false;
    private bool enter = false;

    private IEnumerator speedIn;
    private IEnumerator speedOut;

    private IEnumerator infinityIn;
    private IEnumerator infinityOut;

    private void Awake()
    {
        SettingsConfig.subscribeOption(setting, GetComponent<TextSpeedSlider>());

        slider = GetComponent<Slider>();
        text = TextObj.GetComponent<Text>();
    }

    public void OnValueChanged()
    {
        if (slider.value == slider.maxValue)
        {
            text.text = "";
            HideSpeed();
            ShowInfinity();
        }
        else
        {
            text.text = Convert.ToInt32(slider.value).ToString();
            ShowSpeed();
            HideInfinity();
        }

        SettingsConfig.SaveOption(setting, option, slider.value);
        SettingsConfig.ApplySetting(setting, option, slider.value);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointer_down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointer_down = false;

        if (!enter)
        {
            HideSpeed();
            HideInfinity();
        }
    }

    public void OnMouseEnter()
    {
        enter = true;

        if (!pointer_down)
        {
            if (SettingsConfig.chosenOptions[setting].data == slider.maxValue)
            {
                ShowInfinity();
            }
            else
            {
                ShowSpeed();
            }
        }
    }

    public void OnMouseExit()
    {
        enter = false;

        if (!pointer_down)
        {
            HideInfinity();
            HideSpeed();
        }
    }

    private void ShowSpeed()
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

    private void ShowInfinity()
    {
        if (infinityOut != null)
            StopCoroutine(infinityOut);

        infinityIn = FadeManager.FadeOnly(InfinityObj, true, SettingsConfig.sliderGuiSpeed);
        StartCoroutine(infinityIn);
    }

    private void HideInfinity()
    {
        if (infinityIn != null)
            StopCoroutine(infinityIn);

        infinityOut = FadeManager.FadeOnly(InfinityObj, false, SettingsConfig.sliderGuiSpeed);
        StartCoroutine(infinityOut);
    }

    public void UpdateVisuals()
    {
        float speed = SettingsConfig.chosenOptions[setting].data;
        slider.value = speed;
        text.text = Convert.ToInt32(speed).ToString();

        StopAllCoroutines();

        TextObj.GetComponent<CanvasGroup>().alpha = 0f;
        InfinityObj.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void InitialUpdateVisuals()
    {
        enter = false;
        pointer_down = false;

        UpdateVisuals();
    }
}
