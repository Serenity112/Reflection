using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CheckBox : MonoBehaviour, ISettingsOptions, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private SettingsList setting;

    [SerializeField]
    private SettingsOptions option;

    private GameObject CircleIn;

    private IEnumerator ICircleOn;
    private IEnumerator ICircleOff;

    private bool pointer_down = false;
    private bool enter = false;

    private void Awake()
    {
        CircleIn = transform.GetChild(0).gameObject;

        SettingsConfig.subscribeOption(setting, this);
    }

    public void OnClick() => StartCoroutine(IOnClick());

    private IEnumerator IOnClick()
    {
        float newData = (SettingsConfig.chosenOptions[setting].data + 1) % 2;

        SettingsConfig.CheckLinkedOptions(setting, option);
        SettingsConfig.SaveOption(setting, option, newData);
        SettingsConfig.ApplySetting(setting, option, newData);

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
        GetComponent<Button>().interactable = true;
        float data = SettingsConfig.chosenOptions[setting].data;

        // 1 - true
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
        enter = false;
        pointer_down = false;

        GetComponent<Button>().interactable = true;
        UpdateVisuals();
    }

    public void OnMouseEnter()
    {
        enter = true;

        float data = SettingsConfig.chosenOptions[setting].data;

        if (data == 0)
        {
            CircleOn();
        }
    }

    public void OnMouseExit()
    {
        enter = false;

        float data = SettingsConfig.chosenOptions[setting].data;

        if (data == 0 && !pointer_down)
        {
            CircleOff();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointer_down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float data = SettingsConfig.chosenOptions[setting].data;
        pointer_down = false;
        if (!enter && (data == 0))
        {
            CircleOff();
        }
    }
}
