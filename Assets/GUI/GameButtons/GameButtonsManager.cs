using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtonsManager : MonoBehaviour
{
    public static GameButtonsManager instance = null;

    private void Awake()
    {
        instance = this;

        GameButtons = gameObject;
    }

    public GameObject GameButtons;

    public GameObject PauseButton;

    public GameObject SkipButton;

    public GameObject HideButton;

    public GameObject LogButton;

    public GameObject ContinueGame;

    public IEnumerator HideAllButtons(float speed)
    {
        StartCoroutine(FadeManager.FadeObject(PauseButton, false, speed));
        yield return HideTextBoxButtons(speed);
    }

    public IEnumerator HideTextBoxButtons(float speed)
    {
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeObject(ContinueGame, false, speed),
            FadeManager.FadeObject(SkipButton, false, speed),
            FadeManager.FadeObject(HideButton, false, speed),
            FadeManager.FadeObject(LogButton, false, speed)
        });
    }

    public IEnumerator ShowAllButtons(float speed)
    {
        StartCoroutine(FadeManager.FadeObject(PauseButton, true, speed));
        yield return ShowTextBoxButtons(speed);
    }

    public IEnumerator ShowTextBoxButtons(float speed)
    {
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeObject(ContinueGame, true, speed),
            FadeManager.FadeObject(SkipButton, true, speed),
            FadeManager.FadeObject(HideButton, true, speed),
            FadeManager.FadeObject(LogButton, true, speed)
        });
    }
}
