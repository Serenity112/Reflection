using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class VolumeSlider : MonoBehaviour, IEndDragHandler, IBeginDragHandler, ISettingsOption
{
    [SerializeField]
    private Settings setting;

    [SerializeField]
    private SettingsOptions option;

    [SerializeField]
    private GameObject TextObj;

    [SerializeField]
    private GameObject MutedImage;

    private Text text;

    private Slider slider;

    private Animator mutedAnimator;

    private bool dragging = false;
    private bool enter = false;
    private bool ismuted = false;

    private IEnumerator percentIn;
    private IEnumerator percentOut;

    private IEnumerator mutedimgIn;
    private IEnumerator mutedimgOut;

    private void Awake()
    {
        SettingsConfig.subscribeOption(GetComponent<VolumeSlider>());

        slider = GetComponent<Slider>();
        text = TextObj.GetComponent<Text>();
        mutedAnimator = MutedImage.GetComponent<Animator>();
    }

    public void OnValueChanged()
    {
        switch (slider.value)
        {
            case 0:
                Mute();
                break;
            default:
                if (ismuted)
                    UnMute();
                text.text = Convert.ToInt32(slider.value).ToString() + "%";
                break;
        }

        SettingsConfig.SaveOption(setting, option, slider.value);

        SettingsConfig.ApplySetting(setting, option, slider.value);
    }

    void Mute()
    {
        ismuted = true;

        ShowMutedImg();
        HideText();

        mutedAnimator.Play("Mute");
    }

    void UnMute()
    {
        ismuted = false;

        HideMutedImg();
        ShowText();

        mutedAnimator.Play("UnMute");
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
            HideMutedImg();
            HideText();
        }
    }

    public void OnMouseEnter()
    {
        enter = true;

        if (!dragging)
        {
            if (SettingsConfig.chosenOptions[setting].data == 0)
            {
                ShowMutedImg();
            }
            else
            {
                ShowText();
            }
        }
    }

    public void OnMouseExit()
    {
        enter = false;

        if (!dragging)
        {
            if (SettingsConfig.chosenOptions[setting].data == 0)
            {
                HideMutedImg();
            }
            else
            {
                HideText();
            }
        }
    }

    private void ShowMutedImg()
    {
        if (mutedimgOut != null)
            StopCoroutine(mutedimgOut);

        mutedimgIn = FadeManager.FadeOnly(MutedImage, true, SettingsConfig.sliderGuiSpeed);
        StartCoroutine(mutedimgIn);
    }

    private void HideMutedImg()
    {
        if (mutedimgIn != null)
            StopCoroutine(mutedimgIn);

        mutedimgOut = FadeManager.FadeOnly(MutedImage, false, SettingsConfig.sliderGuiSpeed);
        StartCoroutine(mutedimgOut);
    }

    private void ShowText()
    {
        if (percentOut != null)
            StopCoroutine(percentOut);

        percentIn = FadeManager.FadeOnly(TextObj, true, SettingsConfig.sliderGuiSpeed);
        StartCoroutine(percentIn);
    }

    private void HideText()
    {
        if (percentIn != null)
            StopCoroutine(percentIn);

        percentOut = FadeManager.FadeOnly(TextObj, false, SettingsConfig.sliderGuiSpeed);
        StartCoroutine(percentOut);
    }

    public void UpdateVisuals()
    {
        float vol = SettingsConfig.chosenOptions[setting].data;

        slider.value = vol;

        if (vol == 0)
        {
            mutedAnimator.Play("Mute");
            ismuted = true;
        }
        else
        {
            text.text = Convert.ToInt32(vol).ToString() + "%";
            mutedAnimator.Play("UnMute");
        }

        MutedImage.GetComponent<CanvasGroup>().alpha = 0f;
        TextObj.GetComponent<CanvasGroup>().alpha = 0f;
    }
}