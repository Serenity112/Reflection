using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ћенеджеры групп кнопок нужны дл€ общих действий с группами кнопок
public abstract class IButtonManager : MonoBehaviour
{
    protected List<IDraggableButton> GameButtons = new List<IDraggableButton>();

    public void SubscribeButton(IDraggableButton button)
    {
        GameButtons.Add(button);
    }

    /// <summary>
    /// —бросить состо€ни€ всех кнопок
    /// </summary>
    public virtual void ResetAllButtonsState()
    {
        foreach (var button in GameButtons)
        {
            button.ResetButtonState();
        }
    }

    /// <summary>
    /// —бросить всЄ по умолчанию
    /// </summary>
    public abstract void ResetManager();
}
