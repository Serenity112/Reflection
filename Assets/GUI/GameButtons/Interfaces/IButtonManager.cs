using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ћенеджеры групп кнопок нужны дл€ общих действий с группами кнопок
public abstract class IButtonManager : MonoBehaviour
{
    public bool ButtonSelected = false;

    public List<GameObject> GameButtons = new List<GameObject>();

    public void SubscribeButton(GameObject button)
    {
        GameButtons.Add(button);
    }

    /// <summary>
    /// —бросить состо€ни€ всех кнопок
    /// </summary>
    public abstract void ResetAllButtonsState();

    /// <summary>
    /// ¬ключить коллайдеры
    /// </summary>
    public abstract void EnableButtons();

    /// <summary>
    /// ќтключить коллайдеры
    /// </summary>
    public abstract void DisableButtons();

    /// <summary>
    /// —бросить всЄ по умолчанию
    /// </summary>
    public abstract void ResetManager();
}
