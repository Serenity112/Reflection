using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IButtonManager : MonoBehaviour
{
    /*    public static GameButtonsManager instance = null;
    */
    public bool ButtonSelected = false;

    public List<GameObject> GameButtons;

    public void SubscribeButton(GameObject button)
    {
        GameButtons.Add(button);
    }

    public abstract void UnSelectButtons();

    public abstract void AppearActualButton();

    // Включить взаимодействие с кнопоками
    public abstract void EnableButtons();

    // Отключить взаимодействие с кнопоками
    public abstract void DisableButtons();
}
