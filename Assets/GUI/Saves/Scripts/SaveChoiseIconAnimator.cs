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

    private void Awake()
    {
        saveFileFields = GetComponent<SaveFileFields>();

        GradLeft = saveFileFields.GradLeft;
        GradRight = saveFileFields.GradRight;
        IconLeft = saveFileFields.IconLeft;
        IconRight = saveFileFields.IconRight;
    }
    public void AppearSide(Side side)
    {
        StartCoroutine(saveFileFields.CloseOverPanel());
        switch (side)
        {
            case Side.Left:
                AppearLeft();
                break;
            case Side.Right:
                AppearRight();
                break;
        }
    }

    public void RemoveSide(Side side)
    {
        switch (side)
        {
            case Side.Left:
                if (saveFileFields.exitRight)
                {
                    StartCoroutine(saveFileFields.OpenOverPanel());
                }
                ExitLeft();
                break;
            case Side.Right:
                if (saveFileFields.exitLeft)
                {
                    StartCoroutine(saveFileFields.OpenOverPanel());
                }
                ExitRight();
                break;
        }
    }

    public void AppearLeft()
    {
        StopAllCoroutines();
        saveFileFields.exitLeft = false;

        StartCoroutine(FadeManager.FadeOnly(GradRight, true, SaveManager.instance.speed));
        StartCoroutine(FadeManager.FadeOnly(IconLeft, true, SaveManager.instance.speed));

        StartCoroutine(FadeManager.FadeOnly(GradLeft, false, SaveManager.instance.speed));
        StartCoroutine(FadeManager.FadeOnly(IconRight, false, SaveManager.instance.speed));
    }

    public void AppearRight()
    {
        StopAllCoroutines();
        saveFileFields.exitRight = false;

        StartCoroutine(FadeManager.FadeOnly(GradLeft, true, SaveManager.instance.speed));
        StartCoroutine(FadeManager.FadeOnly(IconRight, true, SaveManager.instance.speed));

        StartCoroutine(FadeManager.FadeOnly(GradRight, false, SaveManager.instance.speed));
        StartCoroutine(FadeManager.FadeOnly(IconLeft, false, SaveManager.instance.speed));
    }

    public void ExitLeft()
    {
        StopAllCoroutines();
        saveFileFields.exitLeft = true;

        StartCoroutine(FadeManager.FadeOnly(GradRight, false, SaveManager.instance.speed));
        StartCoroutine(FadeManager.FadeOnly(IconLeft, false, SaveManager.instance.speed));

        if (saveFileFields.exitRight)
        {
            StartCoroutine(FadeManager.FadeOnly(GradLeft, false, SaveManager.instance.speed));
            StartCoroutine(FadeManager.FadeOnly(IconRight, false, SaveManager.instance.speed));
        }
    }

    public void ExitRight()
    {
        StopAllCoroutines();
        saveFileFields.exitRight = true;

        StartCoroutine(FadeManager.FadeOnly(GradLeft, false, SaveManager.instance.speed));
        StartCoroutine(FadeManager.FadeOnly(IconRight, false, SaveManager.instance.speed));

        if (saveFileFields.exitLeft)
        {
            StartCoroutine(FadeManager.FadeOnly(GradRight, false, SaveManager.instance.speed));
            StartCoroutine(FadeManager.FadeOnly(IconLeft, false, SaveManager.instance.speed));
        }
    }

    public void InstantHideAll()
    {
        StopAllCoroutines();
        GradLeft.GetComponent<CanvasGroup>().alpha = 0f;
        GradRight.GetComponent<CanvasGroup>().alpha = 0f;
        IconLeft.GetComponent<CanvasGroup>().alpha = 0f;
        IconRight.GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void HideLeft()
    {
        StartCoroutine(FadeManager.FadeOnly(GradRight, false, SaveManager.instance.speed));
        StartCoroutine(FadeManager.FadeOnly(IconLeft, false, SaveManager.instance.speed));
    }

    public void HideRight()
    {
        StartCoroutine(FadeManager.FadeOnly(GradLeft, false, SaveManager.instance.speed));
        StartCoroutine(FadeManager.FadeOnly(IconRight, false, SaveManager.instance.speed));
    }
}
