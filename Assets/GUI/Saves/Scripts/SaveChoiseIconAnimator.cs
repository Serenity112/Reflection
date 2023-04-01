using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveChoiseIconAnimator : MonoBehaviour
{
    private GameObject GradLeft;
    private GameObject GradRight;
    private GameObject IconLeft;
    private GameObject IconRight;

    private SaveFileFields saveFileFields;
    private void Start()
    {
        saveFileFields = GetComponent<SaveFileFields>();

        GradLeft = saveFileFields.GradLeft;
        GradRight = saveFileFields.GradRight;
        IconLeft = saveFileFields.IconLeft;
        IconRight = saveFileFields.IconRight;
    }
    public void appearSide(Side side)
    {
        StopAllCoroutines();

        switch (side)
        {
            case Side.Left:
                chooseLeft();
                break;
            case Side.Right:
                chooseRight();
                break;
        }
    }

    public void removeSide(Side side)
    {
        StopAllCoroutines();

        switch (side)
        {
            case Side.Left:
                cancelLeft();
                break;
            case Side.Right:
                cancelRight();
                break;
        }
    }

    public void chooseLeft()
    {
        saveFileFields.exitLeft = false;

        if (saveFileFields.AllowSaveLoad)
        {
            StartCoroutine(FadeManager.FadeObject(GradRight, true, SaveManager.instance.optionsGradientSpeed));
            StartCoroutine(FadeManager.FadeObject(IconLeft, true, SaveManager.instance.optionsGradientSpeed));

            StartCoroutine(FadeManager.FadeObject(GradLeft, false, SaveManager.instance.optionsGradientSpeed));
            StartCoroutine(FadeManager.FadeObject(IconRight, false, SaveManager.instance.optionsGradientSpeed));
        }

        if (saveFileFields.AllowOverPanel)
        {
            saveFileFields.CloseOverPanel();
        }
    }

    public void chooseRight()
    {
        saveFileFields.exitRight = false;

        if (saveFileFields.AllowSaveLoad)
        {
            StartCoroutine(FadeManager.FadeObject(GradLeft, true, SaveManager.instance.optionsGradientSpeed));
            StartCoroutine(FadeManager.FadeObject(IconRight, true, SaveManager.instance.optionsGradientSpeed));

            StartCoroutine(FadeManager.FadeObject(GradRight, false, SaveManager.instance.optionsGradientSpeed));
            StartCoroutine(FadeManager.FadeObject(IconLeft, false, SaveManager.instance.optionsGradientSpeed));
        }

        if (saveFileFields.AllowOverPanel)
        {
            saveFileFields.CloseOverPanel();
        }
    }

    public void cancelLeft()
    {
        saveFileFields.exitLeft = true;

        if (saveFileFields.AllowSaveLoad)
        {
            StartCoroutine(FadeManager.FadeObject(GradRight, false, SaveManager.instance.optionsGradientSpeed));
            StartCoroutine(FadeManager.FadeObject(IconLeft, false, SaveManager.instance.optionsGradientSpeed));

            if (saveFileFields.exitRight)
            {

                StartCoroutine(FadeManager.FadeObject(IconRight, false, SaveManager.instance.optionsGradientSpeed));
                StartCoroutine(FadeManager.FadeObject(GradLeft, false, SaveManager.instance.optionsGradientSpeed));
            }
        }

        if (saveFileFields.AllowOverPanel)
        {
            if (saveFileFields.exitRight)
            {
                saveFileFields.OpenOverPanel();
            }
        }
    }

    public void cancelRight()
    {
        saveFileFields.exitRight = true;

        if (saveFileFields.AllowSaveLoad)
        {
            StartCoroutine(FadeManager.FadeObject(IconRight, false, SaveManager.instance.optionsGradientSpeed));
            StartCoroutine(FadeManager.FadeObject(GradLeft, false, SaveManager.instance.optionsGradientSpeed));

            if (saveFileFields.exitLeft)
            {
                StartCoroutine(FadeManager.FadeObject(GradRight, false, SaveManager.instance.optionsGradientSpeed));
                StartCoroutine(FadeManager.FadeObject(IconLeft, false, SaveManager.instance.optionsGradientSpeed));
            }
        }

        if (saveFileFields.AllowOverPanel)
        {
            if (saveFileFields.exitLeft)
            {
                saveFileFields.OpenOverPanel();
            }
        }
    }
}