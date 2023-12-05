using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Игровые кнопки - пауза, хайд, скип, лог
public class GameButtonsManager : IButtonManager
{
    public static GameButtonsManager instance = null;

    public void Awake()
    {
        instance = this;
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

    public override void ResetAllButtonsState()
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

    public override void ResetManager()
    {
        throw new System.NotImplementedException();
    }
}
