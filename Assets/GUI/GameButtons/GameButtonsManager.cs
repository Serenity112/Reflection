using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtonsManager : MonoBehaviour
{
    public static GameButtonsManager instance = null;

    private List<GameObject> GameButtons;

    [HideInInspector] public bool ButtonSelected = false;

    private void Awake()
    {
        instance = this;

        GameButtons = new List<GameObject>();
    }

    public void SubscribeButton(GameObject button)
    {
        GameButtons.Add(button);
    }

    public IEnumerator HideTextBoxButtons(float speed)
    {
        List<IEnumerator> list = new List<IEnumerator>();
        foreach (var button_obj in GameButtons)
        {
            list.Add(FadeManager.FadeObject(button_obj, false, speed));
        }
        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(list));
    }

    public IEnumerator ShowAllButtons(float speed)
    {
        List<IEnumerator> list = new List<IEnumerator>();
        foreach (var button_obj in GameButtons)
        {
            list.Add(FadeManager.FadeObject(button_obj, true, speed));
        }
        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(list));
    }

    public void AppearActualButton()
    {
        foreach (var button_obj in GameButtons)
        {
            if (button_obj.GetComponent<GameButtonComponent>() != null)
            {
                button_obj.GetComponent<GameButtonComponent>().AppearIfEntered();
            }
        }
    }

    /*public IEnumerator ShowTextBoxButtons(float speed)
    {
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeObject(ContinueGame, true, speed),
            FadeManager.FadeObject(SkipButton, true, speed),
            FadeManager.FadeObject(HideButton, true, speed),
            FadeManager.FadeObject(LogButton, true, speed)
        });
    }*/
}
