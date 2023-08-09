using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtonsManager : IButtonManager
{
    public static GameButtonsManager instance = null;

    private void Awake()
    {
        instance = this;

        GameButtons = new List<GameObject>();
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

    public override void AppearActualButton()
    {
        foreach (var button_obj in GameButtons)
        {
            if (button_obj.GetComponent<GameButtonComponent>() != null)
            {
                button_obj.GetComponent<GameButtonComponent>().AppearIfEntered();
            }
        }
    }

    public override void UnSelectButtons()
    {
        // -
    }

    public override void EnableButtons()
    {
        // -
    }

    public override void DisableButtons()
    {
        // -
    }
}
