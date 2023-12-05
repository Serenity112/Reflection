using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour, ISettingsOptions
{
    [SerializeField]
    private SettingsList setting;

    [SerializeField]
    private SettingsOptions option;

    private GameObject CircleIn;
    //private GameObject CircleOut;

    private IEnumerator ICircleOn;
    private IEnumerator ICircleOff;

    private void Awake()
    {
        CircleIn = transform.GetChild(0).gameObject;
        //CircleOut = transform.GetChild(1).gameObject;

        SettingsConfig.subscribeOption(setting, this);
    }

    public void OnClick() => StartCoroutine(IOnClick());

    private IEnumerator IOnClick()
    {
        float newData = (SettingsConfig.chosenOptions[setting].data + 1) % 2;
        SettingsConfig.ApplySetting(setting, option, newData);
        SettingsConfig.SaveOptionToFile(setting, option, newData);

        Vector3 origScale = CircleIn.transform.localScale;
        Vector3 shrinkedScale = origScale * 0.85f;

        GetComponent<Button>().interactable = false;


        yield return StartCoroutine(ExpandManager.ExpandObject(CircleIn, shrinkedScale, 0.05f));
        yield return StartCoroutine(ExpandManager.ExpandObject(CircleIn, origScale, 0.05f));
        if (newData == 0)
        {
            CircleOff();
        }
        else
        {
            CircleOn();
        }

        GetComponent<Button>().interactable = true;
    }

    private void OnMouseEnter()
    {
        float data = SettingsConfig.chosenOptions[setting].data;

        if (data == 0)
        {
            CircleOn();
        }
    }
    private void OnMouseExit()
    {
        float data = SettingsConfig.chosenOptions[setting].data;

        if (data == 0)
        {
            CircleOff();
        }
    }

    private void CircleOn()
    {
        if (ICircleOff != null)
            StopCoroutine(ICircleOff);
        ICircleOn = FadeManager.FadeObject(CircleIn, true, SettingsConfig.spacingSpeed);
        StartCoroutine(ICircleOn);
    }

    private void CircleOff()
    {
        if (ICircleOn != null)
            StopCoroutine(ICircleOn);
        ICircleOff = FadeManager.FadeObject(CircleIn, false, SettingsConfig.spacingSpeed);
        StartCoroutine(ICircleOff);
    }

    public void UpdateVisuals()
    {
        float data = SettingsConfig.chosenOptions[setting].data;

        if (data == 1)
        {
            CircleOn();
        }
        else
        {
            CircleOff();
        }
    }

    public void InitialUpdateVisuals()
    {
        float data = SettingsConfig.chosenOptions[setting].data;

        if (data == 1)
        {
            FadeManager.FadeObject(CircleIn, true);
        }
        else
        {
            FadeManager.FadeObject(CircleIn, false);
        }
    }
}
