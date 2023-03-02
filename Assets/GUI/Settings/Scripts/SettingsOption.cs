using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SettingsOption : MonoBehaviour, ISettingsOption
{
    [SerializeField]
    private Settings setting;

    [SerializeField]
    private SettingsOptions option;

    private GameObject spacing;

    private Text text;

    IEnumerator spacingOn;
    IEnumerator spacingOff;

    IEnumerator colorsOn;
    IEnumerator colorsOff;

    void Awake()
    {
        spacing = transform.GetChild(0).gameObject;
        text = gameObject.GetComponent<Text>();

        SettingsConfig.subscribeOption(GetComponent<SettingsOption>());
    }

    public void OnClick()
    {
        SettingsConfig.ApplySetting(setting, option);

        SettingsConfig.SaveOption(setting, option);

        SettingsConfig.UpdateAllVisuals();
    }

    void OnMouseEnter()
    {
        ToWhite();
    }
    void OnMouseExit()
    {
        if (!SettingsConfig.isOptionEnabled(setting, option))
        {
            SpacingOff();
            ToGray();
        }
    }
    public void SpacingOn()
    {
        if (spacingOff != null)
            StopCoroutine(spacingOff);
        spacingOn = FadeManager.FadeObject(spacing, true, SettingsConfig.spacingSpeed);
        StartCoroutine(spacingOn);
    }
    public void SpacingOff()
    {
        if (spacingOn != null)
            StopCoroutine(spacingOn);
        spacingOff = FadeManager.FadeObject(spacing, false, SettingsConfig.spacingSpeed);
        StartCoroutine(spacingOff);
    }
    public void ToWhite()
    {
        if (colorsOff != null)
            StopCoroutine(colorsOff);
        colorsOn = FadeManager.FadeTextToColor(text, new Color(1f, 1f, 1f, 1.05f), SettingsConfig.spacingSpeed);
        StartCoroutine(colorsOn);
    }
    public void ToGray()
    {
        if (colorsOn != null)
            StopCoroutine(colorsOn);
        colorsOff = FadeManager.FadeTextToColor(text, new Color(0.6f, 0.6f, 0.6f, 1.05f), SettingsConfig.spacingSpeed);
        StartCoroutine(colorsOff);
    }

    public void UpdateVisuals()
    {
        if (SettingsConfig.isOptionEnabled(setting, option))
        {
            SpacingOn();
            ToWhite();
        }
        else
        {
            SpacingOff();
            ToGray();
        }
    }
}