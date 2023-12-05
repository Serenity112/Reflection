using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsOption : MonoBehaviour, ISettingsOptions, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private SettingsList setting;

    [SerializeField]
    private SettingsOptions option;

    private GameObject spacing;

    private Text text;

    private IEnumerator spacingOn;
    private IEnumerator spacingOff;

    private IEnumerator colorsOn;
    private IEnumerator colorsOff;

    private bool pointer_down = false;
    private bool enter = false;

    void Awake()
    {
        spacing = transform.GetChild(0).gameObject;
        text = gameObject.GetComponent<Text>();

        SettingsConfig.subscribeOption(setting, this);
    }

    public void OnClick()
    {
        SettingsConfig.CheckLinkedOptions(setting, option);
        SettingsConfig.SaveOptionToFile(setting, option);
        SettingsConfig.UpdateGroupVisuals(setting);
    }

    private void OnMouseEnter()
    {
        if (!SettingsConfig.isOptionEnabled(setting, option))
        {
            StartCoroutine(ToWhite());
        }
    }

    private void OnMouseExit()
    {
        if (!SettingsConfig.isOptionEnabled(setting, option))
        {
            StartCoroutine(SpacingOff());
            if (!pointer_down)
            {
                StartCoroutine(ToGray());
            }
        }
    }

    // Подчёркивание 
    private IEnumerator SpacingOn()
    {
        if (spacingOff != null)
            StopCoroutine(spacingOff);
        spacingOn = FadeManager.FadeObject(spacing, true, SettingsConfig.spacingSpeed);
        yield return StartCoroutine(spacingOn);
    }

    private IEnumerator SpacingOff()
    {
        if (spacingOn != null)
            StopCoroutine(spacingOn);
        spacingOff = FadeManager.FadeObject(spacing, false, SettingsConfig.spacingSpeed);
        yield return StartCoroutine(spacingOff);
    }

    // Белый текст
    private IEnumerator ToWhite()
    {
        if (colorsOff != null)
            StopCoroutine(colorsOff);
        colorsOn = FadeManager.FadeTextToColor(text, new Color(1f, 1f, 1f, 1.05f), SettingsConfig.spacingSpeed);
        yield return StartCoroutine(colorsOn);
    }

    private IEnumerator ToGray()
    {
        if (colorsOn != null)
            StopCoroutine(colorsOn);
        colorsOff = FadeManager.FadeTextToColor(text, new Color(0.6f, 0.6f, 0.6f, 1.05f), SettingsConfig.spacingSpeed);
        yield return StartCoroutine(colorsOff);
    }

    public void UpdateVisuals()
    {
        StopAllCoroutines();

        List<IEnumerator> list = new List<IEnumerator>();

        if (SettingsConfig.isOptionEnabled(setting, option))
        {
            list.Add(SpacingOn());
            list.Add(ToWhite());

        }
        else
        {
            list.Add(SpacingOff());
            list.Add(ToGray());
        }

        StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(list));
    }

    public void InitialUpdateVisuals()
    {
        UpdateVisuals();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointer_down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointer_down = false;
        if (!enter && !SettingsConfig.isOptionEnabled(setting, option))
        {
            StartCoroutine(ToGray());
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        enter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enter = false;
    }
}
