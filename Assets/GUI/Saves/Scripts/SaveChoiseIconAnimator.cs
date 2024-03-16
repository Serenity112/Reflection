using UnityEngine;

public class SaveChoiseIconAnimator : MonoBehaviour
{
    public GameObject GradLeft;
    public GameObject GradRight;
    public GameObject IconLeft;
    public GameObject IconRight;

    private SaveFileFields saveFileFields;

    private float _speed = 4f;

    private bool exitLeft { get; set; } = true;
    private bool exitRight { get; set; } = true;

    private void Awake()
    {
        saveFileFields = GetComponent<SaveFileFields>();
    }

    public void AppearSide(Side side)
    {
        StartCoroutine(saveFileFields.CloseOverPanel());

        switch (side)
        {
            case Side.Left:
                StopAllCoroutines();
                exitLeft = false;
                AppearLeft();
                break;

            case Side.Right:
                StopAllCoroutines();
                exitRight = false;
                AppearRight();
                break;
        }
    }

    public void AppearLeft()
    {
        StartCoroutine(FadeManager.FadeOnly(GradRight, true, _speed));
        StartCoroutine(FadeManager.FadeOnly(IconLeft, true, _speed));

        StartCoroutine(FadeManager.FadeOnly(GradLeft, false, _speed));
        StartCoroutine(FadeManager.FadeOnly(IconRight, false, _speed));
    }

    public void AppearRight()
    {
        StartCoroutine(FadeManager.FadeOnly(GradLeft, true, _speed));
        StartCoroutine(FadeManager.FadeOnly(IconRight, true, _speed));

        StartCoroutine(FadeManager.FadeOnly(GradRight, false, _speed));
        StartCoroutine(FadeManager.FadeOnly(IconLeft, false, _speed));
    }


    public void RemoveSide(Side side)
    {
        switch (side)
        {
            case Side.Left:
                StopAllCoroutines();
                exitLeft = true;
                ExitLeft();

                if (exitRight)
                {
                    ExitRight();
                    StartCoroutine(saveFileFields.OpenOverPanel());
                }

                break;
            case Side.Right:
                StopAllCoroutines();
                exitRight = true;
                ExitRight();

                if (exitLeft)
                {
                    ExitLeft();
                    StartCoroutine(saveFileFields.OpenOverPanel());
                }

                break;
        }
    }

    public void ExitLeft()
    {
        StartCoroutine(FadeManager.FadeOnly(GradRight, false, _speed));
        StartCoroutine(FadeManager.FadeOnly(IconLeft, false, _speed));
    }

    public void ExitRight()
    {
        StartCoroutine(FadeManager.FadeOnly(GradLeft, false, _speed));
        StartCoroutine(FadeManager.FadeOnly(IconRight, false, _speed));
    }

    public void ResetPanel()
    {
        StopAllCoroutines();
        GradLeft.GetComponent<CanvasGroup>().alpha = 0f;
        GradRight.GetComponent<CanvasGroup>().alpha = 0f;
        IconLeft.GetComponent<CanvasGroup>().alpha = 0f;
        IconRight.GetComponent<CanvasGroup>().alpha = 0f;
    }
}
